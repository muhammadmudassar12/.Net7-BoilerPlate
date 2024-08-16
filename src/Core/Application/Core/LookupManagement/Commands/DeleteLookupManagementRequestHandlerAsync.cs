using AutoMapper;
using EMS20.WebApi.Application.Resources.ResFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS20.WebApi.Application.Core.LookupManagement.Commands;
public class DeleteLookupManagementRequestAsync : IRequest<ResultResponse<DefaultIdType>>
{
    public Guid Id { get; set; }
}
public class DeleteLookupManagementRequestHandlerAsync : IRequestHandler<DeleteLookupManagementRequestAsync, ResultResponse<DefaultIdType>>
{
    private readonly IRepositoryWithEvents<Domain.Core.LookupManagement> _repository;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer<langResApplication> _lang;

    public DeleteLookupManagementRequestHandlerAsync(IRepositoryWithEvents<Domain.Core.LookupManagement> repository, IMapper mapper, IStringLocalizer<langResApplication> lang)
    {
        _repository = repository;
        _mapper = mapper;
        _lang = lang;
    }
    public async Task<ResultResponse<DefaultIdType>> Handle(DeleteLookupManagementRequestAsync request, CancellationToken cancellationToken)
    {
        var lookupManagement = await _repository.GetByIdAsync(request.Id);
        if (lookupManagement == null)
        {
            throw new ArgumentException("Lookup not found");
        }
        await _repository.DeleteAsync(lookupManagement, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return new ResultResponse<DefaultIdType>
        {
            Data = request.Id,
            Message = _lang["Success"].Value,
            Success = true
        };
    }

}