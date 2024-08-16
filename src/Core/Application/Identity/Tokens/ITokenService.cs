using EMS20.WebApi.Application.Common.Interfaces;

namespace EMS20.WebApi.Application.Identity.Tokens;

public interface ITokenService : ITransientService
{
    Task<LoginResponse> GetTokenAsync(TokenRequest request, string ipAddress, CancellationToken cancellationToken);

    Task<TokenResponse> RefreshTokenAsync(RefreshTokenRequest request, string ipAddress);
    Task LogoutAsync(DefaultIdType? UserId);
}