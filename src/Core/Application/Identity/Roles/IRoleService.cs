using EMS20.WebApi.Application.Common.Interfaces;

namespace EMS20.WebApi.Application.Identity.Roles;

public interface IRoleService : ITransientService
{
    Task<List<RoleDto>> GetListAsync(CancellationToken cancellationToken);

    Task<int> GetCountAsync(CancellationToken cancellationToken);

    Task<bool> ExistsAsync(string roleName, string? excludeId);

    Task<RoleDto> GetByIdAsync(string id);

    Task<RoleDto> GetByIdWithPermissionsAsync(string roleId, CancellationToken cancellationToken);

    Task<string> CreateOrUpdateAsync(CreateOrUpdateRoleRequest request);

    Task<string> CreatePermissionsAsync(CreateRolePermissionsRequest request, CancellationToken cancellationToken);
    Task<string> UpdatePermissionsAsync(UpdateRolePermissionsRequest request, CancellationToken cancellationToken);
    Task<string> DeletePermissionsAsync(DeleteRolePermissionsRequest request, CancellationToken cancellationToken);

    Task<string> DeleteAsync(string id);
}