using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public static partial class DocumentNames
    {
        public const string PublishDiagnostics = "textDocument/publishDiagnostics";
    }

    public static class PublishDiagnosticsExtensions
    {
        public static void PublishDiagnostics(this IResponseRouter mediator, PublishDiagnosticsParams @params)
        {
            mediator.SendNotification(DocumentNames.PublishDiagnostics, @params);
        }
    }
}
