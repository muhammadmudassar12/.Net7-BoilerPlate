using EMS20.WebApi.Application.Common.FileStorage;

namespace EMS20.WebApi.Application.Identity.Users;

public class UpdateUserProfileRequest
{
    public string Id { get; set; } = default!;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? ImageUrl { get; set; }
    public bool DeleteCurrentImage { get; set; } = false;
}