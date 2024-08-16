using Microsoft.AspNetCore.Authorization;
using EMS20.WebApi.Application.Identity.Users;
using EMS20.WebApi.Shared.Authorization;

namespace EMS20.WebApi.Infrastructure.Auth.Permissions;

internal class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IUserService _userService;

    public PermissionAuthorizationHandler(IUserService userService) =>
        _userService = userService;

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        if (context.User?.GetUserId() is { } userId &&
            await _userService.HasPermissionAsync(userId, requirement.Permission))
        {
            context.Succeed(requirement);
        }
    }
}