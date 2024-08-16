using DocumentFormat.OpenXml.Office2010.Excel;
using EMS20.WebApi.Application.Identity.Roles;
using EMS20.WebApi.Application.Identity.Tokens;
using EMS20.WebApi.Application.Identity.Users;
using EMS20.WebApi.Host.Controllers;
using EMS20.WebApi.Shared.Authorization;
using System.Threading;

namespace EMS20.WebApi.Host.Controllers.Identity;

public class RolesController : VersionedApiController
{
    private readonly IRoleService _roleService;
    private readonly IUserService _userService;

    public RolesController(IRoleService roleService, IUserService userService) => (_roleService, _userService) = (roleService, userService);

    [HttpGet("roles.getlistofrolesasync")]
    [MustHavePermission(FSHAction.View, FSHResource.Roles)]
    [OpenApiOperation("Get a list of all roles.", "")]
    public async Task<ResultResponse<List<RoleDto>>> GetListAsync(CancellationToken cancellationToken)
    {
        return new ResultResponse<List<RoleDto>>
        {
            Data = await _roleService.GetListAsync(cancellationToken),
            Message = "Request Executed Successfully",
            Success = true
        };
    }

    [HttpGet("roles.getrolebyidasync")]
    [AllowAnonymous]
    [MustHavePermission(FSHAction.View, FSHResource.Roles)]
    [OpenApiOperation("Get role details.", "")]
    public async Task<ResultResponse<RoleDto>> GetByIdAsync(string id)
    {
        return new ResultResponse<RoleDto>
        {
            Data = await _roleService.GetByIdAsync(id),
            Message = "Request Executed Successfully",
            Success = true
        };
    }

    [HttpGet("roles.getlistofpermissionsbyroleid")]
    [MustHavePermission(FSHAction.View, FSHResource.RoleClaims)]
    [OpenApiOperation("Get role details with its permissions.", "")]
    public async Task<ResultResponse<RoleDto>> GetByIdWithPermissionsAsync(string id, CancellationToken cancellationToken)
    {
        return new ResultResponse<RoleDto>
        {
            Data = await _roleService.GetByIdWithPermissionsAsync(id, cancellationToken),
            Message = "Request Executed Successfully",
            Success = true
        };
    }

    [HttpPost("roles.createpermissionsbyroleidasync")]
    [MustHavePermission(FSHAction.Create, FSHResource.RoleClaims)]
    [OpenApiOperation("Create new role's permissions.", "")]
    public async Task<ActionResult<string>> CreatePermissionsAsync(string id, CreateRolePermissionsRequest request, CancellationToken cancellationToken)
    {
        if (id != request.RoleId)
        {
            return BadRequest();
        }
        return Ok(await _roleService.CreatePermissionsAsync(request, cancellationToken));
    }

    [HttpPut("roles.updatepermissionsbyroleidasync")]
    [MustHavePermission(FSHAction.Update, FSHResource.RoleClaims)]
    [OpenApiOperation("Update a role's permissions.", "")]
    public async Task<ActionResult<string>> UpdatePermissionsAsync(string id, UpdateRolePermissionsRequest request, CancellationToken cancellationToken)
    {
        if (id != request.RoleId)
        {
            return BadRequest();
        }
        return Ok(await _roleService.UpdatePermissionsAsync(request, cancellationToken));
    }

    [HttpDelete("roles.deletepermissionsbyroleidasync")]
    [MustHavePermission(FSHAction.Delete, FSHResource.RoleClaims)]
    [OpenApiOperation("delete a role's permissions.", "")]
    public async Task<ActionResult<string>> DeletePermissionsAsync([FromQuery]string id, DeleteRolePermissionsRequest request, CancellationToken cancellationToken)
    {
        if (id != request.RoleId)
        {
            return BadRequest();
        }
        return Ok(await _roleService.DeletePermissionsAsync(request, cancellationToken));
    }

    [HttpPost("roles.createroleasync")]
    [MustHavePermission(FSHAction.Create, FSHResource.Roles)]
    [OpenApiOperation("Create or update a role.", "")]
    public async Task<ResultResponse<string>> RegisterRoleAsync(CreateOrUpdateRoleRequest request)
    {
        return new ResultResponse<string>
        {
            Data = await _roleService.CreateOrUpdateAsync(request),
            Message = "Request Executed Successfully",
            Success = true
        };
    }

    [HttpDelete("roles.deleteroleasync")]
    [MustHavePermission(FSHAction.Delete, FSHResource.Roles)]
    [OpenApiOperation("Delete a role.", "")]
    public async Task<ResultResponse<string>> DeleteAsync(string id)
    {
        return new ResultResponse<string>
        {
            Data = await _roleService.DeleteAsync(id),
            Message = "Request Executed Successfully",
            Success = true
        };
    }

    [HttpGet("roles.getlistofusersbyrole")]
    [OpenApiOperation("Get list of users associated with the role.", "")]
    public async Task<ResultResponse<Dictionary<string, List<UserDetailsWithoutRoleDto>>>> getlistofusersbyrole([FromQuery]List<Guid> ids, CancellationToken cancellationToken)
    {
        return await _userService.ListOfUsersByRole(ids, cancellationToken);
    }
}