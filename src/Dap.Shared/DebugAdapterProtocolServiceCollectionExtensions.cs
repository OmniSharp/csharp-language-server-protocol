using System;
using DryIoc;
using OmniSharp.Extensions.DebugAdapter.Protocol;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Shared
{
    internal static class DebugAdapterProtocolServiceCollectionExtensions
    {
        internal static IContainer AddDebugAdapterProtocolInternals<T>(this IContainer container, DebugAdapterRpcOptionsBase<T> options) where T : IJsonRpcHandlerRegistry<T>
        {
            if (options.Serializer == null)
            {
                throw new ArgumentException("Serializer is missing!", nameof(options));
            }

            container = container.AddJsonRpcServerCore(options);

            container.RegisterInstanceMany(options.Serializer);
            container.RegisterInstance(options.RequestProcessIdentifier);
            container.RegisterMany<DebugAdapterSettingsBag>(nonPublicServiceTypes: true, reuse: Reuse.Singleton);
            container.RegisterMany<DapReceiver>(nonPublicServiceTypes: true, reuse: Reuse.Singleton);
            container.RegisterMany<DebugAdapterRequestRouter>(Reuse.Singleton);
            container.RegisterMany<DebugAdapterHandlerCollection>(nonPublicServiceTypes: true, reuse: Reuse.Singleton);
            container.RegisterInitializer<DebugAdapterHandlerCollection>(
                (manager, context) => {
                    var descriptions = context.Resolve<IJsonRpcHandlerCollection>();
                    descriptions.Populate(context, manager);
                }
            );
            container.RegisterMany<DapResponseRouter>(Reuse.Singleton);

            return container;
        }
    }
}
