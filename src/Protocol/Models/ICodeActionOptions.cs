using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public interface ICodeActionOptions : IWorkDoneProgressOptions
    {
        /// <summary>
        /// CodeActionKinds that this server may return.
        ///
        /// The list of kinds may be generic, such as `CodeActionKind.Refactor`, or the server
        /// may list out every specific kind they provide.
        /// </summary>
        [Optional]
        Container<CodeActionKind> CodeActionKinds { get; set; }
    }
}
