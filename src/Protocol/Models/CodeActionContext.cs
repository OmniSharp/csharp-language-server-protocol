using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// Contains additional diagnostic information about the context in which
    /// a code action is run.
    /// </summary>
    public class CodeActionContext
    {
        /// <summary>
        /// An array of diagnostics.
        /// </summary>
        public Container<Diagnostic> Diagnostics { get; set; }

        /// <summary>
        /// Requested kind of actions to return.
        ///
        /// Actions not of this kind are filtered out by the client before being shown. So servers
        /// can omit computing them.
        /// </summary>
        public Container<CodeActionKind> Only { get; set; }
    }
}
