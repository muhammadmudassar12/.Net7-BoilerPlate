using EMS20.WebApi.Shared.Multitenancy;

namespace EMS20.WebApi.Infrastructure.OpenApi
{
    public class TenantIdHeaderAttribute : SwaggerHeaderAttribute
    {
        public TenantIdHeaderAttribute()
            : base(
                MultitenancyConstants.TenantIdName,
                "Input your tenant Id to access this API",
                "root", // Default value
                true)
        {
        }
    }

}
