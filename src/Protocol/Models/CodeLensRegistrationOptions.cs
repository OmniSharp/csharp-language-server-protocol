using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class CodeLensRegistrationOptions : TextDocumentRegistrationOptions, ICodeLensOptions
    {
        /// <summary>
        /// Code lens has a resolve provider as well.
        /// </summary>
        [Optional]
        public bool ResolveProvider { get; set; }
    }
}
