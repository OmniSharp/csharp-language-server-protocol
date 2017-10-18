using System;
using NSubstitute;
using OmniSharp.Extensions.LanguageServer.Abstractions;
using OmniSharp.Extensions.LanguageServer.Models;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;

namespace Lsp.Tests
{
    static class TextDocumentSyncHandlerExtensions
    {
        public static ITextDocumentSyncHandler With(DocumentSelector documentSelector)
        {
            return Substitute.For<ITextDocumentSyncHandler>().With(documentSelector);
        }

        public static ITextDocumentSyncHandler With(this ITextDocumentSyncHandler handler, DocumentSelector documentSelector)
        {
            ((IDidChangeTextDocumentHandler)handler).GetRegistrationOptions().Returns(new TextDocumentChangeRegistrationOptions() { DocumentSelector = documentSelector });
            ((IDidOpenTextDocumentHandler)handler).GetRegistrationOptions().Returns(new TextDocumentRegistrationOptions() { DocumentSelector = documentSelector });
            ((IDidCloseTextDocumentHandler)handler).GetRegistrationOptions().Returns(new TextDocumentRegistrationOptions() { DocumentSelector = documentSelector });
            ((IDidSaveTextDocumentHandler)handler).GetRegistrationOptions().Returns(new TextDocumentSaveRegistrationOptions() { DocumentSelector = documentSelector });

            handler
                .GetTextDocumentAttributes(Arg.Is<Uri>(x => documentSelector.IsMatch(new TextDocumentAttributes(x, ""))))
                .Returns(c => new TextDocumentAttributes(c.Arg<Uri>(), ""));

            return handler;
        }

        private static void For<T>(this ITextDocumentSyncHandler handler, DocumentSelector documentSelector)
            where T : class, IRegistration<TextDocumentRegistrationOptions>
        {
            var me = handler as T;
            me.GetRegistrationOptions().Returns(GetOptions(me, documentSelector));
        }

        private static TextDocumentRegistrationOptions GetOptions<R>(IRegistration<R> handler, DocumentSelector documentSelector)
            where R : TextDocumentRegistrationOptions, new()
        {
            return new R { DocumentSelector = documentSelector };
        }
    }
}