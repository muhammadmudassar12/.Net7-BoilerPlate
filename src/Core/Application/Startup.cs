using System.Reflection;
using EMS20.WebApi.Application.Common.DependencyResolver;
using Microsoft.Extensions.DependencyInjection;

namespace EMS20.WebApi.Application;

public static class Startup
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();
        //HandlerRegistration.RegisterHandlers(services);
       
        return services

            .AddValidatorsFromAssembly(assembly)
            .AddMediatR(assembly)
            .AddAutoMapper(Assembly.GetExecutingAssembly());
    }
}