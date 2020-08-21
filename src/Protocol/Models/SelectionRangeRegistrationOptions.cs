using System.Collections.Generic;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class SelectionRangeRegistrationOptions : WorkDoneTextDocumentRegistrationOptions
    {
        public class StaticOptions : StaticWorkDoneTextDocumentRegistrationOptions { }

        class SelectionRangeRegistrationOptionsConverter : RegistrationOptionsConverterBase<SelectionRangeRegistrationOptions, StaticOptions>
        {
            public SelectionRangeRegistrationOptionsConverter() : base(nameof(ServerCapabilities.SelectionRangeProvider))
            {
            }
            public override StaticOptions Convert(SelectionRangeRegistrationOptions source) => new StaticOptions { WorkDoneProgress = source.WorkDoneProgress };
        }
    }
}
