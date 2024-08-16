
using EMS20.WebApi.Application.Resources.ResFile;
using EMS20.WebApi.Domain.Enumeration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace EMS20.WebApi.Application.Common.Auxillary;
public static class LangService
{
    static ResourceManager rm;
    static CultureInfo c;
    static LangService()
    {
        rm = new ResourceManager("EMS20.WebApi.src.Application.Resources.langResApplication", typeof(langResApplication).Assembly);
    }

    public static string GetString(string key, string AppLanguageEnum, params string[] param)
    {
        if (string.IsNullOrEmpty(key))
            return key;

        if (string.IsNullOrEmpty(AppLanguageEnum))
            AppLanguageEnum = AppLanguages.ar_SA;

        if (AppLanguageEnum == AppLanguages.ar_SA)
        {
            c = new CultureInfo("ar-SA");
        }
        else if (AppLanguageEnum == AppLanguages.en_US)
        {
            c = new CultureInfo("en-US");
        }

        string str = rm.GetString(key, c);

        if (param != null && param.Length > 0)
        {
            return string.Format(str, param);
        }
        else
        {
            return str;
        }
    }
}
