using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    using static DocumentNames;
    public static partial class DocumentNames
    {
        public const string Rename = "textDocument/rename";
    }

    [Serial, Method(Rename)]
    public interface IRenameHandler : IJsonRpcRequestHandler<RenameParams, WorkspaceEdit>, IRegistration<TextDocumentRegistrationOptions>, ICapability<RenameCapability> { }
}
