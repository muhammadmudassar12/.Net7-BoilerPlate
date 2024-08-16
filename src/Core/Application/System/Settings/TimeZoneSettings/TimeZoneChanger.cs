using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS20.WebApi.Application.System.Settings.TimeZoneSettings;
public static class TimeZoneChanger
{
    public static int GetOffsetFromTimeZone(string timeZone)
    {
        if (timeZone.StartsWith("(UTC"))
        {
            int endIndex = timeZone.IndexOf(')');
            if (endIndex != -1)
            {
                var offsetString = timeZone.Substring(4, endIndex - 4).Trim();
                bool negative = offsetString[0] == '-';
                string[] parts = offsetString.Substring(1).Split(':');

                if (parts.Length == 2 && int.TryParse(parts[0], out int hours) && int.TryParse(parts[1], out int minutes))
                {
                    int totalMinutes = (hours * 60) + minutes;
                    totalMinutes = negative ? -totalMinutes : totalMinutes;
                    return totalMinutes;
                }
            }
        }
        return 0;
    }
}