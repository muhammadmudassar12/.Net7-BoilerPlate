using Microsoft.Extensions.DependencyInjection;

namespace EMS20.WebApi.Application.Common.DependencyResolver;

internal static class ConfigureServicesExtension
{
    internal static void AutoDependenciesResolver(this IServiceCollection services)
    {
        // Configure Services
        DependenciesScanner(services, typeof(ServiceType.ISingleton), ServiceLifetime.Singleton);
        DependenciesScanner(services, typeof(ServiceType.IScoped), ServiceLifetime.Scoped);
        DependenciesScanner(services, typeof(ServiceType.ITransient), ServiceLifetime.Transient);
    }
    internal static IServiceCollection DependenciesScanner(IServiceCollection services, Type interfaceType, ServiceLifetime servicelifetime)
    {
        try
        {
            var interfaceTypes =
            AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(t => interfaceType.IsAssignableFrom(t)
                            && t.IsClass && !t.IsAbstract)
                .Select(t => new
                {
                    Service = t.GetInterfaces().FirstOrDefault(),
                    Implementation = t
                })
                .Where(t => t.Service is not null
                            && interfaceType.IsAssignableFrom(t.Service));

            foreach (var type in interfaceTypes)
            {
                RegisterDependency(services, type.Service!, type.Implementation, servicelifetime);
            }
        }
        catch (Exception)
        {

        }


        return services;
    }

    internal static IServiceCollection RegisterDependency(IServiceCollection services, Type serviceType, Type implementationType, ServiceLifetime serviceLifetime)
    {
        return serviceLifetime switch
        {
            ServiceLifetime.Transient => services.AddTransient(serviceType, implementationType),
            ServiceLifetime.Scoped => services.AddScoped(serviceType, implementationType),
            ServiceLifetime.Singleton => services.AddSingleton(serviceType, implementationType),
            _ => throw new ArgumentException("Invalid lifeTime", nameof(serviceLifetime))
        };
    }
}
