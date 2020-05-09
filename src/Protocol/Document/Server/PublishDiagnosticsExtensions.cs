using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    public static class PublishDiagnosticsExtensions
    {
        public static void PublishDiagnostics(this ILanguageServerDocument mediator, PublishDiagnosticsParams @params)
        {
            mediator.SendNotification(DocumentNames.PublishDiagnostics, @params);
        }
    }
}
