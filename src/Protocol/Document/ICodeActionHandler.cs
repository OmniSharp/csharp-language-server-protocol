using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    using static DocumentNames;
    public static partial class DocumentNames
    {
        public const string CodeAction = "textDocument/codeAction";
    }

    [Parallel, Method(CodeAction)]
    public interface ICodeActionHandler : IRequestHandler<CodeActionParams, CommandContainer>, IRegistration<TextDocumentRegistrationOptions>, ICapability<CodeActionCapability> { }
}
