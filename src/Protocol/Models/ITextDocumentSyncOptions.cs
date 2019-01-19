using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public interface ITextDocumentSyncOptions
    {
        bool OpenClose { get; set; }
        TextDocumentSyncKind Change { get; set; }
        bool WillSave { get; set; }
        bool WillSaveWaitUntil { get; set; }
        SaveOptions Save { get; set; }
    }
}
