using AutoMapper;
using EMS20.WebApi.Application.Core.LookupManagement.ResponseDto;
using EMS20.WebApi.Application.Resources.ResFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS20.WebApi.Application.Core.LookupManagement.Queries;
public class GetAllLookupsRequestAsync : ListRequest, IRequest<ResultResponse<List<LookupManagementDto>>>
{
}
public class GetAllLookupsRequestHandlerAsync : IRequestHandler<GetAllLookupsRequestAsync, ResultResponse<List<LookupManagementDto>>>
{
    private readonly IRepositoryWithEvents<Domain.Core.LookupManagement> _repository;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer<langResApplication> _lang;

    public GetAllLookupsRequestHandlerAsync(IRepositoryWithEvents<Domain.Core.LookupManagement> repository, IMapper mapper, IStringLocalizer<langResApplication> lang)
    {
        _repository = repository;
        _mapper = mapper;
        _lang = lang;
    }
    public async Task<ResultResponse<List<LookupManagementDto>>> Handle(GetAllLookupsRequestAsync request, CancellationToken cancellationToken)
    {
        var lookups = await _repository.ListAsync(cancellationToken);
        var lookupsMapping = _mapper.Map<List<LookupManagementDto>>(lookups);
        var response = new ResultResponse<List<LookupManagementDto>> { Data = lookupsMapping, Message = _lang["Success"].Value, Success = true };
        return response;
    }
}
