namespace EMS20.WebApi.Application.Identity.Tokens;

public record RefreshTokenRequest(string Token, string RefreshToken);