using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS20.WebApi.Application.Identity.Roles;
public class CreateRolePermissionsRequest
{
    public string RoleId { get; set; } = default!;
    public List<string> Permissions { get; set; } = default!;
}

public class CreateRolePermissionsRequestValidator : CustomValidator<CreateRolePermissionsRequest>
{
    public CreateRolePermissionsRequestValidator()
    {
        RuleFor(r => r.RoleId)
            .NotEmpty();
        RuleFor(r => r.Permissions)
            .NotNull();
    }
}
