using EMS20.WebApi.Application.Common.Exceptions;
using EMS20.WebApi.Application.Common.Mailing;
using Microsoft.AspNetCore.WebUtilities;
using EMS20.WebApi.Application.Identity.Users.Password;
using EMS20.WebApi.Domain.System;
using Microsoft.EntityFrameworkCore;

namespace EMS20.WebApi.Infrastructure.Identity;

internal partial class UserService
{
    public async Task<ForgotPasswordResponse> ForgotPasswordAsync(ForgotPasswordRequest request, string origin)
    {
        //EnsureValidTenant();

        var user = await _userManager.FindByEmailAsync(request.Email.Normalize());
        if (user is null || !await _userManager.IsEmailConfirmedAsync(user))
        {
            // Don't reveal that the user does not exist or is not confirmed
            throw new NotFoundException(_t["User not found!"]);
        }

        // For more information on how to enable account confirmation and password reset please
        // visit https://go.microsoft.com/fwlink/?LinkID=532713

        string code = await _userManager.GeneratePasswordResetTokenAsync(user);

        string sixDigitCode = GenerateRandomCode(6).ToUpper();
        var otp = new OtpCode(code, false, new Guid(user.Id), origin, sixDigitCode, DateTime.Now.AddHours(1));
        _ = await _db.AddAsync(otp);
        await _db.SaveChangesAsync();

        const string route = "account/reset-password";
        var endpointUri = new Uri(string.Concat($"{origin}/", route));
        string passwordResetUrl = QueryHelpers.AddQueryString(endpointUri.ToString(), "Token", code);
        ForgotPasswordEmailModel eMailModel = new ForgotPasswordEmailModel()
        {
            Code = otp.SixDigitCode
        };

        ForgotPasswordResponse response = new ForgotPasswordResponse
        {
            Code = code,
            Email = user.Email
        };



        var mailRequest = new MailRequest(
            new List<string> { user.Email },
            _t["Reset Password"],
            _templateService.GenerateEmailTemplate("reset-password", eMailModel));

        //var mailRequest = new MailRequest(
        //    new List<string> { request.Email },
        //    _t["Reset Password"],
        //    _t[$"Your Password Reset Token is '{code} for the user  {user.Email}'. You can reset your password using the {endpointUri} Endpoint."]);
        _jobService.Enqueue(() => _mailService.SendAsync(mailRequest, CancellationToken.None));

        return response;
    }

    public async Task<string> ResetPasswordAsync(ResetPasswordRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email?.Normalize()!);
        _ = user ?? throw new NotFoundException(_t["User not found!"]);
        var otp = await _db.OtpCodes.FirstOrDefaultAsync(x => x.UserId == new Guid(user.Id) && x.SixDigitCode == request.Otp && x.Code == request.Token);
        // Don't reveal that the user does not exist
        _ = user ?? throw new InternalServerException(_t["Incorrect Otp!"]);

        var result = await _userManager.ResetPasswordAsync(user, otp.Code!, request.Password!);

        return result.Succeeded
            ? _t["Password Reset Successful!"]
            : throw new InternalServerException(_t["An Error has occurred!"]);
    }

    public async Task ChangePasswordAsync(ChangePasswordRequest model, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        _ = user ?? throw new NotFoundException(_t["User Not Found."]);

        var result = await _userManager.ChangePasswordAsync(user, model.Password, model.NewPassword);

        if (!result.Succeeded)
        {
            throw new InternalServerException(_t["Change password failed"], result.GetErrors(_t));
        }
    }

    public async Task<OtpVerificationResponse> VerifyOtp(OtpVerificationRequest model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email?.Normalize()!);
        _ = user ?? throw new NotFoundException(_t["User not found!"]);

        var otp = await _db.OtpCodes.FirstOrDefaultAsync(x => x.UserId == new Guid(user.Id) && x.SixDigitCode == model.Otp && x.IsUtilized == false);

        _ = user ?? throw new NotFoundException(_t["Incorrect Otp!"]);

        return new OtpVerificationResponse
        {
            Token = otp.Code,
            Email = user.Email
        };
    }
}