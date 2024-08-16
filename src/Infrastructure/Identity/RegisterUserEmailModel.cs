namespace EMS20.WebApi.Infrastructure.Identity;

public class RegisterUserEmailModel
{
    public string UserName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Url { get; set; } = default!;
}

public class ForgotPasswordEmailModel
{
    public string Email { get; set; } = default!;
    public string Url { get; set; } = default!;
    public string Code { get; set; } = default!;
}

