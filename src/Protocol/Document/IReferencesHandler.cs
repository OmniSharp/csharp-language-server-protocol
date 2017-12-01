using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    using static DocumentNames;
    public static partial class DocumentNames
    {
        public const string References = "textDocument/references";
    }

    [Parallel, Method(References)]
    public interface IReferencesHandler : IRequestHandler<ReferenceParams, LocationContainer>, IRegistration<TextDocumentRegistrationOptions>, ICapability<ReferencesCapability> { }
}
