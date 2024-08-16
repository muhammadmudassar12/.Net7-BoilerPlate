using EMS20.WebApi.Domain.Enumeration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS20.WebApi.Domain.Core;
public class LookupManagement : AuditableEntity, IAggregateRoot
{
    public string? Name_Eng { get; set; }
    public string? Name_Ar { get; set; }
    public string? Decscription_Eng { get; set; }
    public string? Decscription_Ar { get; set; }
    public string? Arg1 { get; set; }
    public string? Arg2 { get; set; }
    public string? Arg3 { get; set; }
    public string? Arg4 { get; set; }
    public LookupTypes? LookupTypeId { get; set; }
    public int? SortOrder { get; set; }
    public Guid? ParentId { get; set; }

}
