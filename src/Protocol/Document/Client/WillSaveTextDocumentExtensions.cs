using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public static class WillSaveTextDocumentExtensions
    {
        public static void WillSaveTextDocument(this ILanguageClientDocument mediator, WillSaveTextDocumentParams @params)
        {
            mediator.SendNotification(DocumentNames.WillSave, @params);
        }
    }
}
