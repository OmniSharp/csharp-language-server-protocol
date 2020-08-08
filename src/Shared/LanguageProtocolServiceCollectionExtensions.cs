using System;
using System.Collections.Generic;
using System.Linq;
using DryIoc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;
using OmniSharp.Extensions.LanguageServer.Protocol.Shared;

namespace OmniSharp.Extensions.LanguageServer.Shared
{
    internal static class LanguageProtocolServiceCollectionExtensions
    {
        internal static IContainer AddLanguageProtocolInternals<T>(this IContainer container, LanguageProtocolRpcOptionsBase<T> options) where T : IJsonRpcHandlerRegistry<T>
        {
            options.RequestProcessIdentifier ??= (options.SupportsContentModified
                ? new RequestProcessIdentifier(RequestProcessType.Parallel)
                : new RequestProcessIdentifier(RequestProcessType.Serial));

            if (options.Serializer == null)
            {
                throw new ArgumentException("Serializer is missing!", nameof(options));
            }

            container = container.AddJsonRpcServerCore(options);

            container.RegisterInstanceMany(options.Serializer);
            container.RegisterInstance(options.RequestProcessIdentifier);
            container.RegisterMany<LanguageProtocolSettingsBag>(nonPublicServiceTypes: true, reuse: Reuse.Singleton);

            container.RegisterMany<SupportedCapabilities>(reuse: Reuse.Singleton);
            container.Register<TextDocumentIdentifiers>(reuse: Reuse.Singleton);
            container.RegisterInitializer<TextDocumentIdentifiers>((identifiers, context) => {
                identifiers.Add(context.GetServices<ITextDocumentIdentifier>().ToArray());
            });
            container.RegisterMany<LspRequestRouter>(reuse: Reuse.Singleton);
            container.RegisterMany<SharedHandlerCollection>(nonPublicServiceTypes: true, reuse: Reuse.Singleton);
            container.RegisterInitializer<SharedHandlerCollection>((manager, context) => {
                var descriptions = context.Resolve<IJsonRpcHandlerCollection>();
                descriptions.Populate(context, manager);
            });
            container.RegisterMany<ResponseRouter>(reuse: Reuse.Singleton);
            container.RegisterMany<ProgressManager>(reuse: Reuse.Singleton);

            return container;
        }
    }
}
