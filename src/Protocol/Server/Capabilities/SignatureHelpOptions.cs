using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities
{
    /// <summary>
    ///  Signature help options.
    /// </summary>
    public class SignatureHelpOptions : ISignatureHelpOptions
    {
        /// <summary>
        ///  The characters that trigger signature help
        ///  automatically.
        /// </summary>
        [Optional]
        public Container<string> TriggerCharacters { get; set; }

        public static SignatureHelpOptions Of(ISignatureHelpOptions options)
        {
            return new SignatureHelpOptions() { TriggerCharacters = options.TriggerCharacters };
        }
    }
}
