using EMS20.WebApi.Application.Common.Interfaces;

namespace EMS20.WebApi.Application.Common.Mailing;

public interface IMailService : ITransientService
{
    Task SendAsync(MailRequest request, CancellationToken ct);
}