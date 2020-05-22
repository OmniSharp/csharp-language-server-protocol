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
    [Parallel, Method(TextDocumentNames.Hover, Direction.ClientToServer)]
    public interface IHoverHandler : IJsonRpcRequestHandler<HoverParams, Hover>, IRegistration<HoverRegistrationOptions>, ICapability<HoverCapability> { }

    public abstract class HoverHandler : IHoverHandler
    {
        private readonly HoverRegistrationOptions _options;
        public HoverHandler(HoverRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public HoverRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<Hover> Handle(HoverParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(HoverCapability capability) => Capability = capability;
        protected HoverCapability Capability { get; private set; }
    }

    public static class HoverExtensions
    {
        public static IDisposable OnHover(
            this ILanguageServerRegistry registry,
            Func<HoverParams, HoverCapability, CancellationToken, Task<Hover>>
                handler,
            HoverRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new HoverRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.Hover,
                new LanguageProtocolDelegatingHandlers.Request<HoverParams, Hover, HoverCapability,
                    HoverRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnHover(
            this ILanguageServerRegistry registry,
            Func<HoverParams, CancellationToken, Task<Hover>> handler,
            HoverRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new HoverRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.Hover,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<HoverParams, Hover,
                    HoverRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnHover(
            this ILanguageServerRegistry registry,
            Func<HoverParams, Task<Hover>> handler,
            HoverRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new HoverRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.Hover,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<HoverParams, Hover,
                    HoverRegistrationOptions>(handler, registrationOptions));
        }

        public static Task<Hover> RequestHover(this ITextDocumentLanguageClient mediator, HoverParams @params, CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest(@params, cancellationToken);
        }
    }
}
