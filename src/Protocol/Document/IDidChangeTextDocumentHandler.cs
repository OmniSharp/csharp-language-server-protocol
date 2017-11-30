using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    using static DocumentNames;
    public static partial class DocumentNames
    {
        public const string DidChange = "textDocument/didChange";
    }

    [Serial, Method(DidChange)]
    public interface IDidChangeTextDocumentHandler : INotificationHandler<DidChangeTextDocumentParams>, IRegistration<TextDocumentChangeRegistrationOptions>, ICapability<SynchronizationCapability> { }
}
