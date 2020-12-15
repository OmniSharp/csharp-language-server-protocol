using NSubstitute;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Lsp.Tests
{
    internal static class TextDocumentSyncHandlerExtensions
    {
        public static ITextDocumentSyncHandler With(DocumentSelector? documentSelector, string language) =>
            Substitute.For<ITextDocumentSyncHandler>().With(documentSelector, language);

        public static ITextDocumentSyncHandler With(this ITextDocumentSyncHandler handler, DocumentSelector? documentSelector, string language)
        {
            ( (IDidChangeTextDocumentHandler) handler ).GetRegistrationOptions(Arg.Any<SynchronizationCapability>(), Arg.Any<ClientCapabilities>()).Returns(new TextDocumentChangeRegistrationOptions() { DocumentSelector = documentSelector });
            ( (IDidOpenTextDocumentHandler) handler ).GetRegistrationOptions(Arg.Any<SynchronizationCapability>(), Arg.Any<ClientCapabilities>()).Returns(new TextDocumentOpenRegistrationOptions() { DocumentSelector = documentSelector });
            ( (IDidCloseTextDocumentHandler) handler ).GetRegistrationOptions(Arg.Any<SynchronizationCapability>(), Arg.Any<ClientCapabilities>()).Returns(new TextDocumentCloseRegistrationOptions() { DocumentSelector = documentSelector });
            ( (IDidSaveTextDocumentHandler) handler ).GetRegistrationOptions(Arg.Any<SynchronizationCapability>(), Arg.Any<ClientCapabilities>()).Returns(new TextDocumentSaveRegistrationOptions() { DocumentSelector = documentSelector });
            handler.GetTextDocumentAttributes(Arg.Any<DocumentUri>())
                   .Returns(c => new TextDocumentAttributes(c.Arg<DocumentUri>(), language));

            handler
               .GetTextDocumentAttributes(Arg.Is<DocumentUri>(x => documentSelector.IsMatch(new TextDocumentAttributes(x, language))))
               .Returns(c => new TextDocumentAttributes(c.Arg<DocumentUri>(), language));

            return handler;
        }
    }
}
