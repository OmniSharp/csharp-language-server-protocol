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
    [Parallel, Method(TextDocumentNames.SelectionRange, Direction.ClientToServer)]
    public interface ISelectionRangeHandler : IJsonRpcRequestHandler<SelectionRangeParams, Container<SelectionRange>>,
        IRegistration<SelectionRangeRegistrationOptions>, ICapability<SelectionRangeCapability>
    {
    }

    public abstract class SelectionRangeHandler : ISelectionRangeHandler
    {
        private readonly SelectionRangeRegistrationOptions _options;

        public SelectionRangeHandler(SelectionRangeRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public SelectionRangeRegistrationOptions GetRegistrationOptions() => _options;

        public abstract Task<Container<SelectionRange>> Handle(SelectionRangeParams request,
            CancellationToken cancellationToken);

        public virtual void SetCapability(SelectionRangeCapability capability) => Capability = capability;
        protected SelectionRangeCapability Capability { get; private set; }
    }

    public static class SelectionRangeExtensions
    {
public static ILanguageServerRegistry OnSelectionRange(this ILanguageServerRegistry registry,
            Func<SelectionRangeParams, SelectionRangeCapability, CancellationToken, Task<Container<SelectionRange>>>
                handler,
            SelectionRangeRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new SelectionRangeRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.SelectionRange,
                new LanguageProtocolDelegatingHandlers.Request<SelectionRangeParams, Container<SelectionRange>, SelectionRangeCapability
                    ,
                    SelectionRangeRegistrationOptions>(handler, registrationOptions));
        }

public static ILanguageServerRegistry OnSelectionRange(this ILanguageServerRegistry registry,
            Func<SelectionRangeParams, CancellationToken, Task<Container<SelectionRange>>> handler,
            SelectionRangeRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new SelectionRangeRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.SelectionRange,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<SelectionRangeParams, Container<SelectionRange>,
                    SelectionRangeRegistrationOptions>(handler, registrationOptions));
        }

public static ILanguageServerRegistry OnSelectionRange(this ILanguageServerRegistry registry,
            Func<SelectionRangeParams, Task<Container<SelectionRange>>> handler,
            SelectionRangeRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new SelectionRangeRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.SelectionRange,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<SelectionRangeParams, Container<SelectionRange>,
                    SelectionRangeRegistrationOptions>(handler, registrationOptions));
        }

public static ILanguageServerRegistry OnSelectionRange(this ILanguageServerRegistry registry,
            Action<SelectionRangeParams, IObserver<IEnumerable<SelectionRange>>, SelectionRangeCapability,
                CancellationToken> handler,
            SelectionRangeRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new SelectionRangeRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.SelectionRange,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<SelectionRangeParams, Container<SelectionRange>,
                        SelectionRange, SelectionRangeCapability, SelectionRangeRegistrationOptions>(handler,
                        registrationOptions, _.GetService<IProgressManager>(), x => new Container<SelectionRange>(x)));
        }

public static ILanguageServerRegistry OnSelectionRange(this ILanguageServerRegistry registry,
            Action<SelectionRangeParams, IObserver<IEnumerable<SelectionRange>>, SelectionRangeCapability>
                handler,
            SelectionRangeRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new SelectionRangeRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.SelectionRange,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<SelectionRangeParams, Container<SelectionRange>,
                        SelectionRange, SelectionRangeCapability, SelectionRangeRegistrationOptions>(handler,
                        registrationOptions, _.GetService<IProgressManager>(), x => new Container<SelectionRange>(x)));
        }

public static ILanguageServerRegistry OnSelectionRange(this ILanguageServerRegistry registry,
            Action<SelectionRangeParams, IObserver<IEnumerable<SelectionRange>>, CancellationToken> handler,
            SelectionRangeRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new SelectionRangeRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.SelectionRange,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<SelectionRangeParams, Container<SelectionRange>,
                        SelectionRange, SelectionRangeRegistrationOptions>(handler, registrationOptions,
                        _.GetService<IProgressManager>(), x => new Container<SelectionRange>(x)));
        }

public static ILanguageServerRegistry OnSelectionRange(this ILanguageServerRegistry registry,
            Action<SelectionRangeParams, IObserver<IEnumerable<SelectionRange>>> handler,
            SelectionRangeRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new SelectionRangeRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.SelectionRange,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<SelectionRangeParams, Container<SelectionRange>,
                        SelectionRange, SelectionRangeRegistrationOptions>(handler, registrationOptions,
                        _.GetService<IProgressManager>(), x => new Container<SelectionRange>(x)));
        }

        public static IRequestProgressObservable<SelectionRange> RequestSelectionRange(
            this ITextDocumentLanguageClient mediator,
            SelectionRangeParams @params,
            CancellationToken cancellationToken = default)
        {
            return mediator.ProgressManager.MonitorUntil(@params, cancellationToken);
        }
    }
}
