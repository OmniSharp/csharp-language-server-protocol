using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Document
{
    [Parallel, Method(TextDocumentNames.ColorPresentation, Direction.ClientToServer)]
    public interface IColorPresentationHandler : IJsonRpcRequestHandler<ColorPresentationParams, Container<ColorPresentation>>, IRegistration<DocumentColorRegistrationOptions>, ICapability<ColorProviderCapability> { }

    public abstract class ColorPresentationHandler : IColorPresentationHandler
    {
        private readonly DocumentColorRegistrationOptions _options;
        public ColorPresentationHandler(DocumentColorRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public DocumentColorRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<Container<ColorPresentation>> Handle(ColorPresentationParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(ColorProviderCapability capability) => Capability = capability;
        protected ColorProviderCapability Capability { get; private set; }
    }

    public static class ColorPresentationExtensions
    {
        public static IDisposable OnColorPresentation(
            this ILanguageServerRegistry registry,
            Func<ColorPresentationParams, ColorProviderCapability, CancellationToken, Task<Container<ColorPresentation>>>
                handler,
            DocumentColorRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentColorRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.ColorPresentation,
                new LanguageProtocolDelegatingHandlers.Request<ColorPresentationParams, Container<ColorPresentation>, ColorProviderCapability,
                    DocumentColorRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnColorPresentation(
            this ILanguageServerRegistry registry,
            Func<ColorPresentationParams, CancellationToken, Task<Container<ColorPresentation>>> handler,
            DocumentColorRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentColorRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.ColorPresentation,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<ColorPresentationParams, Container<ColorPresentation>,
                    DocumentColorRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnColorPresentation(
            this ILanguageServerRegistry registry,
            Func<ColorPresentationParams, Task<Container<ColorPresentation>>> handler,
            DocumentColorRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentColorRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.ColorPresentation,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<ColorPresentationParams, Container<ColorPresentation>,
                    DocumentColorRegistrationOptions>(handler, registrationOptions));
        }

        public static Task<Container<ColorPresentation>> RequestColorPresentation(this ITextDocumentLanguageClient mediator, ColorPresentationParams @params, CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest(@params, cancellationToken);
        }
    }
}
