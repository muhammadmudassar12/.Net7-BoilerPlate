using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS20.WebApi.Application.Identity.Users.Password;
public class OtpVerificationRequest
{
    public string? Otp { get; set; }
    public string? Email { get; set; }
}

public class OtpVerificationResponse
{
    public string? Token { get; set; }
    public string? Email { get; set; }
}

