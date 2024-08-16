using AutoMapper;
using EMS20.WebApi.Application.System.Settings.Commands.CreateSettings;

namespace EMS20.WebApi.Application.System.Settings.Queries.GetSettings;
public class GetSettingsRequest : IRequest<ResultResponse<List<SettingsDto>>>
{
    public string? Group { get; set; }
}

public class GetSettingsByGroupAsyncSpec : Specification<Domain.System.Settings, SettingsDto>
{
    public GetSettingsByGroupAsyncSpec(string group) =>
        Query
        .Where(x => x.Group == group);
}

public class GetSettingsHandler : IRequestHandler<GetSettingsRequest, ResultResponse<List<SettingsDto>>>
{
    private readonly IRepository<EMS20.WebApi.Domain.System.Settings> _repository;
    private readonly IStringLocalizer _t;
    private readonly IMapper _mapper;

    public GetSettingsHandler(IRepository<EMS20.WebApi.Domain.System.Settings> repository, IStringLocalizer<GetSettingsHandler> localizer, IMapper mapper) => (_repository, _t, _mapper) = (repository, localizer, mapper);

    public async Task<ResultResponse<List<SettingsDto>>> Handle(GetSettingsRequest request, CancellationToken cancellationToken)
    {
        var settings = await _repository.ListAsync(
        new GetSettingsByGroupAsyncSpec(request.Group), cancellationToken);
        var Mappedsettings = _mapper.Map<List<SettingsDto>>(settings);
        var response = new ResultResponse<List<SettingsDto>> { Data = Mappedsettings, Message = _t["Success"].Value, Success = true };
        return response;

    }
}