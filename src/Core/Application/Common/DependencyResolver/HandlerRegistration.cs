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
    public static class HandlerRegistration
    {
        public static void RegisterHandlers(IServiceCollection services)
        {
            // Find all the IRequestHandler types in the assembly
            var requestHandlerTypes = Assembly.GetExecutingAssembly()
                                             .GetTypes()
                                             .Where(type => type.GetInterfaces().Any(IsRequestHandlerInterface))
                                             .ToList();

            // Register each handler type with its corresponding interface
            foreach (var handlerType in requestHandlerTypes)
            {
                var interfaceType = handlerType.GetInterfaces().FirstOrDefault(IsRequestHandlerInterface);
                if (interfaceType != null)
                {
                    services.AddTransient(interfaceType, handlerType);
                }
            }
        }

        private static bool IsRequestHandlerInterface(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IRequestHandler<,>);
        }
    }
}
