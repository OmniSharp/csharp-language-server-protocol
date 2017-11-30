using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    using static DocumentNames;
    public static partial class DocumentNames
    {
        public const string Definition = "textDocument/definition";
    }

    [Parallel, Method(Definition)]
    public interface IDefinitionHandler : IRequestHandler<TextDocumentPositionParams, LocationOrLocations>, IRegistration<TextDocumentRegistrationOptions>, ICapability<DefinitionCapability> { }
}
