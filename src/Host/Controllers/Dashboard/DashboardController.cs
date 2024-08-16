//using EMS20.WebApi.Application.Dashboard;
//using EMS20.WebApi.Application.Identity.Roles;
//using EMS20.WebApi.Shared.Authorization;
//using System.Threading;

//namespace EMS20.WebApi.Host.Controllers.Dashboard;

//public class DashboardController : VersionedApiController
//{
//    [HttpGet]
//    [OpenApiOperation("Get statistics for the dashboard.", "")]
//    public async Task<ResultResponse<StatsDto>> GetAsync()
//    {
//        return new ResultResponse<StatsDto>
//        {
//            Data =await Mediator.Send(new GetStatsRequest()),
//            Message = "Request Executed Successfully",
//            Success = true
//        };
//    }
//}