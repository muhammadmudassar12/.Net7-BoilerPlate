using System.Security.Claims;
using EMS20.WebApi.Application.Common.Exceptions;
using EMS20.WebApi.Application.Common.Mailing;
using EMS20.WebApi.Domain.Common;
using EMS20.WebApi.Domain.Identity;
using EMS20.WebApi.Shared.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using EMS20.WebApi.Application.Identity.Users;

namespace EMS20.WebApi.Infrastructure.Identity;

internal partial class UserService
{
    /// <summary>
    /// This is used when authenticating with AzureAd.
    /// The local user is retrieved using the objectidentifier claim present in the ClaimsPrincipal.
    /// If no such claim is found, an InternalServerException is thrown.
    /// If no user is found with that ObjectId, a new one is created and populated with the values from the ClaimsPrincipal.
    /// If a role claim is present in the principal, and the user is not yet in that roll, then the user is added to that role.
    /// </summary>
    public async Task<string> GetOrCreateFromPrincipalAsync(ClaimsPrincipal principal)
    {
        string? objectId = principal.GetObjectId();
        if (string.IsNullOrWhiteSpace(objectId))
        {
            throw new InternalServerException(_t["Invalid objectId"]);
        }

        var user = await _userManager.Users.Where(u => u.ObjectId == objectId).FirstOrDefaultAsync()
            ?? await CreateOrUpdateFromPrincipalAsync(principal);

        if (principal.FindFirstValue(ClaimTypes.Role) is string role &&
            await _roleManager.RoleExistsAsync(role) &&
            !await _userManager.IsInRoleAsync(user, role))
        {
            await _userManager.AddToRoleAsync(user, role);
        }

        return user.Id;
    }

    private async Task<ApplicationUser> CreateOrUpdateFromPrincipalAsync(ClaimsPrincipal principal)
    {
        string? email = principal.FindFirstValue(ClaimTypes.Upn);
        string? username = principal.GetDisplayName();
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(username))
        {
            throw new InternalServerException(string.Format(_t["Username or Email not valid."]));
        }

        var user = await _userManager.FindByNameAsync(username);
        if (user is not null && !string.IsNullOrWhiteSpace(user.ObjectId))
        {
            throw new InternalServerException(string.Format(_t["Username {0} is already taken."], username));
        }

        if (user is null)
        {
            user = await _userManager.FindByEmailAsync(email);
            if (user is not null && !string.IsNullOrWhiteSpace(user.ObjectId))
            {
                throw new InternalServerException(string.Format(_t["Email {0} is already taken."], email));
            }
        }

        IdentityResult? result;
        if (user is not null)
        {
            user.ObjectId = principal.GetObjectId();
            result = await _userManager.UpdateAsync(user);

            await _events.PublishAsync(new ApplicationUserUpdatedEvent(user.Id));
        }
        else
        {
            user = new ApplicationUser
            {
                ObjectId = principal.GetObjectId(),
                FirstName = principal.FindFirstValue(ClaimTypes.GivenName),
                LastName = principal.FindFirstValue(ClaimTypes.Surname),
                Email = email,
                NormalizedEmail = email.ToUpperInvariant(),
                UserName = username,
                NormalizedUserName = username.ToUpperInvariant(),
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                IsActive = true
            };
            result = await _userManager.CreateAsync(user);

            await _events.PublishAsync(new ApplicationUserCreatedEvent(user.Id));
        }

        if (!result.Succeeded)
        {
            throw new InternalServerException(_t["Validation Errors Occurred."], result.GetErrors(_t));
        }

        return user;
    }

    public async Task<string> CreateAsync(CreateUserRequest request, string origin)
    {
        var existingUserByEmail = await _userManager.FindByEmailAsync(request.Email);
        var existingUserByPhoneNumber = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == request.PhoneNumber);
        var existingUserByUserName = await _userManager.FindByNameAsync(request.UserName);

        var existingUser = existingUserByEmail ?? existingUserByPhoneNumber ?? existingUserByUserName;

        if (existingUser != null)
        {
            if (existingUser.DeletedBy != null || existingUser.DeletedOn != null)
            {
                existingUser.FirstName = request.FirstName;
                existingUser.LastName = request.LastName;
                existingUser.IsActive = true;
                existingUser.Email = request.Email;
                existingUser.PhoneNumber = request.PhoneNumber;
                existingUser.UserName = request.UserName;
                existingUser.ImageUrl= request.ImageUrl;

                existingUser.DeletedOn = null;
                existingUser.DeletedBy = null;

                var result = await _userManager.UpdateAsync(existingUser);
                if (!result.Succeeded)
                {
                    throw new InternalServerException(_t["Validation Errors Occurred."], result.GetErrors(_t));
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(existingUser);
                var passwordResetResult = await _userManager.ResetPasswordAsync(existingUser, token, request.Password);
            }
            else
            {
                // If user exists and is not soft-deleted, return error
                var errorList = new List<string>();

                if (existingUserByEmail != null)
                {
                    errorList.Add("Email already exists.");
                }

                if (existingUserByPhoneNumber != null)
                {
                    errorList.Add("Phone Number already exists.");
                }

                if (existingUserByUserName != null)
                {
                    errorList.Add("Username already exists.");
                }

                throw new CustomException("User Creation Error!", errorList, System.Net.HttpStatusCode.InternalServerError);
            }
            return existingUser.Id;
        }
        else
        {
            var user = new ApplicationUser
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.UserName,
                PhoneNumber = request.PhoneNumber,
                IsActive = true,
                ImageUrl = request.ImageUrl,
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                throw new InternalServerException(_t["Validation Errors Occurred."], result.GetErrors(_t));
            }

            if (request.UserRoles != null)
            {
                foreach (var userRole in request.UserRoles)
                {
                    await _userManager.AddToRoleAsync(user, userRole.RoleName!);
                }
            }
            var messages = new List<string> { string.Format(_t["User {0} Registered."], user.UserName) };

            if (_securitySettings.RequireConfirmedAccount && !string.IsNullOrEmpty(user.Email))
            {
                // send verification email
                string emailVerificationUri = await GetEmailVerificationUriAsync(user, origin);
                RegisterUserEmailModel eMailModel = new RegisterUserEmailModel()
                {
                    Email = user.Email,
                    UserName = user.UserName,
                    Url = emailVerificationUri
                };
                var mailRequest = new MailRequest(
                    new List<string> { user.Email },
                    _t["Confirm Registration"],
                    _templateService.GenerateEmailTemplate("email-confirmation", eMailModel));
                await _mailService.SendAsync(mailRequest, CancellationToken.None);
                messages.Add(_t[$"Please check {user.Email} to verify your account!"]);
            }

            await _events.PublishAsync(new ApplicationUserCreatedEvent(user.Id));

            return user.Id;
        }
    }
    public async Task<string> CreateAnonymousAsync(CreateAnonymousUserRequest request, string origin)
    {
        var existingUserByEmail = await _userManager.FindByEmailAsync(request.Email);
        var existingUserByPhoneNumber = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == request.PhoneNumber);
        var existingUserByUserName = await _userManager.FindByNameAsync(request.UserName);

        var existingUser = existingUserByEmail ?? existingUserByPhoneNumber ?? existingUserByUserName;

        if (existingUser != null)
        {
            if (existingUser.DeletedBy != null || existingUser.DeletedOn != null)
            {
                existingUser.FirstName = request.FirstName;
                existingUser.LastName = request.LastName;
                existingUser.IsActive = true;
                existingUser.Email = request.Email;
                existingUser.PhoneNumber = request.PhoneNumber;
                existingUser.UserName = request.UserName;
                existingUser.ImageUrl= request.ImageUrl;

                existingUser.DeletedOn = null;
                existingUser.DeletedBy = null;

                var result = await _userManager.UpdateAsync(existingUser);
                if (!result.Succeeded)
                {
                    throw new InternalServerException(_t["Validation Errors Occurred."], result.GetErrors(_t));
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(existingUser);
                var passwordResetResult = await _userManager.ResetPasswordAsync(existingUser, token, request.Password);
            }
            else
            {
                // If user exists and is not soft-deleted, return error
                var errorList = new List<string>();

                if (existingUserByEmail != null)
                {
                    errorList.Add("Email already exists.");
                }

                if (existingUserByPhoneNumber != null)
                {
                    errorList.Add("Phone Number already exists.");
                }

                if (existingUserByUserName != null)
                {
                    errorList.Add("Username already exists.");
                }

                throw new CustomException("User Creation Error!", errorList, System.Net.HttpStatusCode.InternalServerError);
            }
            return existingUser.Id;
        }
        else
        {
            var user = new ApplicationUser
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.UserName,
                PhoneNumber = request.PhoneNumber,
                IsActive = true,
                ImageUrl = request.ImageUrl,
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                throw new InternalServerException(_t["Validation Errors Occurred."], result.GetErrors(_t));
            }

            await _userManager.AddToRoleAsync(user, FSHRoles.User);

            var messages = new List<string> { string.Format(_t["User {0} Registered."], user.UserName) };

            if (_securitySettings.RequireConfirmedAccount && !string.IsNullOrEmpty(user.Email))
            {
                // send verification email
                string emailVerificationUri = await GetEmailVerificationUriAsync(user, origin);
                RegisterUserEmailModel eMailModel = new RegisterUserEmailModel()
                {
                    Email = user.Email,
                    UserName = user.UserName,
                    Url = emailVerificationUri
                };
                var mailRequest = new MailRequest(
                    new List<string> { user.Email },
                    _t["Confirm Registration"],
                    _templateService.GenerateEmailTemplate("email-confirmation", eMailModel));
                await _mailService.SendAsync(mailRequest, CancellationToken.None);
                messages.Add(_t[$"Please check {user.Email} to verify your account!"]);
            }

            await _events.PublishAsync(new ApplicationUserCreatedEvent(user.Id));

            return user.Id;
        }
    }


    public async Task UpdateAsync(UpdateUserProfileRequest request, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        _ = user ?? throw new NotFoundException(_t["User Not Found."]);

        string currentImage = user.ImageUrl ?? string.Empty;

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.PhoneNumber = request.PhoneNumber;
        user.ImageUrl = request.ImageUrl;
        string? phoneNumber = await _userManager.GetPhoneNumberAsync(user);
        if (request.PhoneNumber != phoneNumber)
        {
            await _userManager.SetPhoneNumberAsync(user, request.PhoneNumber);
        }

        var result = await _userManager.UpdateAsync(user);

        await _signInManager.RefreshSignInAsync(user);

        await _events.PublishAsync(new ApplicationUserUpdatedEvent(user.Id));

        if (!result.Succeeded)
        {
            throw new InternalServerException(_t["Update profile failed"], result.GetErrors(_t));
        }
    }

    public async Task<string> ResendEmail(DefaultIdType UserId, string origin)
    {
        var user = await _userManager.FindByIdAsync(UserId.ToString());
        string emailVerificationUri = await GetEmailVerificationUriAsync(user, origin);
        RegisterUserEmailModel eMailModel = new RegisterUserEmailModel()
        {
            Email = user.Email,
            UserName = user.UserName,
            Url = emailVerificationUri
        };
        var mailRequest = new MailRequest(
            new List<string> { user.Email },
            _t["Confirm Registration"],
            _templateService.GenerateEmailTemplate("email-confirmation", eMailModel));
        var messages = new List<string>();
        await _mailService.SendAsync(mailRequest, CancellationToken.None);
        messages.Add(_t[$"OTP has been resent. Please check {user.Email} to verify your account!"]);
        return string.Join(Environment.NewLine, user.Id);
    }
}
