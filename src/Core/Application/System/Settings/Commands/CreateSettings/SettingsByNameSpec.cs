using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS20.WebApi.Application.System.Settings.Commands.CreateSettings;
public class SettingsByNameSpec : Specification<Domain.System.Settings>, ISingleResultSpecification
{
    public SettingsByNameSpec(string name) =>
        Query.Where(b => b.Name == name);
}