using System.Text.Json.Serialization;

namespace EMS20.WebApi.Application.Identity.Users;

public class UserDetailsDto
{
    public DefaultIdType Id { get; set; }

    public string? UserName { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public bool IsActive { get; set; }

    public bool EmailConfirmed { get; set; }

    public string? PhoneNumber { get; set; }

    public string? ImageUrl { get; set; }

    public List<UserRoleDto>? Roles { get; set; }
}

public class UserDetailsWithoutRoleDto
{
    public DefaultIdType Id { get; set; }

    public string? UserName { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public bool IsActive { get; set; }

    public bool EmailConfirmed { get; set; }

    public string? PhoneNumber { get; set; }

    public string? ImageUrl { get; set; }

}