using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServerProtocol.Models;

// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    [Method("$/cancelRequest")]
    public interface ICancelRequestHandler : INotificationHandler<CancelParams> { }
}