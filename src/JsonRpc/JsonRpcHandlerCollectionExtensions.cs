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
                    case JsonRpcHandlerFactoryDescription factory when !(factory.Method is null):
                        handlersManager.Add(factory.Method, factory.HandlerFactory(serviceProvider), factory.Options);
                        continue;
                    case JsonRpcHandlerFactoryDescription factory:
                        handlersManager.Add(factory.HandlerFactory(serviceProvider), factory.Options);
                        continue;
                    case JsonRpcHandlerTypeDescription type when !(type.Method is null):
                        handlersManager.Add(type.Method, (ActivatorUtilities.CreateInstance(serviceProvider, type.HandlerType) as IJsonRpcHandler)!, type.Options);
                        continue;
                    case JsonRpcHandlerTypeDescription type:
                        handlersManager.Add((ActivatorUtilities.CreateInstance(serviceProvider, type.HandlerType) as IJsonRpcHandler)!, type.Options);
                        continue;
                    case JsonRpcHandlerInstanceDescription instance when !(instance.Method is null):
                        handlersManager.Add(instance.Method, instance.HandlerInstance, instance.Options);
                        continue;
                    case JsonRpcHandlerInstanceDescription instance:
                        handlersManager.Add(instance.HandlerInstance, instance.Options);
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
