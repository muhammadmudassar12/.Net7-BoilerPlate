using EMS20.WebApi.Application.Common.Persistence;
using EMS20.WebApi.Application.System.Settings.Queries.GetSettings;
using EMS20.WebApi.Application.System.Settings.TimeZoneSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS20.WebApi.Infrastructure.Settings;
public class TimeZoneService : ITimeZoneService
{
    private readonly IRepository<Domain.System.Settings> _settingsRepository;
    public TimeZoneService(IRepository<Domain.System.Settings> settingsRepository)
    {
        _settingsRepository = settingsRepository;
    }

    public async Task<int> GetTimeZoneOffsetAsync(CancellationToken cancellationToken)
    {
        var settingsSpec = new GetSettingsByGroupAsyncSpec("Company Info");
        var settings = await _settingsRepository.ListAsync(settingsSpec, cancellationToken);
        var setting = settings.FirstOrDefault();

        if (setting == null)
        {
            throw new Exception("Settings not found.");
        }

        var timeZone = setting.TimeZone;
        var offset = TimeZoneChanger.GetOffsetFromTimeZone(timeZone);
        return offset;
    }
}

