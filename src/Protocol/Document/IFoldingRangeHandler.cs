using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

// ReSharper disable once CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Server
{
    [Parallel, Method(TextDocumentNames.FoldingRange, Direction.ClientToServer)]
    public interface IFoldingRangeHandler : IJsonRpcRequestHandler<FoldingRangeRequestParam, Container<FoldingRange>>,
        IRegistration<FoldingRangeRegistrationOptions>, ICapability<FoldingRangeCapability>
    {
    }

    public abstract class FoldingRangeHandler : IFoldingRangeHandler
    {
        private readonly FoldingRangeRegistrationOptions _options;

        public FoldingRangeHandler(FoldingRangeRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public FoldingRangeRegistrationOptions GetRegistrationOptions() => _options;

        public abstract Task<Container<FoldingRange>> Handle(FoldingRangeRequestParam request,
            CancellationToken cancellationToken);

        public virtual void SetCapability(FoldingRangeCapability capability) => Capability = capability;
        protected FoldingRangeCapability Capability { get; private set; }
    }

    public static class FoldingRangeExtensions
    {
public static ILanguageServerRegistry OnFoldingRange(this ILanguageServerRegistry registry,
            Func<FoldingRangeRequestParam, FoldingRangeCapability, CancellationToken, Task<Container<FoldingRange>>>
                handler,
            FoldingRangeRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new FoldingRangeRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.FoldingRange,
                new LanguageProtocolDelegatingHandlers.Request<FoldingRangeRequestParam, Container<FoldingRange>, FoldingRangeCapability
                    ,
                    FoldingRangeRegistrationOptions>(handler, registrationOptions));
        }

public static ILanguageServerRegistry OnFoldingRange(this ILanguageServerRegistry registry,
            Func<FoldingRangeRequestParam, CancellationToken, Task<Container<FoldingRange>>> handler,
            FoldingRangeRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new FoldingRangeRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.FoldingRange,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<FoldingRangeRequestParam, Container<FoldingRange>,
                    FoldingRangeRegistrationOptions>(handler, registrationOptions));
        }

public static ILanguageServerRegistry OnFoldingRange(this ILanguageServerRegistry registry,
            Func<FoldingRangeRequestParam, Task<Container<FoldingRange>>> handler,
            FoldingRangeRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new FoldingRangeRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.FoldingRange,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<FoldingRangeRequestParam, Container<FoldingRange>,
                    FoldingRangeRegistrationOptions>(handler, registrationOptions));
        }

public static ILanguageServerRegistry OnFoldingRange(this ILanguageServerRegistry registry,
            Action<FoldingRangeRequestParam, IObserver<IEnumerable<FoldingRange>>, FoldingRangeCapability,
                CancellationToken> handler, FoldingRangeRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new FoldingRangeRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.FoldingRange,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<FoldingRangeRequestParam, Container<FoldingRange>,
                        FoldingRange, FoldingRangeCapability, FoldingRangeRegistrationOptions>(handler,
                        registrationOptions, _.GetService<IProgressManager>(), x => new Container<FoldingRange>(x)));
        }

public static ILanguageServerRegistry OnFoldingRange(this ILanguageServerRegistry registry,
            Action<FoldingRangeRequestParam, IObserver<IEnumerable<FoldingRange>>, FoldingRangeCapability>
                handler,
            FoldingRangeRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new FoldingRangeRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.FoldingRange,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<FoldingRangeRequestParam, Container<FoldingRange>,
                        FoldingRange, FoldingRangeCapability, FoldingRangeRegistrationOptions>(handler,
                        registrationOptions, _.GetService<IProgressManager>(), x => new Container<FoldingRange>(x)));
        }

public static ILanguageServerRegistry OnFoldingRange(this ILanguageServerRegistry registry,
            Action<FoldingRangeRequestParam, IObserver<IEnumerable<FoldingRange>>, CancellationToken> handler,
            FoldingRangeRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new FoldingRangeRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.FoldingRange,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<FoldingRangeRequestParam, Container<FoldingRange>,
                        FoldingRange, FoldingRangeRegistrationOptions>(handler, registrationOptions,
                        _.GetService<IProgressManager>(), x => new Container<FoldingRange>(x)));
        }

public static ILanguageServerRegistry OnFoldingRange(this ILanguageServerRegistry registry,
            Action<FoldingRangeRequestParam, IObserver<IEnumerable<FoldingRange>>> handler,
            FoldingRangeRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new FoldingRangeRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.FoldingRange,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<FoldingRangeRequestParam, Container<FoldingRange>,
                        FoldingRange, FoldingRangeRegistrationOptions>(handler, registrationOptions,
                        _.GetService<IProgressManager>(), x => new Container<FoldingRange>(x)));
        }

        public static IRequestProgressObservable<FoldingRange> RequestFoldingRange(
            this ITextDocumentLanguageClient mediator,
            FoldingRangeRequestParam @params,
            CancellationToken cancellationToken = default)
        {
            return mediator.ProgressManager.MonitorUntil(@params, cancellationToken);
        }
    }
}
