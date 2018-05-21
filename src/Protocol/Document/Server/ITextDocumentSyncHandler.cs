using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    public interface ITextDocumentSyncHandler : IDidChangeTextDocumentHandler, IDidOpenTextDocumentHandler, IDidCloseTextDocumentHandler, IDidSaveTextDocumentHandler, ITextDocumentIdentifier
    {
    }
}
