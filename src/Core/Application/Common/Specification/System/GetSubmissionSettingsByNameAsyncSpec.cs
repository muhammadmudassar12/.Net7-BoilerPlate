using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS20.WebApi.Application.Common.Specification.System;

public class GetSubmissionSettingsByNameAsyncSpec : Specification<Domain.System.Settings, Domain.System.Settings>, ISingleResultSpecification
{
    public GetSubmissionSettingsByNameAsyncSpec(string name) =>
        Query
        .Where(x => x.Name.ToLower() == name.ToLower());
}
