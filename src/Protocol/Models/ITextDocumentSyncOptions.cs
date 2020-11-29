using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public interface ITextDocumentSyncOptions
    {
        [Optional] bool OpenClose { get; set; }
        [Optional] TextDocumentSyncKind Change { get; set; }
        [Optional] bool WillSave { get; set; }
        [Optional] bool WillSaveWaitUntil { get; set; }
        [Optional] BooleanOr<SaveOptions> Save { get; set; }
    }
}
