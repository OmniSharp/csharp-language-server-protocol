using System.Collections.Generic;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class TypeDefinitionRegistrationOptions : WorkDoneTextDocumentRegistrationOptions
    {
        public class StaticOptions : StaticWorkDoneTextDocumentRegistrationOptions { }

        class TypeDefinitionRegistrationOptionsConverter : RegistrationOptionsConverterBase<TypeDefinitionRegistrationOptions, StaticOptions>
        {
            public TypeDefinitionRegistrationOptionsConverter() : base(nameof(ServerCapabilities.TypeDefinitionProvider))
            {
            }
            public override StaticOptions Convert(TypeDefinitionRegistrationOptions source) => new StaticOptions { WorkDoneProgress = source.WorkDoneProgress };
        }
    }
}
