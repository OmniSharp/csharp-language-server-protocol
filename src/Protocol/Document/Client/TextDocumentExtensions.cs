using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public static class TextDocumentExtensions
    {
        public static void DidChangeTextDocument(this ILanguageClientDocument mediator, DidChangeTextDocumentParams @params)
        {
            mediator.SendNotification(DocumentNames.DidChange, @params);
        }

        public static void DidOpenTextDocument(this ILanguageClientDocument mediator, DidOpenTextDocumentParams @params)
        {
            mediator.SendNotification(DocumentNames.DidOpen, @params);
        }

        public static void DidSaveTextDocument(this ILanguageClientDocument mediator, DidSaveTextDocumentParams @params)
        {
            mediator.SendNotification(DocumentNames.DidSave, @params);
        }

        public static void DidCloseTextDocument(this ILanguageClientDocument mediator, DidCloseTextDocumentParams @params)
        {
            mediator.SendNotification(DocumentNames.DidClose, @params);
        }
    }
}
