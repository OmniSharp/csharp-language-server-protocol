using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities
{
    /// <summary>
    ///  Code Lens options.
    /// </summary>
    public class CodeLensOptions : ICodeLensOptions
    {
        /// <summary>
        ///  Code lens has a resolve provider as well.
        /// </summary>
        [Optional]
        public bool ResolveProvider { get; set; }

        public static CodeLensOptions Of(ICodeLensOptions options)
        {
            return new CodeLensOptions() { ResolveProvider = options.ResolveProvider };
        }
    }
}
