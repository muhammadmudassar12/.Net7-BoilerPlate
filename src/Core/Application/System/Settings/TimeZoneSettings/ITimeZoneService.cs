using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS20.WebApi.Application.System.Settings.TimeZoneSettings;
public interface ITimeZoneService
{
    Task<int> GetTimeZoneOffsetAsync(CancellationToken cancellationToken);
}
