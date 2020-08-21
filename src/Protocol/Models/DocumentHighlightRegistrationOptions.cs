using System.Collections.Generic;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class DocumentHighlightRegistrationOptions : WorkDoneTextDocumentRegistrationOptions
    {
        public class StaticOptions : WorkDoneProgressOptions { }

        class DocumentHighlightRegistrationOptionsConverter : RegistrationOptionsConverterBase<DocumentHighlightRegistrationOptions, StaticOptions>
        {
            public DocumentHighlightRegistrationOptionsConverter() : base(nameof(ServerCapabilities.DocumentHighlightProvider))
            {
            }
            public override StaticOptions Convert(DocumentHighlightRegistrationOptions source) => new StaticOptions { WorkDoneProgress = source.WorkDoneProgress };
        }
    }
}
