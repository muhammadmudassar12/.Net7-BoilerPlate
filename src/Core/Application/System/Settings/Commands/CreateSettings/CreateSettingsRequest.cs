using EMS20.WebApi.Application.Resources.ResFile;

namespace EMS20.WebApi.Application.System.Settings.Commands.CreateSettings;
public class CreateSettingsRequest : IRequest<ResultResponse<DefaultIdType>>
{
    public string? Group { get;  set; }
    public string? Name { get; set; }
    public bool Locked { get; set; }
    public string? Payload { get; set; }
}

public class CreateSettingsRequestValidator : CustomValidator<CreateSettingsRequest>
{
    public CreateSettingsRequestValidator(IReadRepository<Domain.System.Settings> repository, IStringLocalizer<CreateSettingsRequestValidator> T) =>
        RuleFor(p => p.Name)
            .NotEmpty()
            .MaximumLength(75)
            .MustAsync(async (name, ct) => await repository.FirstOrDefaultAsync(new SettingsByNameSpec(name), ct) is null)
                .WithMessage((_, name) => T["Settings {0} already Exists.", name]);
}

public class CreateSettingsRequestHandler : IRequestHandler<CreateSettingsRequest,  ResultResponse<DefaultIdType>>
{
    private readonly IRepositoryWithEvents<Domain.System.Settings> _repository;
    public CreateSettingsRequestHandler(IRepositoryWithEvents<Domain.System.Settings> repository) => _repository = repository;
    public async Task<ResultResponse<DefaultIdType>> Handle(CreateSettingsRequest request, CancellationToken cancellationToken)
    {
        var settings = new Domain.System.Settings(request?.Group, request.Name, request.Locked, request.Payload);
        await _repository.AddAsync(settings, cancellationToken);
        return new ResultResponse<DefaultIdType>
        {
            Data = settings.Id,
            Success = true,
            Message = "Settings Created Successfully"
        };
    }
}
