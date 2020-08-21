using System.Collections.Generic;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class DocumentFormattingRegistrationOptions : WorkDoneTextDocumentRegistrationOptions
    {
        public class StaticOptions : WorkDoneProgressOptions
        {
        }

        class DocumentFormattingRegistrationOptionsConverter : RegistrationOptionsConverterBase<DocumentFormattingRegistrationOptions, StaticOptions>
        {
            public DocumentFormattingRegistrationOptionsConverter() : base(nameof(ServerCapabilities.DocumentFormattingProvider))
            {
            }
            public override StaticOptions Convert(DocumentFormattingRegistrationOptions source) => new StaticOptions { WorkDoneProgress = source.WorkDoneProgress };
        }
    }
}
