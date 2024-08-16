using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS20.WebApi.Application.Common.Specification.System;
public class GetSubmissionSettingsByGroupAsyncSpec : Specification<Domain.System.Settings, Domain.System.Settings>, ISingleResultSpecification
{
    public GetSubmissionSettingsByGroupAsyncSpec(string name) =>
        Query
        .Where(x => x.Group.ToLower() == name.ToLower());
}
