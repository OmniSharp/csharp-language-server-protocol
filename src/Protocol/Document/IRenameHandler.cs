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
    [Serial, Method(TextDocumentNames.Rename, Direction.ClientToServer)]
    public interface IRenameHandler : IJsonRpcRequestHandler<RenameParams, WorkspaceEdit>, IRegistration<RenameRegistrationOptions>, ICapability<RenameCapability> { }

    public abstract class RenameHandler : IRenameHandler
    {
        private readonly RenameRegistrationOptions _options;
        public RenameHandler(RenameRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public RenameRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<WorkspaceEdit> Handle(RenameParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(RenameCapability capability) => Capability = capability;
        protected RenameCapability Capability { get; private set; }
    }

    public static class RenameExtensions
    {
        public static IDisposable OnRename(
            this ILanguageServerRegistry registry,
            Func<RenameParams, RenameCapability, CancellationToken, Task<WorkspaceEdit>>
                handler,
            RenameRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new RenameRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.Rename,
                new LanguageProtocolDelegatingHandlers.Request<RenameParams, WorkspaceEdit, RenameCapability,
                    RenameRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnRename(
            this ILanguageServerRegistry registry,
            Func<RenameParams, CancellationToken, Task<WorkspaceEdit>> handler,
            RenameRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new RenameRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.Rename,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<RenameParams, WorkspaceEdit,
                    RenameRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnRename(
            this ILanguageServerRegistry registry,
            Func<RenameParams, Task<WorkspaceEdit>> handler,
            RenameRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new RenameRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.Rename,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<RenameParams, WorkspaceEdit,
                    RenameRegistrationOptions>(handler, registrationOptions));
        }

        public static Task<WorkspaceEdit> RequestRename(this ITextDocumentLanguageClient mediator, RenameParams @params, CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest(@params, cancellationToken);
        }
    }
}
