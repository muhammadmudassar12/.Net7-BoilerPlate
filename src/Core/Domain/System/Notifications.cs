using EMS20.WebApi.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS20.WebApi.Domain.System;
public class Notifications : AuditableEntity,IAggregateRoot
{
    public string? Message { get; set; }
    public DefaultIdType? ToUserId { get; set; }
    public virtual ApplicationUser? ToUser { get; set; }
    public string? Title { get; set; }
    public bool? IsRead { get; set; }
    public string? ReferenceTable { get; set; }
    public DefaultIdType? ReferenceId { get; set; }
}
