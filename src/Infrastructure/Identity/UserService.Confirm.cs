using System.Text;
using EMS20.WebApi.Application.Common.Exceptions;
using EMS20.WebApi.Infrastructure.Common;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using EMS20.WebApi.Shared.Multitenancy;
using EMS20.WebApi.Domain.System;
using DocumentFormat.OpenXml.Drawing;
using EMS20.WebApi.Domain.Identity;

namespace EMS20.WebApi.Infrastructure.Identity;

internal partial class UserService
{
    private string GenerateRandomCode(int length)
    {
        const string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        StringBuilder sb = new StringBuilder();
        Random random = new Random();

        for (int i = 0; i < length; i++)
        {
            int index = random.Next(validChars.Length);
            sb.Append(validChars[index]);
        }

        return sb.ToString();
    }

    private async Task<string> GetEmailVerificationUriAsync(ApplicationUser user, string origin)
    {
        EnsureValidTenant();

        /*string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        string sixDigitCode = GenerateRandomCode(6).ToUpper();
        var otp = new OtpCode(code, false, new Guid(user.Id), origin, sixDigitCode, DateTime.Now.AddHours(1));
        _ = await _db.AddAsync(otp);
        await _db.SaveChangesAsync();

        const string route = "api/users/confirm-email/";
        var endpointUri = new Uri(string.Concat($"{origin}/", route));
        string verificationUri = QueryHelpers.AddQueryString(endpointUri.ToString(), QueryStringKeys.UserId, user.Id);
        verificationUri = QueryHelpers.AddQueryString(verificationUri, QueryStringKeys.Code, sixDigitCode);
        verificationUri = QueryHelpers.AddQueryString(verificationUri, MultitenancyConstants.TenantIdName, _currentTenant.Id!);
        return verificationUri;*/

        string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        string sixDigitCode = GenerateRandomCode(6).ToUpper();
        var otp = new OtpCode(code, false, new Guid(user.Id), origin, sixDigitCode, DateTime.Now.AddHours(1));
        _ = await _db.AddAsync(otp);
        await _db.SaveChangesAsync();

        const string route = "api/users/confirm-email/";
        var endpointUri = new Uri(string.Concat($"{origin}/", route));
        string verificationUri = QueryHelpers.AddQueryString(endpointUri.ToString(), QueryStringKeys.UserId, user.Id);
        verificationUri = QueryHelpers.AddQueryString(verificationUri, QueryStringKeys.Code, otp.SixDigitCode);
        verificationUri = QueryHelpers.AddQueryString(verificationUri, MultitenancyConstants.TenantIdName, _currentTenant.Id!);
        return otp.SixDigitCode;
    }

    public async Task<string> ConfirmEmailAsync(string userId, string code, CancellationToken cancellationToken)
    {
        /*EnsureValidTenant();

        var user = await _userManager.Users
            .Where(u => u.Id == userId && !u.EmailConfirmed)
            .FirstOrDefaultAsync(cancellationToken);
        var otp = await _db.OtpCodes.FirstOrDefaultAsync(x => x.UserId == new Guid( userId ) && x.SixDigitCode == code);
        _ = user ?? throw new InternalServerException(_t["An error occurred while confirming E-Mail."]);

        var result = await _userManager.ConfirmEmailAsync(user, otp.Code);

        return result.Succeeded
            ? string.Format(_t["Account Confirmed for E-Mail {0}. You can now use the /api/tokens endpoint to generate JWT."], user.Email)
            : throw new InternalServerException(string.Format(_t["An error occurred while confirming {0}"], user.Email));*/
        EnsureValidTenant();

        var user = await _userManager.Users
            .Where(u => u.Id == userId && !u.EmailConfirmed)
            .FirstOrDefaultAsync(cancellationToken);

        _ = user ?? throw new InternalServerException(_t["An error occurred while confirming E-Mail."]);
        var otp = await _db.OtpCodes.FirstOrDefaultAsync(x => x.UserId == new Guid(userId) && x.SixDigitCode == code);



        code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(otp.Code));
        var result = await _userManager.ConfirmEmailAsync(user, code);

        return result.Succeeded
            ? string.Format(_t["Account Confirmed for E-Mail {0}. You can now use the /api/tokens endpoint to generate JWT."], user.Email)
            : throw new InternalServerException(string.Format(_t["An error occurred while confirming {0}"], user.Email));
    }

    public async Task<string> ConfirmPhoneNumberAsync(string userId, string code)
    {
        EnsureValidTenant();

        var user = await _userManager.FindByIdAsync(userId);

        _ = user ?? throw new InternalServerException(_t["An error occurred while confirming Mobile Phone."]);
        if (string.IsNullOrEmpty(user.PhoneNumber)) throw new InternalServerException(_t["An error occurred while confirming Mobile Phone."]);

        var result = await _userManager.ChangePhoneNumberAsync(user, user.PhoneNumber, code);

        return result.Succeeded
            ? user.PhoneNumberConfirmed
                ? string.Format(_t["Account Confirmed for Phone Number {0}. You can now use the /api/tokens endpoint to generate JWT."], user.PhoneNumber)
                : string.Format(_t["Account Confirmed for Phone Number {0}. You should confirm your E-mail before using the /api/tokens endpoint to generate JWT."], user.PhoneNumber)
            : throw new InternalServerException(string.Format(_t["An error occurred while confirming {0}"], user.PhoneNumber));
    }
}
