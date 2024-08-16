using EMS20.WebApi.Application.Auditing;
using EMS20.WebApi.Application.Identity.Users;
using EMS20.WebApi.Application.Identity.Users.Password;
using EMS20.WebApi.Shared.Authorization;

namespace EMS20.WebApi.Host.Controllers.Personal;

public class PersonalController : VersionedApiController
{
    private readonly IUserService _userService;

    public PersonalController(IUserService userService) => _userService = userService;

    [HttpGet("personal.getcurrentuserdetailasync")]
    [OpenApiOperation("Get profile details of currently logged in user.", "")]
    public async Task<ActionResult<UserDetailsDto>> GetProfileAsync(CancellationToken cancellationToken)
    {
        return User.GetUserId() is not { } userId || string.IsNullOrEmpty(userId)
            ? Unauthorized()
            : Ok(await _userService.GetAsync(userId, cancellationToken));
    }

    [HttpPut("personal.updatecurrentuserdetailasync")]
    [OpenApiOperation("Update profile details of currently logged in user.", "")]
    public async Task<ActionResult> UpdateProfileAsync(UpdateUserProfileRequest request)
    {
        if (User.GetUserId() is not { } userId || string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        await _userService.UpdateAsync(request, userId);
        return Ok();
    }

    [HttpPut("personal.changepasswordasync")]
    [OpenApiOperation("Change password of currently logged in user.", "")]
    [ApiConventionMethod(typeof(FSHApiConventions), nameof(FSHApiConventions.Register))]
    public async Task<ActionResult> ChangePasswordAsync(ChangePasswordRequest model)
    {
        if (User.GetUserId() is not { } userId || string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        await _userService.ChangePasswordAsync(model, userId);
        return Ok();
    }

    [HttpGet("personal.getcurrentuserpermissionsasync")]
    [OpenApiOperation("Get permissions of currently logged in user.", "")]
    public async Task<ActionResult<List<string>>> GetPermissionsAsync(CancellationToken cancellationToken)
    {
        return User.GetUserId() is not { } userId || string.IsNullOrEmpty(userId)
            ? Unauthorized()
            : Ok(await _userService.GetPermissionsAsync(userId, cancellationToken));
    }

    [HttpGet("personal.getcurrentuserlogsasync")]
    [OpenApiOperation("Get audit logs of currently logged in user.", "")]
    public Task<List<AuditDto>> GetLogsAsync()
    {
        return Mediator.Send(new GetMyAuditLogsRequest());
    }
}