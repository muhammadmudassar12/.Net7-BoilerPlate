using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS20.WebApi.Domain.Common;
public class GeoCoordinate
{
    public GeoCoordinate(double? latitude, double? longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }

    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}
