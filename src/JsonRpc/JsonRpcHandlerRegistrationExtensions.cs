using System;
using Microsoft.Extensions.DependencyInjection;

namespace OmniSharp.Extensions.JsonRpc
{
    public static class JsonRpcHandlerRegistrationExtensions
    {
        public static IServiceCollection AddJsonRpcHandler(this IServiceCollection services, string method, IJsonRpcHandler handler, JsonRpcHandlerOptions? options = null)
        {
            services.AddSingleton(JsonRpcHandlerDescription.Named(method, handler, options));
            return services;
        }

        public static IServiceCollection AddJsonRpcHandler(this IServiceCollection services, string method, JsonRpcHandlerFactory handlerFunc, JsonRpcHandlerOptions? options = null)
        {
            services.AddSingleton(JsonRpcHandlerDescription.Named(method, handlerFunc, options));
            return services;
        }

        public static IServiceCollection AddJsonRpcHandler(this IServiceCollection services, IJsonRpcHandler handler, JsonRpcHandlerOptions? options = null)
        {
            services.AddSingleton(JsonRpcHandlerDescription.Infer(handler, options));
            return services;
        }

        public static IServiceCollection AddJsonRpcHandler(this IServiceCollection services, JsonRpcHandlerFactory handlerFunc, JsonRpcHandlerOptions? options = null)
        {
            services.AddSingleton(JsonRpcHandlerDescription.Infer(handlerFunc, options));
            return services;
        }

        public static IServiceCollection AddJsonRpcHandler(this IServiceCollection services, params IJsonRpcHandler[] handlers)
        {
            foreach (var item in handlers)
            {
                services.AddSingleton(JsonRpcHandlerDescription.Infer(item));
            }

            return services;
        }

        public static IServiceCollection AddJsonRpcHandler<THandler>(this IServiceCollection services, JsonRpcHandlerOptions? options = null) where THandler : IJsonRpcHandler =>
            AddJsonRpcHandler(services, typeof(THandler), options);

        public static IServiceCollection AddJsonRpcHandler<THandler>(this IServiceCollection services, string method, JsonRpcHandlerOptions? options = null)
            where THandler : IJsonRpcHandler => AddJsonRpcHandler(services, method, typeof(THandler), options);

        public static IServiceCollection AddJsonRpcHandler(this IServiceCollection services, Type type, JsonRpcHandlerOptions? options = null)
        {
            services.AddSingleton(JsonRpcHandlerDescription.Infer(type, options));
            return services;
        }

        public static IServiceCollection AddJsonRpcHandler(this IServiceCollection services, string method, Type type, JsonRpcHandlerOptions? options = null)
        {
            services.AddSingleton(JsonRpcHandlerDescription.Named(method, type, options));
            return services;
        }
    }
}
