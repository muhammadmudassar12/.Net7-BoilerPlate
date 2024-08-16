using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using EMS20.WebApi.Application.Common.Exceptions;
using EMS20.WebApi.Infrastructure.Auth;
using EMS20.WebApi.Infrastructure.Auth.Jwt;
using EMS20.WebApi.Infrastructure.Multitenancy;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using EMS20.WebApi.Application.Identity.Tokens;
using EMS20.WebApi.Shared.Authorization;
using EMS20.WebApi.Shared.Multitenancy;
using EMS20.WebApi.Application.Identity.Users;
using System.Threading;
using EMS20.WebApi.Domain.Identity;

namespace EMS20.WebApi.Infrastructure.Identity;

internal class TokenService : ITokenService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IStringLocalizer _t;
    private readonly SecuritySettings _securitySettings;
    private readonly JwtSettings _jwtSettings;
    private readonly FSHTenantInfo? _currentTenant;
    private readonly IUserService _userService;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public TokenService(
        UserManager<ApplicationUser> userManager,
        IOptions<JwtSettings> jwtSettings,
        IStringLocalizer<TokenService> localizer,
        FSHTenantInfo? currentTenant,
        IOptions<SecuritySettings> securitySettings,
        IUserService userService,
        SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _t = localizer;
        _jwtSettings = jwtSettings.Value;
        _currentTenant = currentTenant;
        _securitySettings = securitySettings.Value;
        _userService = userService;
        _signInManager = signInManager;
    }

    public async Task<LoginResponse> GetTokenAsync(TokenRequest request, string ipAddress, CancellationToken cancellationToken)
    {
        // Added currenttenantId when gwt token (Mudassar)
        if (string.IsNullOrWhiteSpace(_currentTenant?.Id) || await _userManager.FindByEmailAsync(request.Email.Trim().Normalize()) is not { } user
            || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            throw new NotFoundException(_t["Invalid Email or Password."]);
        }

        if (!user.IsActive)
        {
            throw new UnauthorizedException(_t["User Not Active. Please contact the administrator."]);
        }

        if (_securitySettings.RequireConfirmedAccount && !user.EmailConfirmed)
        {
            throw new UnauthorizedException(_t["E-Mail not confirmed."]);
        }

        var response = new LoginResponse
        {
            user = new UserDetailsWithoutRoleDto
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                ImageUrl = user.ImageUrl,
                Id = new Guid(user.Id),
                UserName = user.UserName,
                IsActive = user.IsActive,
                EmailConfirmed = user.EmailConfirmed,
            },
            roles = await _userManager.GetRolesAsync(user),
            permissions = await _userService.GetPermissionsAsync(user.Id, cancellationToken),
            token = await GenerateTokensAndUpdateUser(user, ipAddress)
        };

        return response;
    }

    public async Task<TokenResponse> RefreshTokenAsync(RefreshTokenRequest request, string ipAddress)
    {
        var userPrincipal = GetPrincipalFromExpiredToken(request.Token);
        string? userEmail = userPrincipal.GetEmail();
        var user = await _userManager.FindByEmailAsync(userEmail!);
        if (user is null)
        {
            throw new UnauthorizedException(_t["Authentication Failed."]);
        }

        if (user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            throw new UnauthorizedException(_t["Invalid Refresh Token."]);
        }

        return await GenerateTokensAndUpdateUser(user, ipAddress);
    }

    private async Task<TokenResponse> GenerateTokensAndUpdateUser(ApplicationUser user, string ipAddress)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var permissions = await _userService.GetPermissionsAsync(user.Id, new CancellationToken());
        string token = GenerateJwt(user, roles, permissions, ipAddress);

        user.RefreshToken = GenerateRefreshToken();
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationInDays);

        await _userManager.UpdateAsync(user);

        return new TokenResponse(token, user.RefreshToken, user.RefreshTokenExpiryTime);
    }

    private string GenerateJwt(ApplicationUser user, IList<string> roles, List<string> permissions, string ipAddress) =>
        GenerateEncryptedToken(GetSigningCredentials(), GetClaims(user, roles, permissions, ipAddress));

    private IEnumerable<Claim> GetClaims(ApplicationUser user, IList<string> roles, List<string> permissions, string ipAddress) =>
        new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email!),
            new(FSHClaims.Fullname, $"{user.FirstName} {user.LastName}"),
            new(ClaimTypes.Name, user.FirstName ?? string.Empty),
            new(ClaimTypes.Surname, user.LastName ?? string.Empty),
            new(FSHClaims.IpAddress, ipAddress),
            new(FSHClaims.Tenant, _currentTenant!.Id),
            new(ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty),
            new Claim("uid", user.Id.ToString())
        }
        .Concat(roles.Select(role => new Claim(ClaimTypes.Role, role)))
        .Concat(permissions.Select(permission => new Claim(FSHClaims.Permission, permission)));

    private static string GenerateRefreshToken()
    {
        byte[] randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private string GenerateEncryptedToken(SigningCredentials signingCredentials, IEnumerable<Claim> claims)
    {
        var token = new JwtSecurityToken(
           claims: claims,
           expires: DateTime.UtcNow.AddMinutes(_jwtSettings.TokenExpirationInMinutes),
           signingCredentials: signingCredentials);
        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key)),
            ValidateIssuer = false,
            ValidateAudience = false,
            RoleClaimType = ClaimTypes.Role,
            ClockSkew = TimeSpan.Zero,
            ValidateLifetime = false
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(
                SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
        {
            throw new UnauthorizedException(_t["Invalid Token."]);
        }

        return principal;
    }

    private SigningCredentials GetSigningCredentials()
    {
        byte[] secret = Encoding.UTF8.GetBytes(_jwtSettings.Key);
        return new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);
    }

    public async Task LogoutAsync(DefaultIdType? UserId)
    {
        await _signInManager.SignOutAsync();
    }

}