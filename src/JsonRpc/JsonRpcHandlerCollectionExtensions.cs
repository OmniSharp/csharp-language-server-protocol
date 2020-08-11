using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace OmniSharp.Extensions.JsonRpc
{
    internal static class JsonRpcHandlerCollectionExtensions
    {
        public static void Populate(this IJsonRpcHandlerCollection collection, IServiceProvider serviceProvider, IHandlersManager handlersManager)
        {
            var links = new List<JsonRpcHandlerLinkDescription>();
            foreach (var item in collection)
            {
                switch (item)
                {
                    case JsonRpcHandlerFactoryDescription factory when string.IsNullOrWhiteSpace(factory.Method):
                        handlersManager.Add(factory.HandlerFactory(serviceProvider), factory.Options);
                        continue;
                    case JsonRpcHandlerFactoryDescription factory:
                        handlersManager.Add(factory.Method, factory.HandlerFactory(serviceProvider), factory.Options);
                        continue;
                    case JsonRpcHandlerTypeDescription type when string.IsNullOrWhiteSpace(type.Method):
                        handlersManager.Add(ActivatorUtilities.CreateInstance(serviceProvider, type.HandlerType) as IJsonRpcHandler, type.Options);
                        continue;
                    case JsonRpcHandlerTypeDescription type:
                        handlersManager.Add(type.Method, ActivatorUtilities.CreateInstance(serviceProvider, type.HandlerType) as IJsonRpcHandler, type.Options);
                        continue;
                    case JsonRpcHandlerInstanceDescription instance when string.IsNullOrWhiteSpace(instance.Method):
                        handlersManager.Add(instance.HandlerInstance, instance.Options);
                        continue;
                    case JsonRpcHandlerInstanceDescription instance:
                        handlersManager.Add(instance.Method, instance.HandlerInstance, instance.Options);
                        continue;
                    case JsonRpcHandlerLinkDescription link:
                        links.Add(link);
                        continue;
                }
            }

            foreach (var link in links)
            {
                handlersManager.AddLink(link.Method, link.LinkToMethod);
            }
        }
    }
}
