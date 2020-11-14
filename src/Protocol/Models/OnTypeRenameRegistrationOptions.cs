using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class OnTypeRenameRegistrationOptions : WorkDoneTextDocumentRegistrationOptions
    {
        public class StaticOptions : StaticWorkDoneTextDocumentRegistrationOptions
        {
        }

        class OnTypeRenameRegistrationOptionsConverter : RegistrationOptionsConverterBase<OnTypeRenameRegistrationOptions, StaticOptions>
        {
            public OnTypeRenameRegistrationOptionsConverter() : base(nameof(ServerCapabilities.OnTypeRenameProvider))
            {
            }

            public override StaticOptions Convert(OnTypeRenameRegistrationOptions source) => new StaticOptions {
                WorkDoneProgress = source.WorkDoneProgress,
                DocumentSelector = source.DocumentSelector
            };
        }
    }
}
