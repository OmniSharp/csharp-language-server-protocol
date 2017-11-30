using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    using static DocumentNames;
    public static partial class DocumentNames
    {
        public const string DidSave = "textDocument/didSave";
    }

    [Serial, Method(DidSave)]
    public interface IDidSaveTextDocumentHandler : INotificationHandler<DidSaveTextDocumentParams>, IRegistration<TextDocumentSaveRegistrationOptions>, ICapability<SynchronizationCapability> { }
}
