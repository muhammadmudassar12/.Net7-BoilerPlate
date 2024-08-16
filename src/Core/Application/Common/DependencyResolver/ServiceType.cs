namespace EMS20.WebApi.Application.Common.DependencyResolver;

public static class ServiceType
{
    public interface IScoped { }
    public interface ISingleton { }
    public interface ITransient { }
}