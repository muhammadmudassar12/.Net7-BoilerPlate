using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS20.WebApi.Application.Identity.Users.Password;
public class ForgotPasswordResponse
{
    public string Email { get; set; } = default!;
    public string Code { get; set; } = default!;
}
