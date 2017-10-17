using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc.Server;
using OmniSharp.Extensions.LanguageServer;
using OmniSharp.Extensions.LanguageServer.Abstractions;
using OmniSharp.Extensions.LanguageServer.Messages;
using OmniSharp.Extensions.LanguageServer.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using Xunit;
using Xunit.Sdk;
using HandlerCollection = OmniSharp.Extensions.LanguageServer.HandlerCollection;

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

    public class MediatorTestsRequestHandlerOfTRequestTResponse
    {
        [Fact]
        public async Task RequestsCancellation()
        {
            var textDocumentSyncHandler = TextDocumentSyncHandlerExtensions.With(DocumentSelector.ForPattern("**/*.cs"));
            textDocumentSyncHandler.Handle(Arg.Any<DidSaveTextDocumentParams>()).Returns(Task.CompletedTask);

            var codeActionHandler = Substitute.For<ICodeActionHandler>();
            codeActionHandler.GetRegistrationOptions().Returns(new TextDocumentRegistrationOptions() { DocumentSelector = DocumentSelector.ForPattern("**/*.cs") });
            codeActionHandler
                .Handle(Arg.Any<CodeActionParams>(), Arg.Any<CancellationToken>())
                .Returns(async (c) => {
                    await Task.Delay(1000, c.Arg<CancellationToken>());
                    throw new XunitException("Task was not cancelled in time!");
                    return new CommandContainer();
                });

            var collection = new HandlerCollection { textDocumentSyncHandler, codeActionHandler };
            var mediator = new LspRequestRouter(collection);

            var id = Guid.NewGuid().ToString();
            var @params = new CodeActionParams() {
                TextDocument = new TextDocumentIdentifier(new Uri("file:///c:/test/123.cs")),
                Range = new Range(new Position(1, 1), new Position(2, 2)),
                Context = new CodeActionContext() {
                    Diagnostics = new Container<Diagnostic>()
                }
            };

            var request = new Request(id, "textDocument/codeAction", JObject.Parse(JsonConvert.SerializeObject(@params)));

            var response = mediator.RouteRequest(request);
            mediator.CancelRequest(id);
            var result = await response;

            result.IsError.Should().BeTrue();
            result.Error.ShouldBeEquivalentTo(new RequestCancelled());
        }

    }
}
