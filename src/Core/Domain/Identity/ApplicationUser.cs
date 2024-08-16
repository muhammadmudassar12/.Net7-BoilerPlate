using EMS20.WebApi.Domain.Common.Contracts;
using Microsoft.AspNetCore.Identity;

namespace EMS20.WebApi.Domain.Identity;

public class ApplicationUser : IdentityUser, IAuditableEntity, ISoftDelete
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? ImageUrl { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
    public string? ObjectId { get; set; }
    public bool IsActive { get; set; }
    public DefaultIdType? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public DefaultIdType? LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }
    public DateTime? DeletedOn { get; set; }
    public DefaultIdType? DeletedBy { get; set; }
}