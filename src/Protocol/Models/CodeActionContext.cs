using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// Contains additional diagnostic information about the context in which
    /// a code action is run.
    /// </summary>
    public class CodeActionContext
    {
        /// <summary>
        /// An array of diagnostics known on the client side overlapping the range provided to the
        /// `textDocument/codeAction` request. They are provied so that the server knows which
        /// errors are currently presented to the user for the given range. There is no guarantee
        /// that these accurately reflect the error state of the resource. The primary parameter
        /// to compute code actions is the provided range.
        /// </summary>
        public Container<Diagnostic> Diagnostics { get; set; }

        /// <summary>
        /// Requested kind of actions to return.
        ///
        /// Actions not of this kind are filtered out by the client before being shown. So servers
        /// can omit computing them.
        /// </summary>
        [Optional]
        public Container<CodeActionKind> Only { get; set; }
    }
}
