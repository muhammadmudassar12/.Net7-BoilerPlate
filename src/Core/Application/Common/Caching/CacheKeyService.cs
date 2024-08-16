using EMS20.WebApi.Application.Common.Interfaces;

namespace EMS20.WebApi.Application.Common.Caching;

public interface ICacheKeyService : IScopedService
{
    public string GetCacheKey(string name, object id, bool includeTenantId = true);
}