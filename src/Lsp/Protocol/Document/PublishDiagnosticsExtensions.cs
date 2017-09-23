using OmniSharp.Extensions.LanguageServerProtocol;
using OmniSharp.Extensions.LanguageServerProtocol.Models;

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