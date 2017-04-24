using System.Threading.Tasks;
using JsonRpc;
using Lsp.Models;
// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    public static class PublishDiagnosticsExtensions
    {
        public static void PublishDiagnostics(this ILanguageServer mediator, PublishDiagnosticsParams @params)
        {
            mediator.SendNotification("textDocument/publishDiagnostics", @params);
        }
    }
}