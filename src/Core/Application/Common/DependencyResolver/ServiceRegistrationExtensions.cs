using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EMS20.WebApi.Application.Common.DependencyResolver
{
    public static class ServiceRegistrationExtensions
    {
        public static void RegisterHandlers(this IServiceCollection services, Assembly assembly)
        {
            var requestHandlerType = typeof(IRequestHandler<,>);
            var requestHandlers = assembly.GetTypes()
                .Where(type => type.GetInterfaces()
                    .Any(interfaceType => interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == requestHandlerType));

            foreach (var handler in requestHandlers)
            {
                var interfaces = handler.GetInterfaces();
                var genericHandler = interfaces.First(interfaceType => interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == requestHandlerType);

                services.AddTransient(genericHandler, handler);
            }
        }
    }
}
