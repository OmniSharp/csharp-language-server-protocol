using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Server;
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
using HandlerCollection = OmniSharp.Extensions.LanguageServer.Server.HandlerCollection;
using ISerializer = OmniSharp.Extensions.LanguageServer.Protocol.Serialization.ISerializer;
using Serializer = OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Serializer;
using System.Reactive.Disposables;

namespace Lsp.Tests
{
    public class TestLanguageServerRegistry : ILanguageServerRegistry
    {
        internal List<IJsonRpcHandler> Handlers = new List<IJsonRpcHandler>();

        public OmniSharp.Extensions.JsonRpc.ISerializer Serializer => new Serializer();

        public ProgressManager ProgressManager { get; } = new ProgressManager();

        public IDisposable AddHandler(string method, IJsonRpcHandler handler)
        {
            throw new NotImplementedException();
        }

        public IDisposable AddHandler<T>() where T : IJsonRpcHandler
        {
            throw new NotImplementedException();
        }

        public IDisposable AddHandler(string method, Func<IServiceProvider, IJsonRpcHandler> handlerFunc)
        {
            throw new NotImplementedException();
        }

        public IDisposable AddHandlers(params IJsonRpcHandler[] handlers)
        {
            Handlers.AddRange(handlers);
            return Disposable.Empty;
        }

        public IDisposable AddTextDocumentIdentifier(params ITextDocumentIdentifier[] handlers) => throw new NotImplementedException();
        public IDisposable AddTextDocumentIdentifier<T>() where T : ITextDocumentIdentifier => throw new NotImplementedException();
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
            var textDocumentSyncHandler = TextDocumentSyncHandlerExtensions.With(DocumentSelector.ForPattern("**/*.cs"), "csharp");
            textDocumentSyncHandler.Handle(Arg.Any<DidSaveTextDocumentParams>(), Arg.Any<CancellationToken>()).Returns(Unit.Value);

            var collection = new HandlerCollection(SupportedCapabilitiesFixture.AlwaysTrue, new TextDocumentIdentifiers()) { textDocumentSyncHandler };
            AutoSubstitute.Provide<IHandlerCollection>(collection);
            AutoSubstitute.Provide<IEnumerable<ILspHandlerDescriptor>>(collection);
            var mediator = AutoSubstitute.Resolve<LspRequestRouter>();

            var @params = new DidSaveTextDocumentParams()
            {
                TextDocument = new TextDocumentIdentifier(new Uri("file:///c:/test/123.cs"))
            };

            var request = new Notification(DocumentNames.DidSave, @params);

            await mediator.RouteNotification(mediator.GetDescriptor(request), request, CancellationToken.None);

            await textDocumentSyncHandler.Received(1).Handle(Arg.Any<DidSaveTextDocumentParams>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task ShouldRouteToCorrect_Notification_WithManyHandlers()
        {
            var textDocumentSyncHandler = TextDocumentSyncHandlerExtensions.With(DocumentSelector.ForPattern("**/*.cs"), "csharp");
            var textDocumentSyncHandler2 = TextDocumentSyncHandlerExtensions.With(DocumentSelector.ForPattern("**/*.cake"), "csharp");
            textDocumentSyncHandler.Handle(Arg.Any<DidSaveTextDocumentParams>(), Arg.Any<CancellationToken>()).Returns(Unit.Value);
            textDocumentSyncHandler2.Handle(Arg.Any<DidSaveTextDocumentParams>(), Arg.Any<CancellationToken>())
                .Returns(Unit.Value);

            var textDocumentIdentifiers = new TextDocumentIdentifiers();
            AutoSubstitute.Provide(textDocumentIdentifiers);
            var collection = new HandlerCollection(SupportedCapabilitiesFixture.AlwaysTrue, textDocumentIdentifiers) { textDocumentSyncHandler, textDocumentSyncHandler2 };
            AutoSubstitute.Provide<IHandlerCollection>(collection);
            AutoSubstitute.Provide<IEnumerable<ILspHandlerDescriptor>>(collection);
            var mediator = AutoSubstitute.Resolve<LspRequestRouter>();

            var @params = new DidSaveTextDocumentParams()
            {
                TextDocument = new TextDocumentIdentifier(new Uri("file:///c:/test/123.cake"))
            };

            var request = new Notification(DocumentNames.DidSave, @params);

            await mediator.RouteNotification(mediator.GetDescriptor(request), request, CancellationToken.None);

            await textDocumentSyncHandler.Received(0).Handle(Arg.Any<DidSaveTextDocumentParams>(), Arg.Any<CancellationToken>());
            await textDocumentSyncHandler2.Received(1).Handle(Arg.Any<DidSaveTextDocumentParams>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task ShouldRouteToCorrect_Request()
        {
            var textDocumentSyncHandler = TextDocumentSyncHandlerExtensions.With(DocumentSelector.ForPattern("**/*.cs"), "csharp");
            textDocumentSyncHandler.Handle(Arg.Any<DidSaveTextDocumentParams>(), Arg.Any<CancellationToken>()).Returns(Unit.Value);

            var codeActionHandler = Substitute.For<ICodeActionHandler>();
            codeActionHandler.GetRegistrationOptions().Returns(new CodeActionRegistrationOptions() { DocumentSelector = DocumentSelector.ForPattern("**/*.cs") });
            codeActionHandler
                .Handle(Arg.Any<CodeActionParams>(), Arg.Any<CancellationToken>())
                .Returns(new CommandOrCodeActionContainer());

            var collection = new HandlerCollection(SupportedCapabilitiesFixture.AlwaysTrue, new TextDocumentIdentifiers()) { textDocumentSyncHandler, codeActionHandler };
            AutoSubstitute.Provide<IHandlerCollection>(collection);
            AutoSubstitute.Provide<IEnumerable<ILspHandlerDescriptor>>(collection);
            var mediator = AutoSubstitute.Resolve<LspRequestRouter>();

            var id = Guid.NewGuid().ToString();
            var @params = new DidSaveTextDocumentParams()
            {
                TextDocument = new TextDocumentIdentifier(new Uri("file:///c:/test/123.cs"))
            };

            var request = new Request(id, DocumentNames.CodeAction, @params);

            await mediator.RouteRequest(mediator.GetDescriptor(request), request, CancellationToken.None);

            await codeActionHandler.Received(1).Handle(Arg.Any<CodeActionParams>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task ShouldRouteToCorrect_Request_WithManyHandlers()
        {

            var textDocumentSyncHandler = TextDocumentSyncHandlerExtensions.With(DocumentSelector.ForPattern("**/*.cs"), "csharp");
            var textDocumentSyncHandler2 = TextDocumentSyncHandlerExtensions.With(DocumentSelector.ForPattern("**/*.cake"), "csharp");
            textDocumentSyncHandler.Handle(Arg.Any<DidSaveTextDocumentParams>(), Arg.Any<CancellationToken>()).Returns(Unit.Value);
            textDocumentSyncHandler2.Handle(Arg.Any<DidSaveTextDocumentParams>(), Arg.Any<CancellationToken>()).Returns(Unit.Value);

            var codeActionHandler = Substitute.For<ICodeActionHandler>();
            codeActionHandler.GetRegistrationOptions().Returns(new CodeActionRegistrationOptions() { DocumentSelector = DocumentSelector.ForPattern("**/*.cs") });
            codeActionHandler
                .Handle(Arg.Any<CodeActionParams>(), Arg.Any<CancellationToken>())
                .Returns(new CommandOrCodeActionContainer());

            var registry = new TestLanguageServerRegistry();
            var codeActionDelegate = Substitute.For<Func<CodeActionParams, CancellationToken, Task<CommandOrCodeActionContainer>>>();
            codeActionDelegate.Invoke(Arg.Any<CodeActionParams>(), Arg.Any<CancellationToken>())
                .Returns(new CommandOrCodeActionContainer());
            registry.OnCodeAction(
                codeActionDelegate,
                new CodeActionRegistrationOptions() { DocumentSelector = DocumentSelector.ForPattern("**/*.cake") }
            );

            var textDocumentIdentifiers = new TextDocumentIdentifiers();
            AutoSubstitute.Provide(textDocumentIdentifiers);
            var handlerCollection = new HandlerCollection(SupportedCapabilitiesFixture.AlwaysTrue, textDocumentIdentifiers) { textDocumentSyncHandler, textDocumentSyncHandler2, codeActionHandler };
            handlerCollection.Add(registry.Handlers);
            AutoSubstitute.Provide<IHandlerCollection>(handlerCollection);
            AutoSubstitute.Provide<IEnumerable<ILspHandlerDescriptor>>(handlerCollection);
            var mediator = AutoSubstitute.Resolve<LspRequestRouter>();

            var id = Guid.NewGuid().ToString();
            var @params = new CodeActionParams()
            {
                TextDocument = new TextDocumentIdentifier(new Uri("file:///c:/test/123.cake"))
            };

            var request = new Request(id, DocumentNames.CodeAction, @params);

            await mediator.RouteRequest(mediator.GetDescriptor(request), request, CancellationToken.None);

            await codeActionHandler.Received(0).Handle(Arg.Any<CodeActionParams>(), Arg.Any<CancellationToken>());
            await codeActionDelegate.Received(1).Invoke(Arg.Any<CodeActionParams>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task ShouldRouteToCorrect_Request_WithManyHandlers_CodeLensHandler()
        {
            var textDocumentSyncHandler = TextDocumentSyncHandlerExtensions.With(DocumentSelector.ForPattern("**/*.cs"), "csharp");
            var textDocumentSyncHandler2 = TextDocumentSyncHandlerExtensions.With(DocumentSelector.ForPattern("**/*.cake"), "csharp");
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

            var collection = new HandlerCollection(SupportedCapabilitiesFixture.AlwaysTrue, new TextDocumentIdentifiers()) { textDocumentSyncHandler, textDocumentSyncHandler2, codeActionHandler, codeActionHandler2 };
            AutoSubstitute.Provide<IHandlerCollection>(collection);
            AutoSubstitute.Provide<IEnumerable<ILspHandlerDescriptor>>(collection);
            var mediator = AutoSubstitute.Resolve<LspRequestRouter>();

            var id = Guid.NewGuid().ToString();
            var @params = new CodeLensParams()
            {
                TextDocument = new TextDocumentIdentifier(new Uri("file:///c:/test/123.cs"))
            };

            var request = new Request(id, DocumentNames.CodeLens, @params);

            await mediator.RouteRequest(mediator.GetDescriptor(request), request, CancellationToken.None);

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

            var collection = new HandlerCollection(SupportedCapabilitiesFixture.AlwaysTrue, new TextDocumentIdentifiers()) { handler };
            AutoSubstitute.Provide<IHandlerCollection>(collection);
            AutoSubstitute.Provide<IEnumerable<ILspHandlerDescriptor>>(collection);
            var mediator = AutoSubstitute.Resolve<LspRequestRouter>();

            var id = Guid.NewGuid().ToString();
            var request = new Request(id, GeneralNames.Shutdown, new JObject());

            await mediator.RouteRequest(mediator.GetDescriptor(request), request, CancellationToken.None);

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

            var collection = new HandlerCollection(SupportedCapabilitiesFixture.AlwaysTrue, new TextDocumentIdentifiers()) { shutdownHandler };
            AutoSubstitute.Provide<IHandlerCollection>(collection);
            AutoSubstitute.Provide<IEnumerable<ILspHandlerDescriptor>>(collection);
            var mediator = AutoSubstitute.Resolve<LspRequestRouter>();

            JToken @params = JValue.CreateNull(); // If the "params" property present but null, this will be JTokenType.Null.

            var id = Guid.NewGuid().ToString();
            var request = new Request(id, GeneralNames.Shutdown, @params);

            await mediator.RouteRequest(mediator.GetDescriptor(request), request, CancellationToken.None);

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

            var collection = new HandlerCollection(SupportedCapabilitiesFixture.AlwaysTrue, new TextDocumentIdentifiers()) { shutdownHandler };
            AutoSubstitute.Provide<IHandlerCollection>(collection);
            AutoSubstitute.Provide<IEnumerable<ILspHandlerDescriptor>>(collection);
            var mediator = AutoSubstitute.Resolve<LspRequestRouter>();

            JToken @params = null; // If the "params" property was missing entirely, this will be null.

            var id = Guid.NewGuid().ToString();
            var request = new Request(id, GeneralNames.Shutdown, @params);

            await mediator.RouteRequest(mediator.GetDescriptor(request), request, CancellationToken.None);

            Assert.True(shutdownHandler.ShutdownRequested, "WasShutDown");
        }
    }
}
