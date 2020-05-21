using System.Collections.Generic;
using System.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities
{
    /// <summary>
    ///  Completion options.
    /// </summary>
    public class CompletionOptions : WorkDoneProgressOptions, ICompletionOptions
    {
        /// <summary>
        /// The server provides support to resolve additional
        /// information for a completion item.
        /// </summary>
        [Optional]
        public bool ResolveProvider { get; set; }

        /// <summary>
        /// Most tools trigger completion request automatically without explicitly requesting
        /// it using a keyboard shortcut (e.g. Ctrl+Space). Typically they do so when the user
        /// starts to type an identifier. For example if the user types `c` in a JavaScript file
        /// code complete will automatically pop up present `console` besides others as a
        /// completion item. Characters that make up identifiers don't need to be listed here.
        ///
        /// If code complete should automatically be trigger on characters not being valid inside
        /// an identifier (for example `.` in JavaScript) list them in `triggerCharacters`.
        /// </summary>
        [Optional]
        public Container<string> TriggerCharacters { get; set; }

        /// <summary>
        /// The list of all possible characters that commit a completion. This field can be used
        /// if clients don't support individual commmit characters per completion item. See
        /// `Capability.textDocument.completion.completionItem.commitCharactersSupport`
        ///
        /// @since 3.2.0
        /// </summary>
        [Optional]
        public Container<string> AllCommitCharacters { get; set; }

        public static CompletionOptions Of(ICompletionOptions options, IEnumerable<IHandlerDescriptor> descriptors)
        {
            return new CompletionOptions()
            {
                ResolveProvider = options.ResolveProvider || descriptors.Any(z => z.HandlerType == typeof(ICompletionResolveHandler)),
                AllCommitCharacters = options.AllCommitCharacters,
                TriggerCharacters = options.TriggerCharacters,
                WorkDoneProgress = options.WorkDoneProgress,
            };
        }
    }
}
