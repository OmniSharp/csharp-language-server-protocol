﻿//HintName: IFoldingRangeHandler.cs
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.DebugAdapter.Protocol;
using OmniSharp.Extensions.DebugAdapter.Protocol.Events;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Test;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Test
{
#nullable enable
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute, System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public static partial class FoldingRangeExtensions
    {
        public static ILanguageServerRegistry OnFoldingRange(this ILanguageServerRegistry registry, Func<FoldingRangeRequestParam, Task<Container<FoldingRange>>> handler, RegistrationOptionsDelegate<FoldingRangeRegistrationOptions, FoldingRangeCapability> registrationOptions)
        {
            return registry.AddHandler(TextDocumentNames.FoldingRange, new LanguageProtocolDelegatingHandlers.Request<FoldingRangeRequestParam, Container<FoldingRange>, FoldingRangeRegistrationOptions, FoldingRangeCapability>(HandlerAdapter<FoldingRangeCapability>.Adapt<FoldingRangeRequestParam, Container<FoldingRange>>(handler), RegistrationAdapter<FoldingRangeCapability>.Adapt<FoldingRangeRegistrationOptions>(registrationOptions)));
        }

        public static ILanguageServerRegistry OnFoldingRange(this ILanguageServerRegistry registry, Func<FoldingRangeRequestParam, CancellationToken, Task<Container<FoldingRange>>> handler, RegistrationOptionsDelegate<FoldingRangeRegistrationOptions, FoldingRangeCapability> registrationOptions)
        {
            return registry.AddHandler(TextDocumentNames.FoldingRange, new LanguageProtocolDelegatingHandlers.Request<FoldingRangeRequestParam, Container<FoldingRange>, FoldingRangeRegistrationOptions, FoldingRangeCapability>(HandlerAdapter<FoldingRangeCapability>.Adapt<FoldingRangeRequestParam, Container<FoldingRange>>(handler), RegistrationAdapter<FoldingRangeCapability>.Adapt<FoldingRangeRegistrationOptions>(registrationOptions)));
        }

        public static ILanguageServerRegistry OnFoldingRange(this ILanguageServerRegistry registry, Func<FoldingRangeRequestParam, FoldingRangeCapability, CancellationToken, Task<Container<FoldingRange>>> handler, RegistrationOptionsDelegate<FoldingRangeRegistrationOptions, FoldingRangeCapability> registrationOptions)
        {
            return registry.AddHandler(TextDocumentNames.FoldingRange, new LanguageProtocolDelegatingHandlers.Request<FoldingRangeRequestParam, Container<FoldingRange>, FoldingRangeRegistrationOptions, FoldingRangeCapability>(HandlerAdapter<FoldingRangeCapability>.Adapt<FoldingRangeRequestParam, Container<FoldingRange>>(handler), RegistrationAdapter<FoldingRangeCapability>.Adapt<FoldingRangeRegistrationOptions>(registrationOptions)));
        }

        public static ILanguageServerRegistry ObserveFoldingRange(this ILanguageServerRegistry registry, Action<FoldingRangeRequestParam, IObserver<IEnumerable<FoldingRange>>> handler, RegistrationOptionsDelegate<FoldingRangeRegistrationOptions, FoldingRangeCapability> registrationOptions)
        {
            return registry.AddHandler(TextDocumentNames.FoldingRange, _ => new LanguageProtocolDelegatingHandlers.PartialResults<FoldingRangeRequestParam, Container<FoldingRange>, FoldingRange, FoldingRangeRegistrationOptions, FoldingRangeCapability>(PartialAdapter<FoldingRangeCapability>.Adapt<FoldingRangeRequestParam, FoldingRange>(handler), RegistrationAdapter<FoldingRangeCapability>.Adapt<FoldingRangeRegistrationOptions>(registrationOptions), _.GetService<IProgressManager>(), Container<FoldingRange>.From));
        }

        public static ILanguageServerRegistry ObserveFoldingRange(this ILanguageServerRegistry registry, Action<FoldingRangeRequestParam, IObserver<IEnumerable<FoldingRange>>, CancellationToken> handler, RegistrationOptionsDelegate<FoldingRangeRegistrationOptions, FoldingRangeCapability> registrationOptions)
        {
            return registry.AddHandler(TextDocumentNames.FoldingRange, _ => new LanguageProtocolDelegatingHandlers.PartialResults<FoldingRangeRequestParam, Container<FoldingRange>, FoldingRange, FoldingRangeRegistrationOptions, FoldingRangeCapability>(PartialAdapter<FoldingRangeCapability>.Adapt<FoldingRangeRequestParam, FoldingRange>(handler), RegistrationAdapter<FoldingRangeCapability>.Adapt<FoldingRangeRegistrationOptions>(registrationOptions), _.GetService<IProgressManager>(), Container<FoldingRange>.From));
        }

        public static ILanguageServerRegistry ObserveFoldingRange(this ILanguageServerRegistry registry, Action<FoldingRangeRequestParam, IObserver<IEnumerable<FoldingRange>>, FoldingRangeCapability, CancellationToken> handler, RegistrationOptionsDelegate<FoldingRangeRegistrationOptions, FoldingRangeCapability> registrationOptions)
        {
            return registry.AddHandler(TextDocumentNames.FoldingRange, _ => new LanguageProtocolDelegatingHandlers.PartialResults<FoldingRangeRequestParam, Container<FoldingRange>, FoldingRange, FoldingRangeRegistrationOptions, FoldingRangeCapability>(PartialAdapter<FoldingRangeCapability>.Adapt<FoldingRangeRequestParam, FoldingRange>(handler), RegistrationAdapter<FoldingRangeCapability>.Adapt<FoldingRangeRegistrationOptions>(registrationOptions), _.GetService<IProgressManager>(), Container<FoldingRange>.From));
        }

        public static IRequestProgressObservable<IEnumerable<FoldingRange>, Container<FoldingRange>> RequestFoldingRange(this ITextDocumentLanguageClient mediator, FoldingRangeRequestParam request, CancellationToken cancellationToken = default) => mediator.ProgressManager.MonitorUntil(request, Container<FoldingRange>.From, cancellationToken);
        public static IRequestProgressObservable<IEnumerable<FoldingRange>, Container<FoldingRange>> RequestFoldingRange(this ILanguageClient mediator, FoldingRangeRequestParam request, CancellationToken cancellationToken = default) => mediator.ProgressManager.MonitorUntil(request, Container<FoldingRange>.From, cancellationToken);
    }
#nullable restore
}