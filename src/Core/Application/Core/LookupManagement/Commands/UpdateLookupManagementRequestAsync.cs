using AutoMapper;
using EMS20.WebApi.Application.Resources.ResFile;
using EMS20.WebApi.Domain.Enumeration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS20.WebApi.Application.Core.LookupManagement.Commands;
public class UpdateLookupManagementRequestAsync : IRequest<ResultResponse<DefaultIdType>>
{
    public Guid? Id { get; set; }
    public string? Name_Eng { get; set; }
    public string? Name_Ar { get; set; }
    public string? Decscription_Eng { get; set; }
    public string? Decscription_Ar { get; set; }
    public string? Arg1 { get; set; }
    public string? Arg2 { get; set; }
    public string? Arg3 { get; set; }
    public string? Arg4 { get; set; }
    public LookupTypes? LookupTypeId { get; set; }
    public int? SortOrder { get; set; }
    public Guid? ParentId { get; set; }
}
public class UpdateLookupManagementRequestHandlerAsync : IRequestHandler<UpdateLookupManagementRequestAsync, ResultResponse<DefaultIdType>>
{
    private readonly IRepositoryWithEvents<Domain.Core.LookupManagement> _repository;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer<langResApplication> _lang;

    public UpdateLookupManagementRequestHandlerAsync(IRepositoryWithEvents<Domain.Core.LookupManagement> repository, IMapper mapper, IStringLocalizer<langResApplication> lang)
    {
        _repository = repository;
        _mapper = mapper;
        _lang = lang;
    }
    public async Task<ResultResponse<DefaultIdType>> Handle(UpdateLookupManagementRequestAsync request, CancellationToken cancellationToken)
    {
        var lookupManagement = await _repository.GetByIdAsync(request.Id);
        if (lookupManagement == null)
        {
            throw new ArgumentException("Lookup not found");
        }

        lookupManagement.Name_Eng = request.Name_Eng;
        lookupManagement.Name_Ar = request.Name_Ar;
        lookupManagement.Decscription_Eng = request.Decscription_Eng;
        lookupManagement.Decscription_Ar = request.Decscription_Ar;
        lookupManagement.Arg1 = request.Arg1;
        lookupManagement.Arg2 = request.Arg2;
        lookupManagement.Arg3 = request.Arg3;
        lookupManagement.Arg4 = request.Arg4;
        lookupManagement.LookupTypeId = request.LookupTypeId;
        lookupManagement.SortOrder = request.SortOrder;
        lookupManagement.ParentId = request.ParentId;

        await _repository.UpdateAsync(lookupManagement, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return new ResultResponse<DefaultIdType>
        {
            Data = lookupManagement.Id,
            Message = _lang["Success"].Value,
            Success = true
        };
    }
}