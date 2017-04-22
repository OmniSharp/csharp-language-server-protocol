using System.Threading.Tasks;
using JsonRpc;
using Lsp.Models;
// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    public static class PublishDiagnosticsExtensions
    {
        public static Task PublishDiagnostics(this IOutgoingRequestRouter mediator, PublishDiagnosticsParams @params)
        {
            return mediator.SendNotification("textDocument/publishDiagnostics", @params);
        }
    }
}