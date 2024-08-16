using EMS20.WebApi.Application.Resources.ResFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS20.WebApi.Application.System.Settings.Commands.CreateSettings;
public class UpdateSettingsRequest : IRequest<ResultResponse<DefaultIdType>>
{
    public Guid? Id { get; set; }
    public string? Group { get; set; }
    public string? Name { get; set; }
    public bool Locked { get; set; }
    public string? Payload { get; set; }
}

public class UpdateSettingsRequestHandler : IRequestHandler<UpdateSettingsRequest,ResultResponse<DefaultIdType>>
{
    private readonly IRepositoryWithEvents<Domain.System.Settings> _repository;
    private readonly IStringLocalizer<langResApplication> _lang;

    public UpdateSettingsRequestHandler(IRepositoryWithEvents<Domain.System.Settings> repository) => _repository = repository;
    public async Task<ResultResponse<DefaultIdType>> Handle(UpdateSettingsRequest request, CancellationToken cancellationToken)
    {
        var settings = await _repository.GetByIdAsync(request.Id);
        _ = settings ?? throw new NotFoundException(_lang["Settings Not Found."]);
        settings.Update(request.Group, request.Name, request.Locked, request.Payload);
        await _repository.UpdateAsync(settings, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
        return new ResultResponse<DefaultIdType>
        {
            Data = settings.Id,
            Success = true,
            Message = "Settings Updated Successfully"
        };
    }
}
