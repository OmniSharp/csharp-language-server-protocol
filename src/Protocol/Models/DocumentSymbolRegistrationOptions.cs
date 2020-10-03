using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class DocumentSymbolRegistrationOptions : WorkDoneTextDocumentRegistrationOptions
    {
        public class StaticOptions : WorkDoneProgressOptions { }

        class DocumentSymbolRegistrationOptionsConverter : RegistrationOptionsConverterBase<DocumentSymbolRegistrationOptions, StaticOptions>
        {
            public DocumentSymbolRegistrationOptionsConverter() : base(nameof(ServerCapabilities.DocumentSymbolProvider))
            {
            }
            public override StaticOptions Convert(DocumentSymbolRegistrationOptions source) => new StaticOptions { WorkDoneProgress = source.WorkDoneProgress };
        }
    }
}
