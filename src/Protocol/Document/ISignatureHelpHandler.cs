using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    using static DocumentNames;
    public static partial class DocumentNames
    {
        public const string SignatureHelp = "textDocument/signatureHelp";
    }

    [Parallel, Method(SignatureHelp)]
    public interface ISignatureHelpHandler : IRequestHandler<TextDocumentPositionParams, SignatureHelp>, IRegistration<SignatureHelpRegistrationOptions>, ICapability<SignatureHelpCapability> { }
}
