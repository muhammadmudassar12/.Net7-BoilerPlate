using EMS20.WebApi.Application.Common.Interfaces;

namespace EMS20.WebApi.Application.Common.Exporters;

public interface IExcelWriter : ITransientService
{
    Stream WriteToStream<T>(IList<T> data);
}