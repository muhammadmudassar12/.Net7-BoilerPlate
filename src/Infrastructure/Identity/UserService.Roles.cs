using EMS20.WebApi.Application.Common.Exceptions;
using EMS20.WebApi.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using EMS20.WebApi.Application.Identity.Users;
using EMS20.WebApi.Shared.Authorization;
using EMS20.WebApi.Shared.Multitenancy;

namespace EMS20.WebApi.Infrastructure.Identity;

internal partial class UserService
{
    public async Task<List<UserRoleDto>> GetRolesAsync(string userId, CancellationToken cancellationToken)
    {
        var userRoles = new List<UserRoleDto>();

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) throw new NotFoundException("User Not Found.");
        var roles = await _userManager.GetRolesAsync(user);
        if (roles is null) throw new NotFoundException("Roles Not Found.");
        foreach (var roleName in roles)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role != null)
            {
                userRoles.Add(new UserRoleDto
                {
                    RoleId = role.Id,
                    RoleName = role.Name,
                    Description = role.Description,
                    Enabled = true // User is in this role
                });
            }
        }

        return userRoles;
    }

    public async Task<string> AssignRolesAsync(string userId, UserRolesRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        var user = await _userManager.Users.Where(u => u.Id == userId).FirstOrDefaultAsync(cancellationToken);

        _ = user ?? throw new NotFoundException(_t["User Not Found."]);

        // Check if the user is an admin for which the admin role is getting disabled
        if (await _userManager.IsInRoleAsync(user, FSHRoles.Admin)
            && request.UserRoles.Any(a => !a.Enabled && a.RoleName == FSHRoles.Admin))
        {
            // Get count of users in Admin Role
            int adminCount = (await _userManager.GetUsersInRoleAsync(FSHRoles.Admin)).Count;

            // Check if user is not Root Tenant Admin
            // Edge Case : there are chances for other tenants to have users with the same email as that of Root Tenant Admin. Probably can add a check while User Registration
            if (user.Email == MultitenancyConstants.Root.EmailAddress)
            {
                if (_currentTenant.Id == MultitenancyConstants.Root.Id)
                {
                    throw new ConflictException(_t["Cannot Remove Admin Role From Root Tenant Admin."]);
                }
            }
            else if (adminCount <= 2)
            {
                throw new ConflictException(_t["Tenant should have at least 2 Admins."]);
            }
        }

        foreach (var userRole in request.UserRoles)
        {
            // Check if Role Exists
            if (await _roleManager.FindByNameAsync(userRole.RoleName!) is not null)
            {
                if (userRole.Enabled)
                {
                    if (!await _userManager.IsInRoleAsync(user, userRole.RoleName!))
                    {
                        await _userManager.AddToRoleAsync(user, userRole.RoleName!);
                    }
                }
                else
                {
                    await _userManager.RemoveFromRoleAsync(user, userRole.RoleName!);
                }
            }
        }

        await _events.PublishAsync(new ApplicationUserUpdatedEvent(user.Id, true));

        return _t["User Roles Updated Successfully."];
    }

    public async Task<string> UpdateUserAsync(string userId, UpdateUserRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        var user = await _userManager.Users.Where(u => u.Id == userId).FirstOrDefaultAsync(cancellationToken);

        _ = user ?? throw new NotFoundException(_t["User Not Found."]);

        // Check if the user is an admin for which the admin role is getting disabled
        if (await _userManager.IsInRoleAsync(user, FSHRoles.Admin)
            && request.UserRoles.Any(a => !a.Enabled && a.RoleName == FSHRoles.Admin))
        {
            // Get count of users in Admin Role
            int adminCount = (await _userManager.GetUsersInRoleAsync(FSHRoles.Admin)).Count;

            // Check if user is not Root Tenant Admin
            // Edge Case : there are chances for other tenants to have users with the same email as that of Root Tenant Admin. Probably can add a check while User Registration
            if (user.Email == MultitenancyConstants.Root.EmailAddress)
            {
                if (_currentTenant.Id == MultitenancyConstants.Root.Id)
                {
                    throw new ConflictException(_t["Cannot Remove Admin Role From Root Tenant Admin."]);
                }
            }
            else if (adminCount <= 2)
            {
                throw new ConflictException(_t["Tenant should have at least 2 Admins."]);
            }
        }

        if (!string.IsNullOrWhiteSpace(request.FirstName))
        {
            user.FirstName = request.FirstName;
        }

        if (!string.IsNullOrWhiteSpace(request.LastName))
        {
            user.LastName = request.LastName;
        }

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            if (user.Email == request.Email)
            {
                _ = user ?? throw new Exception("Email already exists");
            }

            user.Email = request.Email;
        }

        if (!string.IsNullOrWhiteSpace(request.ImageUrl))
        {
            user.ImageUrl = request.ImageUrl;
        }
        if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            if (user.PhoneNumber.Trim().Replace(" ", "") == request.PhoneNumber.Trim().Replace(" ", ""))
            {
                _ = user ?? throw new Exception("PhoneNumber already exists");
            }
            user.PhoneNumber = request.PhoneNumber;
        }
        if (!request.IsActive.HasValue)
        {
            user.IsActive = (bool)request.IsActive;
        }

        await _userManager.UpdateAsync(user);

        if (request.UserRoles != null)
            foreach (var userRole in request.UserRoles)
            {
                // Check if Role Exists
                if (await _roleManager.FindByNameAsync(userRole.RoleName!) is not null)
                {
                    if (userRole.Enabled)
                    {
                        if (!await _userManager.IsInRoleAsync(user, userRole.RoleName!))
                        {
                            await _userManager.AddToRoleAsync(user, userRole.RoleName!);
                        }
                    }
                    else
                    {
                        await _userManager.RemoveFromRoleAsync(user, userRole.RoleName!);
                    }
                }
            }

        await _events.PublishAsync(new ApplicationUserUpdatedEvent(user.Id, true));

        return _t["User Updated Successfully."];
    }

    public async Task<string> DeleteUserAsync(string userId, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(userId, nameof(userId));

        var user = await _userManager.Users.Where(u => u.Id == userId).FirstOrDefaultAsync(cancellationToken);

        _ = user ?? throw new NotFoundException(_t["User Not Found."]);

        await _userManager.DeleteAsync(user);

        await _events.PublishAsync(new ApplicationUserUpdatedEvent(user.Id, true));

        return _t["User Deleted Successfully."];

    }

    
}