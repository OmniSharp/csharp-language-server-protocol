using OmniSharp.Extensions.LanguageServer;
using OmniSharp.Extensions.LanguageServer.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public static class PublishDiagnosticsExtensions
    {
        public static void PublishDiagnostics(this ILanguageServer mediator, PublishDiagnosticsParams @params)
        {
            mediator.SendNotification("textDocument/publishDiagnostics", @params);
        }
    }
}