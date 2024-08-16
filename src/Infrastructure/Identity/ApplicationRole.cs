using EMS20.WebApi.Domain.Common.Contracts;
using Microsoft.AspNetCore.Identity;

namespace EMS20.WebApi.Infrastructure.Identity;

public class ApplicationRole : IdentityRole, IAuditableEntity, ISoftDelete
{
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DefaultIdType? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public DefaultIdType? LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }
    public DateTime? DeletedOn { get; set; }
    public DefaultIdType? DeletedBy { get; set; }

    public ApplicationRole(string name, string? description = null, bool isActive =true )
        : base(name)
    {
        Description = description;
        NormalizedName = name.ToUpperInvariant();
        IsActive = isActive;
    }
}
