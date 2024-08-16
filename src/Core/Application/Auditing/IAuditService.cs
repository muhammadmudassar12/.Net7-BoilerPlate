using EMS20.WebApi.Application.Common.Interfaces;

namespace EMS20.WebApi.Application.Auditing;

public interface IAuditService : ITransientService
{
    Task<List<AuditDto>> GetUserTrailsAsync(DefaultIdType userId);
}