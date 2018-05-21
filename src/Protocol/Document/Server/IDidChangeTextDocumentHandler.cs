using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    using static DocumentNames;

    [Serial, Method(DidChange)]
    public interface IDidChangeTextDocumentHandler : IJsonRpcNotificationHandler<DidChangeTextDocumentParams>,
        IRegistration<TextDocumentChangeRegistrationOptions>, ICapability<SynchronizationCapability>
    {
        TextDocumentSyncKind Change { get; }
    }
}
