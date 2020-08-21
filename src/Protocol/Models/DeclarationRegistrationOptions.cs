using System.Collections.Generic;
using System.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class DeclarationRegistrationOptions : WorkDoneTextDocumentRegistrationOptions
    {
        public class StaticOptions : StaticWorkDoneTextDocumentRegistrationOptions { }

        class DeclarationRegistrationOptionsConverter : RegistrationOptionsConverterBase<DeclarationRegistrationOptions, StaticOptions>
        {
            public DeclarationRegistrationOptionsConverter() : base(nameof(ServerCapabilities.DeclarationProvider))
            {
            }
            public override StaticOptions Convert(DeclarationRegistrationOptions source) => new StaticOptions { WorkDoneProgress = source.WorkDoneProgress };
        }
    }
}
