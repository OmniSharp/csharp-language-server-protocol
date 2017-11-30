using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    using static DocumentNames;
    public static partial class DocumentNames
    {
        public const string Completion = "textDocument/completion";
    }

    [Parallel, Method(Completion)]
    public interface ICompletionHandler : IRequestHandler<TextDocumentPositionParams, CompletionList>, IRegistration<CompletionRegistrationOptions>, ICapability<CompletionCapability> { }
}
