using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Abstractions;
using OmniSharp.Extensions.LanguageServer.Capabilities.Client;
using OmniSharp.Extensions.LanguageServer.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    [Serial, Method("textDocument/willSaveWaitUntil")]
    public interface IWillSaveWaitUntilTextDocumentHandler : IRequestHandler<WillSaveTextDocumentParams>, IRegistration<TextDocumentRegistrationOptions>, ICapability<SynchronizationCapability> { }
}
