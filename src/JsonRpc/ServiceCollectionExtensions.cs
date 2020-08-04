using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MediatR;
using MediatR.Pipeline;
using MediatR.Registration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OmniSharp.Extensions.JsonRpc.Pipelines;

namespace OmniSharp.Extensions.JsonRpc
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddJsonRpcMediatR(this IServiceCollection services, IEnumerable<Assembly> assemblies = null)
        {
            ServiceRegistrar.AddRequiredServices(services, new MediatRServiceConfiguration());
            ServiceRegistrar.AddMediatRClasses(services, assemblies ?? Enumerable.Empty<Assembly>());
            services.AddTransient(typeof(IRequestPreProcessor<>), typeof(RequestMustNotBeNullProcessor<>));
            services.AddTransient(typeof(IRequestPostProcessor<,>), typeof(ResponseMustNotBeNullProcessor<,>));
            services.AddScoped<IRequestContext, RequestContext>();
            services.RemoveAll<ServiceFactory>();
            services.AddScoped<ServiceFactory>(serviceProvider => { return serviceType => GetHandler(serviceProvider, serviceType); });
            return services;
        }

        private static object GetHandler(IServiceProvider serviceProvider, Type serviceType)
        {
            if (serviceType.IsGenericType &&
                typeof(IRequestHandler<,>).IsAssignableFrom(serviceType.GetGenericTypeDefinition()))
            {
                var context = serviceProvider.GetService<IRequestContext>();
                return context.Descriptor != null ? context.Descriptor.Handler : serviceProvider.GetService(serviceType);
            }

            return serviceProvider.GetService(serviceType);
        }
    }
}
