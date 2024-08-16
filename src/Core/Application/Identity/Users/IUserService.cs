using System.Security.Claims;
using EMS20.WebApi.Application.Common.Interfaces;
using EMS20.WebApi.Application.Common.Models;
using EMS20.WebApi.Application.Identity.Users.Password;
using EMS20.WebApi.Domain.Identity;
using EMS20.WebApi.Shared.Authorization;
using EMS20.WebApi.Shared.Events;
using EMS20.WebApi.Shared.Multitenancy;

namespace EMS20.WebApi.Application.Identity.Users;

public interface IUserService : ITransientService
{
    Task<PaginationResponse<UserDetailsDto>> SearchAsync(UserListFilter filter, CancellationToken cancellationToken);

    Task<bool> ExistsWithNameAsync(string name);
    Task<bool> ExistsWithEmailAsync(string email, string? exceptId = null);
    Task<bool> ExistsWithPhoneNumberAsync(string phoneNumber, string? exceptId = null);

    Task<List<UserDetailsDto>> GetListAsync(CancellationToken cancellationToken);

    Task<int> GetCountAsync(CancellationToken cancellationToken);

    Task<UserDetailsDto> GetAsync(string userId, CancellationToken cancellationToken);

    Task<List<UserRoleDto>> GetRolesAsync(string userId, CancellationToken cancellationToken);
    Task<string> AssignRolesAsync(string userId, UserRolesRequest request, CancellationToken cancellationToken);

    Task<List<string>> GetPermissionsAsync(string userId, CancellationToken cancellationToken);
    Task<bool> HasPermissionAsync(string userId, string permission, CancellationToken cancellationToken = default);
    Task InvalidatePermissionCacheAsync(string userId, CancellationToken cancellationToken);

    Task ToggleStatusAsync(ToggleUserStatusRequest request, CancellationToken cancellationToken);

    Task<string> GetOrCreateFromPrincipalAsync(ClaimsPrincipal principal);
    Task<string> CreateAsync(CreateUserRequest request, string origin);
    Task<string> CreateAnonymousAsync(CreateAnonymousUserRequest request, string origin);
    Task UpdateAsync(UpdateUserProfileRequest request, string userId);

    Task<string> ConfirmEmailAsync(string userId, string code, CancellationToken cancellationToken);
    Task<string> ConfirmPhoneNumberAsync(string userId, string code);

    Task<ForgotPasswordResponse> ForgotPasswordAsync(ForgotPasswordRequest request, string origin);
    Task<string> ResetPasswordAsync(ResetPasswordRequest request);
    Task ChangePasswordAsync(ChangePasswordRequest request, string userId);
    Task<string> UpdateUserAsync(string userId, UpdateUserRequest request, CancellationToken cancellationToken);
    Task<string> DeleteUserAsync(string userId, CancellationToken cancellationToken);
    Task<ResultResponse<Dictionary<string, List<UserDetailsWithoutRoleDto>>>> ListOfUsersByRole(List<DefaultIdType> roleId, CancellationToken cancellationToken);
    Task<string> ResendEmail(DefaultIdType UserId, string origin);
    Task<OtpVerificationResponse> VerifyOtp(OtpVerificationRequest model);
    Task<List<ApplicationUser>> GetUsersByRolesAsync(List<string> roles);
}