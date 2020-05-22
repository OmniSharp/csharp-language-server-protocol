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
    [Serial, Method(TextDocumentNames.PrepareRename, Direction.ClientToServer)]
    public interface IPrepareRenameHandler : IJsonRpcRequestHandler<PrepareRenameParams, RangeOrPlaceholderRange>, IRegistration<object>, ICapability<RenameCapability> { }

    public abstract class PrepareRenameHandler : IPrepareRenameHandler
    {
        private readonly TextDocumentRegistrationOptions _options;
        public PrepareRenameHandler(TextDocumentRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public object GetRegistrationOptions() => new object();
        public abstract Task<RangeOrPlaceholderRange> Handle(PrepareRenameParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(RenameCapability capability) => Capability = capability;
        protected RenameCapability Capability { get; private set; }
    }

    public static class PrepareRenameExtensions
    {
        public static IDisposable OnPrepareRename(
            this ILanguageServerRegistry registry,
            Func<PrepareRenameParams, RenameCapability, CancellationToken, Task<RangeOrPlaceholderRange>>
                handler)
        {
            return registry.AddHandler(TextDocumentNames.PrepareRename,
                new LanguageProtocolDelegatingHandlers.RequestCapability<PrepareRenameParams, RangeOrPlaceholderRange, RenameCapability>(handler));
        }

        public static IDisposable OnPrepareRename(
            this ILanguageServerRegistry registry,
            Func<PrepareRenameParams, CancellationToken, Task<RangeOrPlaceholderRange>> handler)
        {
            return registry.AddHandler(TextDocumentNames.PrepareRename, RequestHandler.For(handler));
        }

        public static IDisposable OnPrepareRename(
            this ILanguageServerRegistry registry,
            Func<PrepareRenameParams, Task<RangeOrPlaceholderRange>> handler)
        {
            return registry.AddHandler(TextDocumentNames.PrepareRename, RequestHandler.For(handler));
        }

        public static Task<RangeOrPlaceholderRange> PrepareRename(this ITextDocumentLanguageClient mediator, PrepareRenameParams @params, CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest(@params, cancellationToken);
        }
    }
}
