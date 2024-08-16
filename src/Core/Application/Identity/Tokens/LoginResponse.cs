using EMS20.WebApi.Application.Identity.Roles;
using EMS20.WebApi.Application.Identity.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS20.WebApi.Application.Identity.Tokens;
public class LoginResponse
{
    public UserDetailsWithoutRoleDto? user { get; set; }
    public IList<string>? roles { get; set; }
    public List<string>? permissions { get; set; }
    public TokenResponse? token { get; set; }

}
