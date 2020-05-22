using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Document
{
    [Parallel, Method(TextDocumentNames.DocumentColor, Direction.ClientToServer)]
    public interface IDocumentColorHandler : IJsonRpcRequestHandler<DocumentColorParams, Container<ColorPresentation>>, IRegistration<DocumentColorRegistrationOptions>, ICapability<ColorProviderCapability> { }

    public abstract class DocumentColorHandler : IDocumentColorHandler
    {
        private readonly DocumentColorRegistrationOptions _options;
        public DocumentColorHandler(DocumentColorRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public DocumentColorRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<Container<ColorPresentation>> Handle(DocumentColorParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(ColorProviderCapability capability) => Capability = capability;
        protected ColorProviderCapability Capability { get; private set; }
    }

    public static class DocumentColorExtensions
    {
        public static IDisposable OnDocumentColor(
            this ILanguageServerRegistry registry,
            Func<DocumentColorParams, ColorProviderCapability, CancellationToken, Task<Container<ColorPresentation>>>
                handler,
            DocumentColorRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentColorRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DocumentColor,
                new LanguageProtocolDelegatingHandlers.Request<DocumentColorParams, Container<ColorPresentation>, ColorProviderCapability,
                    DocumentColorRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDocumentColor(
            this ILanguageServerRegistry registry,
            Func<DocumentColorParams, CancellationToken, Task<Container<ColorPresentation>>> handler,
            DocumentColorRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentColorRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DocumentColor,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<DocumentColorParams, Container<ColorPresentation>,
                    DocumentColorRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDocumentColor(
            this ILanguageServerRegistry registry,
            Func<DocumentColorParams, Task<Container<ColorPresentation>>> handler,
            DocumentColorRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentColorRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DocumentColor,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<DocumentColorParams, Container<ColorPresentation>,
                    DocumentColorRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDocumentColor(
            this ILanguageServerRegistry registry,
            Action<DocumentColorParams, IObserver<IEnumerable<ColorPresentation>>, ColorProviderCapability,
                CancellationToken> handler,
            DocumentColorRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentColorRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DocumentColor,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<DocumentColorParams, Container<ColorPresentation>,
                        ColorPresentation, ColorProviderCapability, DocumentColorRegistrationOptions>(handler,
                        registrationOptions, _.GetService<IProgressManager>(),
                        x => new Container<ColorPresentation>(x)));
        }

        public static IDisposable OnDocumentColor(
            this ILanguageServerRegistry registry,
            Action<DocumentColorParams, IObserver<IEnumerable<ColorPresentation>>, ColorProviderCapability>
                handler,
            DocumentColorRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentColorRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DocumentColor,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<DocumentColorParams, Container<ColorPresentation>,
                        ColorPresentation, ColorProviderCapability, DocumentColorRegistrationOptions>(handler,
                        registrationOptions, _.GetService<IProgressManager>(),
                        x => new Container<ColorPresentation>(x)));
        }

        public static IDisposable OnDocumentColor(
            this ILanguageServerRegistry registry,
            Action<DocumentColorParams, IObserver<IEnumerable<ColorPresentation>>, CancellationToken> handler,
            DocumentColorRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentColorRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DocumentColor,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<DocumentColorParams, Container<ColorPresentation>,
                        ColorPresentation, DocumentColorRegistrationOptions>(handler, registrationOptions,
                        _.GetService<IProgressManager>(),
                        x => new Container<ColorPresentation>(x)));
        }

        public static IDisposable OnDocumentColor(
            this ILanguageServerRegistry registry,
            Action<DocumentColorParams, IObserver<IEnumerable<ColorPresentation>>> handler,
            DocumentColorRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentColorRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DocumentColor,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<DocumentColorParams, Container<ColorPresentation>,
                        ColorPresentation, DocumentColorRegistrationOptions>(handler, registrationOptions,
                        _.GetService<IProgressManager>(),
                        x => new Container<ColorPresentation>(x)));
        }

        public static IRequestProgressObservable<ColorPresentation> RequestDocumentColor(
            this ITextDocumentLanguageClient mediator,
            DocumentColorParams @params,
            CancellationToken cancellationToken = default)
        {
            return mediator.ProgressManager.MonitorUntil(@params, cancellationToken);
        }
    }
}
