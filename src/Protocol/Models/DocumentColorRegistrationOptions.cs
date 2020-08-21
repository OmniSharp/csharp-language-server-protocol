using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class DocumentColorRegistrationOptions : WorkDoneTextDocumentRegistrationOptions
    {
        public class StaticOptions : StaticWorkDoneTextDocumentRegistrationOptions
        {
        }

        class DocumentColorRegistrationOptionsConverter : RegistrationOptionsConverterBase<DocumentColorRegistrationOptions, StaticOptions>
        {
            public DocumentColorRegistrationOptionsConverter() : base(nameof(ServerCapabilities.ColorProvider))
            {
            }
            public override StaticOptions Convert(DocumentColorRegistrationOptions source) => new StaticOptions { WorkDoneProgress = source.WorkDoneProgress };
        }
    }
}
