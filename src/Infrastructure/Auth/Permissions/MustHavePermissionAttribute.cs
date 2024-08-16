using Microsoft.AspNetCore.Authorization;
using EMS20.WebApi.Shared.Authorization;

namespace EMS20.WebApi.Infrastructure.Auth.Permissions;

public class MustHavePermissionAttribute : AuthorizeAttribute
{
    public MustHavePermissionAttribute(string action, string resource) =>
        Policy = FSHPermission.NameFor(action, resource);
}