using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS20.WebApi.Application.Identity.Roles;
public class DeleteRolePermissionsRequest
{
    public string RoleId { get; set; } = default!;
    public List<int> PermissionIds { get; set; } = default!;
}

public class DeleteRolePermissionsRequestValidator : CustomValidator<DeleteRolePermissionsRequest>
{
    public DeleteRolePermissionsRequestValidator()
    {
        RuleFor(r => r.RoleId)
            .NotEmpty();
        RuleFor(r => r.PermissionIds)
            .NotNull();
    }
}
