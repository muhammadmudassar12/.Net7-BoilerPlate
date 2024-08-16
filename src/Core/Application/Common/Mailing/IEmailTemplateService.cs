using EMS20.WebApi.Application.Common.Interfaces;

namespace EMS20.WebApi.Application.Common.Mailing;

public interface IEmailTemplateService : ITransientService
{
    string GenerateEmailTemplate<T>(string templateName, T mailTemplateModel);
}