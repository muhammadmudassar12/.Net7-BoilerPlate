using Azure;
using DocumentFormat.OpenXml.Spreadsheet;
using EMS20.WebApi.Application.Identity.Roles;
using EMS20.WebApi.Application.Identity.Tokens;
using EMS20.WebApi.Application.Identity.Users;
using System.Security.Claims;

namespace EMS20.WebApi.Host.Controllers.Identity;

public sealed class TokensController : VersionedApiController
{
    private readonly ITokenService _tokenService;
    private readonly IUserService _userService;
    private readonly IRoleService _roleService;

    public TokensController(ITokenService tokenService, IUserService userService, IRoleService roleService)
    {
        _userService = userService;
        _tokenService = tokenService;
        _roleService = roleService;
    }

    [HttpPost("token.gettokenasync")]
    [AllowAnonymous]
    [TenantIdHeader]
    [OpenApiOperation("Request an access token using credentials.", "")]
    public async Task<ResultResponse<LoginResponse>> GetTokenAsync(TokenRequest request, CancellationToken cancellationToken)
    {

        var response = await _tokenService.GetTokenAsync(request, GetIpAddress()!, cancellationToken);

        return new ResultResponse<LoginResponse>
        {
            Data = response,
            Message = "Request Executed Successfully",
            Success = true
        };
    }

    [HttpPost("token.getrefreshtokenasync")]
    [AllowAnonymous]
    [TenantIdHeader]
    [OpenApiOperation("Request an access token using a refresh token.", "")]
    [ApiConventionMethod(typeof(FSHApiConventions), nameof(FSHApiConventions.Search))]
    public async Task<ResultResponse<TokenResponse>> RefreshAsync(RefreshTokenRequest request)
    {
        return new ResultResponse<TokenResponse>
        {
            Data = await _tokenService.RefreshTokenAsync(request, GetIpAddress()!),
            Message = "Request Executed Successfully",
            Success = true
        };

    }

    [HttpPost("token.logoutuserasync")]
    [OpenApiOperation("Logout the user.", "")]
    public async Task<ResultResponse<string>> LogoutAsync()
    {
        var userInfo = User;
        var userId = new Guid(User.FindFirstValue("uid"));

        await _tokenService.LogoutAsync(userId);
        return new ResultResponse<string>
        {
            Data = "User Logged out Successfully",
            Message = "Request Executed Successfully",
            Success = true
        };

    }



    private string? GetIpAddress() =>
        Request.Headers.ContainsKey("X-Forwarded-For")
            ? Request.Headers["X-Forwarded-For"]
            : HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "N/A";
}