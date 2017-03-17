using System.Threading.Tasks;
using JsonRPC;
using Lsp.Models;
// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    public static class PublishDiagnosticsExtensions
    {
        public static Task PublishDiagnostics(this IClient mediator, PublishDiagnosticsParams @params)
        {
            return mediator.SendNotification("textDocument/publishDiagnostics", @params);
        }
    }
}