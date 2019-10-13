using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities
{
    public class CodeActionOptions : WorkDoneProgressOptions, ICodeActionOptions
    {
        /// <summary>
        /// CodeActionKinds that this server may return.
        ///
        /// The list of kinds may be generic, such as `CodeActionKind.Refactor`, or the server
        /// may list out every specific kind they provide.
        /// </summary>
        [Optional]
        public Container<CodeActionKind> CodeActionKinds { get; set; }

        public static CodeActionOptions Of(ICodeActionOptions options)
        {
            return new CodeActionOptions() {
                CodeActionKinds = options.CodeActionKinds,
                WorkDoneProgress = options.WorkDoneProgress
            };
        }
    }
}
