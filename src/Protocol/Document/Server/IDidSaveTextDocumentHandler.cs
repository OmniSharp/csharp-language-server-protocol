using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    using static DocumentNames;
    [Serial, Method(DidSave)]
    public interface IDidSaveTextDocumentHandler : IJsonRpcNotificationHandler<DidSaveTextDocumentParams>, IRegistration<TextDocumentSaveRegistrationOptions>, ICapability<SynchronizationCapability> { }
}
