using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.Embedded.MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Server;
using OmniSharp.Extensions.LanguageServer;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Server;
using OmniSharp.Extensions.LanguageServer.Server.Abstractions;
using OmniSharp.Extensions.LanguageServer.Server.Handlers;
using OmniSharp.Extensions.LanguageServer.Server.Matchers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;
using HandlerCollection = OmniSharp.Extensions.LanguageServer.Server.HandlerCollection;
using ISerializer = OmniSharp.Extensions.LanguageServer.Protocol.Serialization.ISerializer;
using Serializer = OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Serializer;

namespace Lsp.Tests
{
    public class TestLanguageServerRegistry : ILanguageServerRegistry
    {
        internal List<IJsonRpcHandler> Handlers = new List<IJsonRpcHandler>();
        public IDisposable AddHandler(string method, IJsonRpcHandler handler)
        {
            throw new NotImplementedException();
        }

        public IDisposable AddHandler<T>() where T : IJsonRpcHandler
        {
            throw new NotImplementedException();
        }

        public IDisposable AddHandlers(params IJsonRpcHandler[] handlers)
        {
            Handlers.AddRange(handlers);
            return new Disposable(() => { });
        }
    }
    public class LspRequestRouterTests : AutoTestBase
    {
        public LspRequestRouterTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            Services
                .AddJsonRpcMediatR(new[] { typeof(LspRequestRouterTests).Assembly })
                .AddSingleton<ISerializer>(new Serializer(ClientVersion.Lsp3));
            Services.AddTransient<IHandlerMatcher, TextDocumentMatcher>();
        }

        [Fact]
        public async Task ShouldRouteToCorrect_Notification()
        {
            var textDocumentSyncHandler = TextDocumentSyncHandlerExtensions.With(DocumentSelector.ForPattern("**/*.cs"));
            textDocumentSyncHandler.Handle(Arg.Any<DidSaveTextDocumentParams>(), Arg.Any<CancellationToken>()).Returns(Unit.Value);

            var collection = new HandlerCollection(SupportedCapabilitiesFixture.AlwaysTrue) { textDocumentSyncHandler };
            AutoSubstitute.Provide<IHandlerCollection>(collection);
            var mediator = AutoSubstitute.Resolve<LspRequestRouter>();

            var @params = new DidSaveTextDocumentParams()
            {
                TextDocument = new TextDocumentIdentifier(new Uri("file:///c:/test/123.cs"))
            };

            var request = new Notification(DocumentNames.DidSave, JObject.Parse(JsonConvert.SerializeObject(@params, new Serializer(ClientVersion.Lsp3).Settings)));

            await mediator.RouteNotification(mediator.GetDescriptor(request), request);

            await textDocumentSyncHandler.Received(1).Handle(Arg.Any<DidSaveTextDocumentParams>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task ShouldRouteToCorrect_Notification_WithManyHandlers()
        {
            var textDocumentSyncHandler = TextDocumentSyncHandlerExtensions.With(DocumentSelector.ForPattern("**/*.cs"));
            var textDocumentSyncHandler2 = TextDocumentSyncHandlerExtensions.With(DocumentSelector.ForPattern("**/*.cake"));
            textDocumentSyncHandler.Handle(Arg.Any<DidSaveTextDocumentParams>(), Arg.Any<CancellationToken>()).Returns(Unit.Value);
            textDocumentSyncHandler2.Handle(Arg.Any<DidSaveTextDocumentParams>(), Arg.Any<CancellationToken>()).Returns(Unit.Value);

            var collection = new HandlerCollection(SupportedCapabilitiesFixture.AlwaysTrue) { textDocumentSyncHandler, textDocumentSyncHandler2 };
            AutoSubstitute.Provide<IHandlerCollection>(collection);
            var mediator = AutoSubstitute.Resolve<LspRequestRouter>();

            var @params = new DidSaveTextDocumentParams()
            {
                TextDocument = new TextDocumentIdentifier(new Uri("file:///c:/test/123.cake"))
            };

            var request = new Notification(DocumentNames.DidSave, JObject.Parse(JsonConvert.SerializeObject(@params, new Serializer(ClientVersion.Lsp3).Settings)));

            await mediator.RouteNotification(mediator.GetDescriptor(request), request);

            await textDocumentSyncHandler.Received(0).Handle(Arg.Any<DidSaveTextDocumentParams>(), Arg.Any<CancellationToken>());
            await textDocumentSyncHandler2.Received(1).Handle(Arg.Any<DidSaveTextDocumentParams>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task ShouldRouteToCorrect_Request()
        {
            var textDocumentSyncHandler = TextDocumentSyncHandlerExtensions.With(DocumentSelector.ForPattern("**/*.cs"));
            textDocumentSyncHandler.Handle(Arg.Any<DidSaveTextDocumentParams>(), Arg.Any<CancellationToken>()).Returns(Unit.Value);

            var codeActionHandler = Substitute.For<ICodeActionHandler>();
            codeActionHandler.GetRegistrationOptions().Returns(new TextDocumentRegistrationOptions() { DocumentSelector = DocumentSelector.ForPattern("**/*.cs") });
            codeActionHandler
                .Handle(Arg.Any<CodeActionParams>(), Arg.Any<CancellationToken>())
                .Returns(new CommandOrCodeActionContainer());

            var collection = new HandlerCollection(SupportedCapabilitiesFixture.AlwaysTrue) { textDocumentSyncHandler, codeActionHandler };
            AutoSubstitute.Provide<IHandlerCollection>(collection);
            var mediator = AutoSubstitute.Resolve<LspRequestRouter>();

            var id = Guid.NewGuid().ToString();
            var @params = new DidSaveTextDocumentParams()
            {
                TextDocument = new TextDocumentIdentifier(new Uri("file:///c:/test/123.cs"))
            };

            var request = new Request(id, DocumentNames.CodeAction, JObject.Parse(JsonConvert.SerializeObject(@params, new Serializer(ClientVersion.Lsp3).Settings)));

            await mediator.RouteRequest(mediator.GetDescriptor(request), request);

            await codeActionHandler.Received(1).Handle(Arg.Any<CodeActionParams>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task ShouldRouteToCorrect_Request_WithManyHandlers()
        {

            var textDocumentSyncHandler = TextDocumentSyncHandlerExtensions.With(DocumentSelector.ForPattern("**/*.cs"));
            var textDocumentSyncHandler2 = TextDocumentSyncHandlerExtensions.With(DocumentSelector.ForPattern("**/*.cake"));
            textDocumentSyncHandler.Handle(Arg.Any<DidSaveTextDocumentParams>(), Arg.Any<CancellationToken>()).Returns(Unit.Value);
            textDocumentSyncHandler2.Handle(Arg.Any<DidSaveTextDocumentParams>(), Arg.Any<CancellationToken>()).Returns(Unit.Value);

            var codeActionHandler = Substitute.For<ICodeActionHandler>();
            codeActionHandler.GetRegistrationOptions().Returns(new TextDocumentRegistrationOptions() { DocumentSelector = DocumentSelector.ForPattern("**/*.cs") });
            codeActionHandler
                .Handle(Arg.Any<CodeActionParams>(), Arg.Any<CancellationToken>())
                .Returns(new CommandOrCodeActionContainer());

            var registry = new TestLanguageServerRegistry();
            var codeActionDelegate = Substitute.For<Func<CodeActionParams, CancellationToken, Task<CommandOrCodeActionContainer>>>();
            codeActionDelegate.Invoke(Arg.Any<CodeActionParams>(), Arg.Any<CancellationToken>())
                .Returns(new CommandOrCodeActionContainer());
            registry.OnCodeAction(
                codeActionDelegate,
                new TextDocumentRegistrationOptions() { DocumentSelector = DocumentSelector.ForPattern("**/*.cake") }
            );

            var handlerCollection = new HandlerCollection(SupportedCapabilitiesFixture.AlwaysTrue) { textDocumentSyncHandler, textDocumentSyncHandler2, codeActionHandler };
            handlerCollection.Add(registry.Handlers);
            AutoSubstitute.Provide<IHandlerCollection>(handlerCollection);
            var mediator = AutoSubstitute.Resolve<LspRequestRouter>();

            var id = Guid.NewGuid().ToString();
            var @params = new CodeActionParams()
            {
                TextDocument = new TextDocumentIdentifier(new Uri("file:///c:/test/123.cake"))
            };

            var request = new Request(id, DocumentNames.CodeAction, JObject.Parse(JsonConvert.SerializeObject(@params, new Serializer(ClientVersion.Lsp3).Settings)));

            await mediator.RouteRequest(mediator.GetDescriptor(request), request);

            await codeActionHandler.Received(0).Handle(Arg.Any<CodeActionParams>(), Arg.Any<CancellationToken>());
            await codeActionDelegate.Received(1).Invoke(Arg.Any<CodeActionParams>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task ShouldRouteToCorrect_Request_WithManyHandlers_CodeLensHandler()
        {
            var textDocumentSyncHandler = TextDocumentSyncHandlerExtensions.With(DocumentSelector.ForPattern("**/*.cs"));
            var textDocumentSyncHandler2 = TextDocumentSyncHandlerExtensions.With(DocumentSelector.ForPattern("**/*.cake"));
            textDocumentSyncHandler.Handle(Arg.Any<DidSaveTextDocumentParams>(), Arg.Any<CancellationToken>()).Returns(Unit.Value);
            textDocumentSyncHandler2.Handle(Arg.Any<DidSaveTextDocumentParams>(), Arg.Any<CancellationToken>()).Returns(Unit.Value);

            var codeActionHandler = Substitute.For<ICodeLensHandler>();
            codeActionHandler.GetRegistrationOptions().Returns(new CodeLensRegistrationOptions() { DocumentSelector = DocumentSelector.ForPattern("**/*.cs") });
            codeActionHandler
                .Handle(Arg.Any<CodeLensParams>(), Arg.Any<CancellationToken>())
                .Returns(new CodeLensContainer());

            var codeActionHandler2 = Substitute.For<ICodeLensHandler>();
            codeActionHandler2.GetRegistrationOptions().Returns(new CodeLensRegistrationOptions() { DocumentSelector = DocumentSelector.ForPattern("**/*.cake") });
            codeActionHandler2
                .Handle(Arg.Any<CodeLensParams>(), Arg.Any<CancellationToken>())
                .Returns(new CodeLensContainer());

            var collection = new HandlerCollection(SupportedCapabilitiesFixture.AlwaysTrue) { textDocumentSyncHandler, textDocumentSyncHandler2, codeActionHandler, codeActionHandler2 };
            AutoSubstitute.Provide<IHandlerCollection>(collection);
            var mediator = AutoSubstitute.Resolve<LspRequestRouter>();

            var id = Guid.NewGuid().ToString();
            var @params = new CodeLensParams()
            {
                TextDocument = new TextDocumentIdentifier(new Uri("file:///c:/test/123.cs"))
            };

            var request = new Request(id, DocumentNames.CodeLens, JObject.Parse(JsonConvert.SerializeObject(@params, new Serializer(ClientVersion.Lsp3).Settings)));

            await mediator.RouteRequest(mediator.GetDescriptor(request), request);

            await codeActionHandler2.Received(0).Handle(Arg.Any<CodeLensParams>(), Arg.Any<CancellationToken>());
            await codeActionHandler.Received(1).Handle(Arg.Any<CodeLensParams>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task ShouldRouteTo_CorrectRequestWhenGivenNullParams()
        {
            var handler = Substitute.For<IShutdownHandler>();
            handler
                .Handle(Arg.Any<EmptyRequest>(), Arg.Any<CancellationToken>())
                .Returns(Unit.Value);

            var collection = new HandlerCollection(SupportedCapabilitiesFixture.AlwaysTrue) { handler };
            AutoSubstitute.Provide<IHandlerCollection>(collection);
            var mediator = AutoSubstitute.Resolve<LspRequestRouter>();

            var id = Guid.NewGuid().ToString();
            var request = new Request(id, GeneralNames.Shutdown, new JObject());

            await mediator.RouteRequest(mediator.GetDescriptor(request), request);

            await handler.Received(1).Handle(Arg.Any<EmptyRequest>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task ShouldHandle_Request_WithNullParameters()
        {
            bool wasShutDown = false;

            var shutdownHandler = new ServerShutdownHandler();
            shutdownHandler.Shutdown.Subscribe(shutdownRequested =>
            {
                wasShutDown = true;
            });

            var collection = new HandlerCollection(SupportedCapabilitiesFixture.AlwaysTrue) { shutdownHandler };
            AutoSubstitute.Provide<IHandlerCollection>(collection);
            var mediator = AutoSubstitute.Resolve<LspRequestRouter>();

            JToken @params = JValue.CreateNull(); // If the "params" property present but null, this will be JTokenType.Null.

            var id = Guid.NewGuid().ToString();
            var request = new Request(id, GeneralNames.Shutdown, @params);

            await mediator.RouteRequest(mediator.GetDescriptor(request), request);

            Assert.True(wasShutDown, "WasShutDown");
        }

        [Fact]
        public async Task ShouldHandle_Request_WithMissingParameters()
        {
            bool wasShutdown = false;
            var shutdownHandler = new ServerShutdownHandler();
            shutdownHandler.Shutdown.Subscribe(shutdownRequested =>
            {
                wasShutdown = true;
            });

            var collection = new HandlerCollection(SupportedCapabilitiesFixture.AlwaysTrue) { shutdownHandler };
            AutoSubstitute.Provide<IHandlerCollection>(collection);
            var mediator = AutoSubstitute.Resolve<LspRequestRouter>();

            JToken @params = null; // If the "params" property was missing entirely, this will be null.

            var id = Guid.NewGuid().ToString();
            var request = new Request(id, GeneralNames.Shutdown, @params);

            await mediator.RouteRequest(mediator.GetDescriptor(request), request);

            Assert.True(shutdownHandler.ShutdownRequested, "WasShutDown");
        }
    }
}
