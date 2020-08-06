using System;
using System.Collections.Generic;
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
            container.RegisterMany<LanguageProtocolSettingsBag>(nonPublicServiceTypes: true);

            container.RegisterMany<SupportedCapabilities>();
            container.RegisterMany<TextDocumentIdentifiers>();
            container.RegisterMany<LspRequestRouter>();
            container.RegisterMany<SharedHandlerCollection>(nonPublicServiceTypes: true);
            container.RegisterMany<ResponseRouter>();
            container.RegisterMany<ProgressManager>();

            return container;
        }
    }
}
