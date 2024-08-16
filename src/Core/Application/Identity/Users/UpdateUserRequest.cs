using EMS20.WebApi.Application.Common.FileStorage;

namespace EMS20.WebApi.Application.Identity.Users;

public class UpdateUserRequest
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? ImageUrl { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Password { get; set; }
    public bool? IsActive { get; set; }
    public List<UserRoleDto>? UserRoles { get; set; }
}