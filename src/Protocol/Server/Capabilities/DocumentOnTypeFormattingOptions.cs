using System.Collections.Generic;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

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

        public static DocumentOnTypeFormattingOptions Of(IDocumentOnTypeFormattingOptions options, IEnumerable<IHandlerDescriptor> descriptors)
        {
            return new DocumentOnTypeFormattingOptions()
            {
                FirstTriggerCharacter = options.FirstTriggerCharacter,
                MoreTriggerCharacter = options.MoreTriggerCharacter,
            };
        }
    }
}
