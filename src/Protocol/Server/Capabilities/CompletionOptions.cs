using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities
{
    /// <summary>
    ///  Completion options.
    /// </summary>
    public class CompletionOptions : ICompletionOptions
    {
        /// <summary>
        ///  The server provides support to resolve additional
        ///  information for a completion item.
        /// </summary>
        [Optional]
        public bool ResolveProvider { get; set; }

        /// <summary>
        ///  The characters that trigger completion automatically.
        /// </summary>
        [Optional]
        public Container<string> TriggerCharacters { get; set; }

        /// <summary>
        /// The list of all possible characters that commit a completion. This field can be used
        /// if clients don't support individual commit characters per completion item. See
        /// `ClientCapabilities.textDocument.completion.completionItem.commitCharactersSupport`
        ///
        /// Since 3.2.0
        [Optional]
        public Container<string> AllCommitCharacters { get; set; }

        public static CompletionOptions Of(ICompletionOptions options)
        {
            return new CompletionOptions()
            {
                AllCommitCharacters = options.AllCommitCharacters,
                ResolveProvider = options.ResolveProvider,
                TriggerCharacters = options.TriggerCharacters
            };
        }
    }
}
