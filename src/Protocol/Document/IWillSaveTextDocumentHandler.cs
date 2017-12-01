using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    using static DocumentNames;
    public static partial class DocumentNames
    {
        public const string WillSave = "textDocument/willSave";
    }

    [Parallel, Method(WillSave)]
    public interface IWillSaveTextDocumentHandler : INotificationHandler<WillSaveTextDocumentParams>, IRegistration<TextDocumentRegistrationOptions>, ICapability<SynchronizationCapability> { }
}
