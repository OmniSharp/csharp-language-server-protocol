using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class FoldingRangeRegistrationOptions : WorkDoneTextDocumentRegistrationOptions
    {
        public class StaticOptions : StaticWorkDoneTextDocumentRegistrationOptions
        {
        }

        class FoldingRangeRegistrationOptionsConverter : RegistrationOptionsConverterBase<FoldingRangeRegistrationOptions, StaticOptions>
        {
            public FoldingRangeRegistrationOptionsConverter() : base(nameof(ServerCapabilities.FoldingRangeProvider))
            {
            }

            public override StaticOptions Convert(FoldingRangeRegistrationOptions source) => new StaticOptions {
                WorkDoneProgress = source.WorkDoneProgress,
                DocumentSelector = source.DocumentSelector
            };
        }
    }
}
