using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities
{
    /// <summary>
    ///  Format document on type options
    /// </summary>
    public class DocumentOnTypeFormattingOptions : IDocumentOnTypeFormattingOptions
    {
        /// <summary>
        ///  A character on which formatting should be triggered, like `}`.
        /// </summary>
        public string FirstTriggerCharacter { get; set; }

        /// <summary>
        ///  More trigger characters.
        /// </summary>
        [Optional]
        public Container<string> MoreTriggerCharacter { get; set; }

        public static DocumentOnTypeFormattingOptions Of(IDocumentOnTypeFormattingOptions options)
        {
            return new DocumentOnTypeFormattingOptions() {
                FirstTriggerCharacter = options.FirstTriggerCharacter,
                MoreTriggerCharacter = options.MoreTriggerCharacter
            };
        }
    }
}
