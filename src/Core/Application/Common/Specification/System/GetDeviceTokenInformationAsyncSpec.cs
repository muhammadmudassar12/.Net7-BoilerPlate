using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS20.WebApi.Application.Common.Specification.System;
public class GetDeviceTokenInformationAsyncSpec : Specification<Domain.System.DeviceRegisteration, Domain.System.DeviceRegisteration>, ISingleResultSpecification
{
    public GetDeviceTokenInformationAsyncSpec(List<DefaultIdType>? userIds) =>
        Query
        .Where(x => userIds.Contains((Guid)x.UserId));
}
