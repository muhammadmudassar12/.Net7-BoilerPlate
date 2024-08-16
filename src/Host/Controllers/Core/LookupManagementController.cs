using EMS20.WebApi.Application.Core.LookupManagement.Commands;
using EMS20.WebApi.Application.Core.LookupManagement.Queries;
using EMS20.WebApi.Application.Core.LookupManagement.ResponseDto;
using Microsoft.AspNetCore.Mvc;

namespace EMS20.WebApi.Host.Controllers.Core;
public class LookupManagementController : VersionedApiController
{
    [HttpPost("LookupManagement.CreateLookupManagementRequestAsync")]
    [MustHavePermission(FSHAction.Create, FSHResource.LookupManagement)]
    [OpenApiOperation("Create Lookup Management")]
    public async Task<ResultResponse<DefaultIdType>> CreateLookupManagementRequestAsync(CreateLookupManagementRequestAsync request)
    {
        return await Mediator.Send(request);
    }

    [HttpPut("LookupManagement.UpdateLookupManagementRequestAsync")]
    [MustHavePermission(FSHAction.Update, FSHResource.LookupManagement)]
    [OpenApiOperation("Update Lookup Management")]
    public async Task<ResultResponse<DefaultIdType>> UpdateLookupManagementRequestAsync(UpdateLookupManagementRequestAsync request)
    {
        return await Mediator.Send(request);
    }

    [HttpDelete("LookupManagement.DeleteLookupManagementRequestAsync")]
    [MustHavePermission(FSHAction.Delete, FSHResource.LookupManagement)]
    [OpenApiOperation("Delete Lookup Management")]
    public async Task<ResultResponse<DefaultIdType>> DeleteLookupManagementRequestAsync([FromQuery] DeleteLookupManagementRequestAsync request)
    {
        return await Mediator.Send(request);
    }

    [HttpGet("LookupManagement.GetAllLookupManagementRequestAsync")]
    [MustHavePermission(FSHAction.View, FSHResource.LookupManagement)]
    [OpenApiOperation("Get Lookup Management")]
    public async Task<ResultResponse<List<LookupManagementDto>>> GetAllLookupsRequestAsync([FromQuery]GetAllLookupsRequestAsync request)
    {
        return await Mediator.Send(request);
    }

    [HttpGet("LookupManagement.GetLookupManagementByIdRequestAsync")]
      [MustHavePermission(FSHAction.View, FSHResource.LookupManagement)]
    [OpenApiOperation("Get Lookup Management")]
    public async Task<ResultResponse<LookupManagementDto>> GetLookupsByIdRequestAsync([FromQuery] GetLookupsByIdRequestAsync request)
    {
        return await Mediator.Send(request);
    }
}
