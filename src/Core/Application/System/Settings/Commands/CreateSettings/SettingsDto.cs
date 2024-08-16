using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS20.WebApi.Application.System.Settings.Commands.CreateSettings;
public class SettingsDto : IDto
{
    public Guid? Id { get; set; }
    public string? Group { get; set; }
    public string? Name { get; set; }
    public bool? Locked { get; set; }
    public string? Payload { get; set; }
    public string TimeZone => ParseTimeZoneFromPayload(Payload);

    private string ParseTimeZoneFromPayload(string payload)
    {
        if (string.IsNullOrEmpty(payload))
        {
            return "UTC";
        }

        if (payload.TrimStart().StartsWith("{"))
        {
            var jsonObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(payload);
            if (jsonObject != null && jsonObject.ContainsKey("timeZone"))
            {
                return jsonObject["timeZone"];
            }
        }
        return payload;
    }
}
