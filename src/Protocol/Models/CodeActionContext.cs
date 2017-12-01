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
    }
}
