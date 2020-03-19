using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities
{
    public class DeclarationOptions : StaticWorkDoneTextDocumentRegistrationOptions, IDeclarationOptions
    {
        public static DeclarationOptions Of(IDeclarationOptions options)
        {
            return new DeclarationOptions()
            {
                WorkDoneProgress = options.WorkDoneProgress
            };
        }
    }
}
