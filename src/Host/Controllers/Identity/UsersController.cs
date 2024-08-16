using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using EMS20.WebApi.Application.Identity.Roles;
using EMS20.WebApi.Application.Identity.Users;
using EMS20.WebApi.Application.Identity.Users.Password;
using EMS20.WebApi.Application.System.DeviceRegisteration.Commands;
using EMS20.WebApi.Host.Controllers;
using EMS20.WebApi.Shared.Authorization;
using System.Threading;

namespace EMS20.WebApi.Host.Controllers.Identity;

public class UsersController : VersionedApiController
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService) => _userService = userService;

    [HttpGet("users.getlistofallusersasync")]
    [MustHavePermission(FSHAction.View, FSHResource.Users)]
    [OpenApiOperation("Get list of all users.", "")]
    public async Task<ResultResponse<List<UserDetailsDto>>> GetListAsync(CancellationToken cancellationToken)
    {
        var users = await _userService.GetListAsync(cancellationToken);

        foreach (var user in users)
        {
            user.Roles = await _userService.GetRolesAsync(user.Id.ToString(), cancellationToken);
        }
        return new ResultResponse<List<UserDetailsDto>>
        {
            Data = users,
            Message = "Request Executed Successfully",
            Success = true
        };

    }

    [HttpGet("users.getuserdetailsbyidasync")]
    [MustHavePermission(FSHAction.View, FSHResource.Users)]
    [OpenApiOperation("Get a user's details.", "")]
    public async Task<ResultResponse<UserDetailsDto>> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        var user = await _userService.GetAsync(id, cancellationToken);
        user.Roles = await _userService.GetRolesAsync(id, cancellationToken);
        return new ResultResponse<UserDetailsDto>
        {
            Data = user,
            Message = "Request Executed Successfully",
            Success = true
        };
    }

    [HttpGet("users.getuserrolebyuserid")]
    [MustHavePermission(FSHAction.View, FSHResource.UserRoles)]
    [OpenApiOperation("Get a user's roles.", "")]
    public async Task<ResultResponse<List<UserRoleDto>>> GetRolesAsync(string id, CancellationToken cancellationToken)
    {
        return new ResultResponse<List<UserRoleDto>>
        {
            Data = await _userService.GetRolesAsync(id, cancellationToken),
            Message = "Request Executed Successfully",
            Success = true
        };
    }

    [HttpPost("users.assignnewroletouserasync")]
    [ApiConventionMethod(typeof(FSHApiConventions), nameof(FSHApiConventions.Register))]
    [MustHavePermission(FSHAction.Update, FSHResource.UserRoles)]
    [OpenApiOperation("Assign new role to user by id.", "")]
    public async Task<ResultResponse<string>> AssignRolesAsync(string id, UserRolesRequest request, CancellationToken cancellationToken)
    {
        return new ResultResponse<string>
        {
            Data = await _userService.AssignRolesAsync(id, request, cancellationToken),
            Message = "Request Executed Successfully",
            Success = true
        };
    }

    [HttpPost("users.createnewuserasync")]
    [TenantIdHeader]
    [MustHavePermission(FSHAction.Create, FSHResource.Users)]
    [OpenApiOperation("Creates a new user.", "")]
    public async Task<ResultResponse<string>> CreateAsync(CreateUserRequest request)
    {
        // TODO: check if registering anonymous users is actually allowed (should probably be an appsetting)
        // and return UnAuthorized when it isn't
        // Also: add other protection to prevent automatic posting (captcha?)
        return new ResultResponse<string>
        {
            Data = await _userService.CreateAsync(request, GetOriginFromRequest()),
            Message = $"Request Executed Successfully. Please check {request.Email} to verify your account!",
            Success = true
        };
    }

    [HttpPost("users.createuseranonymouslyasync")]
    [TenantIdHeader]
    [AllowAnonymous]
    [OpenApiOperation("Anonymous user creates a user.", "")]
    [ApiConventionMethod(typeof(FSHApiConventions), nameof(FSHApiConventions.Register))]
    public async Task<ResultResponse<string>> SelfRegisterAsync(CreateAnonymousUserRequest request)
    {
        // TODO: check if registering anonymous users is actually allowed (should probably be an appsetting)
        // and return UnAuthorized when it isn't
        // Also: add other protection to prevent automatic posting (captcha?)
        return new ResultResponse<string>
        {
            Data = await _userService.CreateAnonymousAsync(request, GetOriginFromRequest()),
            Message = $"Request Executed Successfully. Please check {request.Email} to verify your account!",
            Success = true
        };
    }

    [HttpPost("users.toggleuserstatusasync")]
    [MustHavePermission(FSHAction.Update, FSHResource.Users)]
    [ApiConventionMethod(typeof(FSHApiConventions), nameof(FSHApiConventions.Register))]
    [OpenApiOperation("Toggle a user's active status.", "")]
    public async Task<ActionResult> ToggleStatusAsync(string id, ToggleUserStatusRequest request, CancellationToken cancellationToken)
    {

        if (id != request.UserId)
        {
            return BadRequest(new ResultResponse<string>
            {
                Data = "UserID is Missing",
                Message = "Bad Request",
                Success = false
            });
        }

        await _userService.ToggleStatusAsync(request, cancellationToken);
        return Ok(new ResultResponse<string>
        {
            Data = "Success",
            Message = "Request Executed Successfully",
            Success = true
        });
    }

    [HttpGet("users.confirmemailofuserasync")]
    [TenantIdHeader]
    [AllowAnonymous]
    [OpenApiOperation("Confirm email address for a user.", "")]
    [ApiConventionMethod(typeof(FSHApiConventions), nameof(FSHApiConventions.Search))]
    public async Task<ResultResponse<string>> ConfirmEmailAsync([FromQuery] string userId, [FromQuery] string code, CancellationToken cancellationToken)
    {
        return new ResultResponse<string>
        {
            Data = await _userService.ConfirmEmailAsync(userId, code, cancellationToken),
            Message = "Request Executed Successfully",
            Success = true
        };
    }

    //[HttpGet("users.confirmuserphonenumberasync")]
    //[AllowAnonymous]
    //[TenantIdHeader]
    //[OpenApiOperation("Confirm phone number for a user.", "")]
    //[ApiConventionMethod(typeof(FSHApiConventions), nameof(FSHApiConventions.Search))]
    //public async Task<ResultResponse<string>> ConfirmPhoneNumberAsync([FromQuery] string userId, [FromQuery] string code)
    //{
    //    return new ResultResponse<string>
    //    {
    //        Data = await _userService.ConfirmPhoneNumberAsync(userId, code),
    //        Message = "Request Executed Successfully",
    //        Success = true
    //    };
    //}

    [HttpPost("users.forgotpasswordasync")]
    [AllowAnonymous]
    [TenantIdHeader]
    [OpenApiOperation("Request a password reset email for a user.", "")]
    [ApiConventionMethod(typeof(FSHApiConventions), nameof(FSHApiConventions.Register))]
    public async Task<ResultResponse<ForgotPasswordResponse>> ForgotPasswordAsync([FromQuery]ForgotPasswordRequest request)
    {
        return new ResultResponse<ForgotPasswordResponse>
        {
            Data = await _userService.ForgotPasswordAsync(request, GetOriginFromRequest()),
            Message = "Request Executed Successfully. Reset Password OTP has been sent. Please check your email address.",
            Success = true
        };
    }

    [HttpPost("users.resetpasswordasync")]
    [OpenApiOperation("Reset a user's password.", "")]
    [AllowAnonymous]
    [ApiConventionMethod(typeof(FSHApiConventions), nameof(FSHApiConventions.Register))]
    public async Task<ResultResponse<string>> ResetPasswordAsync([FromQuery]ResetPasswordRequest request)
    {
        return new ResultResponse<string>
        {
            Data = await _userService.ResetPasswordAsync(request),
            Message = "Request Executed Successfully",
            Success = true
        };
    }


    [HttpPost("user.updateuserasync")]
    [ApiConventionMethod(typeof(FSHApiConventions), nameof(FSHApiConventions.Register))]
   // [MustHavePermission(FSHAction.Update, FSHResource.UserRoles)]
    [OpenApiOperation("Update a user's detail.", "")]
    public async Task<ResultResponse<string>> UpdateUserAsync(string id, UpdateUserRequest request, CancellationToken cancellationToken)
    {
        return new ResultResponse<string>
        {
            Data = await _userService.UpdateUserAsync(id, request, cancellationToken),
            Message = "Request Executed Successfully",
            Success = true
        };
    }
    [HttpPost("user.deleteuserasync")]
    [ApiConventionMethod(typeof(FSHApiConventions), nameof(FSHApiConventions.Register))]
    // [MustHavePermission(FSHAction.Delete, FSHResource.UserRoles)]
    [OpenApiOperation("delete a user.", "")]
    public async Task<ResultResponse<string>> deleteuserasync(string id, CancellationToken cancellationToken)
    {
        return new ResultResponse<string>
        {
            Data = await _userService.DeleteUserAsync(id, cancellationToken),
            Message = "Request Executed Successfully",
            Success = true
        };
    }

    [HttpGet("users.resendconfirmationemail")]
    [AllowAnonymous]
    [TenantIdHeader]
    [OpenApiOperation("Resend Confirmation email to user..", "")]
    [ApiConventionMethod(typeof(FSHApiConventions), nameof(FSHApiConventions.Search))]
    public async Task<ResultResponse<string>> ResendConfirmationEmail([FromQuery] DefaultIdType userId)
    {
        return new ResultResponse<string>
        {
            Data = await _userService.ResendEmail(userId, GetOriginFromRequest()),
            Message = "Request Executed Successfully. OTP has been resent. Please check your email address.",
            Success = true
        };
    }

    [HttpGet("users.verifyOtpAsync")]
    [AllowAnonymous]
    [TenantIdHeader]
    [OpenApiOperation("Verify otp.", "")]
    [ApiConventionMethod(typeof(FSHApiConventions), nameof(FSHApiConventions.Search))]
    public async Task<ResultResponse<OtpVerificationResponse>> verifyOtpAsync([FromQuery]OtpVerificationRequest request)
    {
        return new ResultResponse<OtpVerificationResponse>
        {
            Data = await _userService.VerifyOtp(request),
            Message = "Otp Verified Successfully",
            Success = true
        };
    }

    [HttpPost("users.registerDeviceAsync")]
    [AllowAnonymous]
    [OpenApiOperation("Register Device.", "")]
    public async Task<ResultResponse<object>> registerDeviceAsync(RegisterDeviceRequestAsync request)
    {
        return new ResultResponse<object>
        {
            Data = await Mediator.Send(request),
            Message = "Otp Verified Successfully",
            Success = true
        };
    }

    [HttpPost("users.udpateDeviceAsync")]
    [AllowAnonymous]
    [OpenApiOperation("Register Device.", "")]
    public async Task<ResultResponse<object>> udpateDeviceAsync(UpdateRegisterDeviceRequestAsync request)
    {
        return new ResultResponse<object>
        {
            Data = await Mediator.Send(request),
            Message = "Otp Verified Successfully",
            Success = true
        };
    }

    private string GetOriginFromRequest() => $"{Request.Scheme}://{Request.Host.Value}{Request.PathBase.Value}";
}
