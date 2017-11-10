using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc.Server;
using OmniSharp.Extensions.LanguageServer;
using OmniSharp.Extensions.LanguageServer.Abstractions;
using OmniSharp.Extensions.LanguageServer.Matchers;
using OmniSharp.Extensions.LanguageServer.Messages;
using OmniSharp.Extensions.LanguageServer.Models;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;
using HandlerCollection = OmniSharp.Extensions.LanguageServer.HandlerCollection;

namespace Lsp.Tests
{
    public class LspRequestRouterTests
    {
        private readonly TestLoggerFactory _testLoggerFactory;
        private readonly IHandlerMatcherCollection _handlerMatcherCollection = new HandlerMatcherCollection();

        public LspRequestRouterTests(ITestOutputHelper testOutputHelper)
        {
            _testLoggerFactory = new TestLoggerFactory(testOutputHelper);
            var logger = Substitute.For<ILogger>();
            var matcher = Substitute.For<IHandlerMatcher>();
            matcher.FindHandler(Arg.Any<object>(), Arg.Any<IEnumerable<ILspHandlerDescriptor>>())
                .Returns(new List<HandlerDescriptor>());

            _handlerMatcherCollection.Add(matcher);
        }

        [Fact]
        public async Task ShouldRouteToCorrect_Notification()
        {
            var textDocumentSyncHandler = TextDocumentSyncHandlerExtensions.With(DocumentSelector.ForPattern("**/*.cs"));
            textDocumentSyncHandler.Handle(Arg.Any<DidSaveTextDocumentParams>()).Returns(Task.CompletedTask);

            var collection = new HandlerCollection { textDocumentSyncHandler };
            var mediator = new LspRequestRouter(collection, _testLoggerFactory, _handlerMatcherCollection);

            var @params = new DidSaveTextDocumentParams() {
                TextDocument = new TextDocumentIdentifier(new Uri("file:///c:/test/123.cs"))
            };

            var request = new Notification("textDocument/didSave", JObject.Parse(JsonConvert.SerializeObject(@params)));

            await mediator.RouteNotification(mediator.GetDescriptor(request), request);

            await textDocumentSyncHandler.Received(1).Handle(Arg.Any<DidSaveTextDocumentParams>());
        }

        [Fact]
        public async Task ShouldRouteToCorrect_Notification_WithManyHandlers()
        {
            var textDocumentSyncHandler = TextDocumentSyncHandlerExtensions.With(DocumentSelector.ForPattern("**/*.cs"));
            var textDocumentSyncHandler2 = TextDocumentSyncHandlerExtensions.With(DocumentSelector.ForPattern("**/*.cake"));
            textDocumentSyncHandler.Handle(Arg.Any<DidSaveTextDocumentParams>()).Returns(Task.CompletedTask);
            textDocumentSyncHandler2.Handle(Arg.Any<DidSaveTextDocumentParams>()).Returns(Task.CompletedTask);

            var collection = new HandlerCollection { textDocumentSyncHandler, textDocumentSyncHandler2 };
            var mediator = new LspRequestRouter(collection, _testLoggerFactory, _handlerMatcherCollection);

            var @params = new DidSaveTextDocumentParams() {
                TextDocument = new TextDocumentIdentifier(new Uri("file:///c:/test/123.cake"))
            };

            var request = new Notification("textDocument/didSave", JObject.Parse(JsonConvert.SerializeObject(@params)));

            await mediator.RouteNotification(mediator.GetDescriptor(request), request);

            await textDocumentSyncHandler.Received(1).Handle(Arg.Any<DidSaveTextDocumentParams>());
            await textDocumentSyncHandler2.Received(0).Handle(Arg.Any<DidSaveTextDocumentParams>());
        }

        [Fact]
        public async Task ShouldRouteToCorrect_Request()
        {
            var textDocumentSyncHandler = TextDocumentSyncHandlerExtensions.With(DocumentSelector.ForPattern("**/*.cs"));
            textDocumentSyncHandler.Handle(Arg.Any<DidSaveTextDocumentParams>()).Returns(Task.CompletedTask);

            var codeActionHandler = Substitute.For<ICodeActionHandler>();
            codeActionHandler.GetRegistrationOptions().Returns(new TextDocumentRegistrationOptions() { DocumentSelector = DocumentSelector.ForPattern("**/*.cs") });
            codeActionHandler
                .Handle(Arg.Any<CodeActionParams>(), Arg.Any<CancellationToken>())
                .Returns(new CommandContainer());

            var collection = new HandlerCollection { textDocumentSyncHandler, codeActionHandler };
            var mediator = new LspRequestRouter(collection, _testLoggerFactory, _handlerMatcherCollection);

            var id = Guid.NewGuid().ToString();
            var @params = new DidSaveTextDocumentParams() {
                TextDocument = new TextDocumentIdentifier(new Uri("file:///c:/test/123.cs"))
            };

            var request = new Request(id, "textDocument/codeAction", JObject.Parse(JsonConvert.SerializeObject(@params)));

            await mediator.RouteRequest(mediator.GetDescriptor(request), request);

            await codeActionHandler.Received(1).Handle(Arg.Any<CodeActionParams>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task ShouldRouteToCorrect_Request_WithManyHandlers()
        {
            var textDocumentSyncHandler = TextDocumentSyncHandlerExtensions.With(DocumentSelector.ForPattern("**/*.cs"));
            var textDocumentSyncHandler2 = TextDocumentSyncHandlerExtensions.With(DocumentSelector.ForPattern("**/*.cake"));
            textDocumentSyncHandler.Handle(Arg.Any<DidSaveTextDocumentParams>()).Returns(Task.CompletedTask);
            textDocumentSyncHandler2.Handle(Arg.Any<DidSaveTextDocumentParams>()).Returns(Task.CompletedTask);

            var codeActionHandler = Substitute.For<ICodeActionHandler>();
            codeActionHandler.GetRegistrationOptions().Returns(new TextDocumentRegistrationOptions() { DocumentSelector = DocumentSelector.ForPattern("**/*.cs") });
            codeActionHandler
                .Handle(Arg.Any<CodeActionParams>(), Arg.Any<CancellationToken>())
                .Returns(new CommandContainer());

            var codeActionHandler2 = Substitute.For<ICodeActionHandler>();
            codeActionHandler2.GetRegistrationOptions().Returns(new TextDocumentRegistrationOptions() { DocumentSelector = DocumentSelector.ForPattern("**/*.cake") });
            codeActionHandler2
                .Handle(Arg.Any<CodeActionParams>(), Arg.Any<CancellationToken>())
                .Returns(new CommandContainer());

            var collection = new HandlerCollection { textDocumentSyncHandler, textDocumentSyncHandler2, codeActionHandler, codeActionHandler2 };
            var mediator = new LspRequestRouter(collection, _testLoggerFactory, _handlerMatcherCollection);

            var id = Guid.NewGuid().ToString();
            var @params = new DidSaveTextDocumentParams() {
                TextDocument = new TextDocumentIdentifier(new Uri("file:///c:/test/123.cake"))
            };

            var request = new Request(id, "textDocument/codeAction", JObject.Parse(JsonConvert.SerializeObject(@params)));

            await mediator.RouteRequest(mediator.GetDescriptor(request), request);

            await codeActionHandler.Received(1).Handle(Arg.Any<CodeActionParams>(), Arg.Any<CancellationToken>());
            await codeActionHandler2.Received(0).Handle(Arg.Any<CodeActionParams>(), Arg.Any<CancellationToken>());
        }
    }
}
