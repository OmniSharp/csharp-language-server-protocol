using System;
using System.Collections.Generic;
using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace OmniSharp.Extensions.JsonRpc
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddJsonRpcMediatR(this IServiceCollection services, IEnumerable<Assembly> assemblies)
        {
            services.AddMediatR(assemblies);
            services.AddScoped<IRequestContext, RequestContext>();
            services.RemoveAll<SingleInstanceFactory>();
            services.AddScoped<SingleInstanceFactory>(
                serviceProvider => {
                    return serviceType => {
                        return GetHandler(serviceProvider, serviceType);
                    };
                }
            );
            return services;
        }

        private static object GetHandler(IServiceProvider serviceProvider, Type serviceType)
        {
            var context = serviceProvider.GetService<IRequestContext>();
            return context.Descriptor.Handler;
            // return context?.Descriptor != null ? context.Descriptor.Handler : serviceProvider.GetService(serviceType);
        }
    }
}
