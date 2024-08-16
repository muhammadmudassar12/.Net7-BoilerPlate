namespace EMS20.WebApi.Application.Identity.Users.Password;

public class ResetPasswordRequest
{
    public string? Email { get; set; }

    public string? Password { get; set; }

    public string? Token { get; set; }
    public string? Otp {  get; set; }   
}