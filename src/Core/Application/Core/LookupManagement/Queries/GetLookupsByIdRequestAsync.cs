using AutoMapper;
using EMS20.WebApi.Application.Core.LookupManagement.ResponseDto;
using EMS20.WebApi.Application.Resources.ResFile;
using EMS20.WebApi.Domain.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS20.WebApi.Application.Core.LookupManagement.Queries;
public class GetLookupsByIdRequestAsync : IRequest<ResultResponse<LookupManagementDto>>
{
    public Guid Id { get; set; }
}
public class GetLookupsByIdRequestAsyncSpec : Specification<Domain.Core.LookupManagement , LookupManagementDto>, ISingleResultSpecification
{
    public GetLookupsByIdRequestAsyncSpec(DefaultIdType Id) =>
        Query
        .Where(x=>x.Id == Id);
}
public class GetLookupsByIdRequestHandlerAsync : IRequestHandler<GetLookupsByIdRequestAsync, ResultResponse<LookupManagementDto>>
{
    private readonly IRepositoryWithEvents<Domain.Core.LookupManagement> _repository;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer<langResApplication> _lang;

    public GetLookupsByIdRequestHandlerAsync(IRepositoryWithEvents<Domain.Core.LookupManagement> repository, IMapper mapper, IStringLocalizer<langResApplication> lang)
    {
        _repository = repository;
        _mapper = mapper;
        _lang = lang;
    }
    public async Task<ResultResponse<LookupManagementDto>> Handle(GetLookupsByIdRequestAsync request, CancellationToken cancellationToken)
    {
        var lookups = await _repository.GetByIdAsync(request.Id);
        var lookupsMapping = _mapper.Map<LookupManagementDto>(lookups);
        var response = new ResultResponse<LookupManagementDto> { Data = lookupsMapping, Message = _lang["Success"].Value, Success = true };
        return response;
    }
}
