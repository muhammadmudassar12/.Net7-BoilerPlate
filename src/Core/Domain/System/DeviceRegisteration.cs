using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS20.WebApi.Domain.System;
public class DeviceRegisteration : AuditableEntity,IAggregateRoot
{
    public string? DeviceId { get; set; }
    public string? DeviceName { get; set; }
    public string? Imei { get; set; }
    public string? DeviceType { get; set; }
    public DefaultIdType? UserId { get; set; }
    public bool? IsLoggedIn { get; set; }
}
