using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS20.WebApi.Domain.Common;
public class CommonRequest
{
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? Imei { get; set; }
    public string? Os { get; set; }
    public string? DeviceName { get; set; }
    public string? ApplicationVersion { get; set; }
}
