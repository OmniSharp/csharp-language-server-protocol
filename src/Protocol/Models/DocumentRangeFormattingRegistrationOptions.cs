using System.Collections.Generic;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class DocumentRangeFormattingRegistrationOptions : WorkDoneTextDocumentRegistrationOptions
    {
        public class StaticOptions : WorkDoneProgressOptions { }

        class DocumentRangeFormattingRegistrationOptionsConverter : RegistrationOptionsConverterBase<DocumentRangeFormattingRegistrationOptions, StaticOptions>
        {
            public DocumentRangeFormattingRegistrationOptionsConverter() : base(nameof(ServerCapabilities.DocumentRangeFormattingProvider))
            {
            }
            public override StaticOptions Convert(DocumentRangeFormattingRegistrationOptions source) => new StaticOptions { WorkDoneProgress = source.WorkDoneProgress };
        }
    }
}
