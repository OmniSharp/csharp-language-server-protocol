using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities
{
    public class DefinitionOptions : StaticWorkDoneTextDocumentRegistrationOptions, IDefinitionOptions
    {
        public static DefinitionOptions Of(IDefinitionOptions options)
        {
            return new DefinitionOptions()
            {
                WorkDoneProgress = options.WorkDoneProgress,
            };
        }
    }
}
