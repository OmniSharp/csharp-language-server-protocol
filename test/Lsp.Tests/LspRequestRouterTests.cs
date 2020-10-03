using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DryIoc;
using JsonRpc.Tests;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Server;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.General;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.WorkDone;
using OmniSharp.Extensions.LanguageServer.Protocol.Shared;
using OmniSharp.Extensions.LanguageServer.Server;
using OmniSharp.Extensions.LanguageServer.Server.Matchers;
using OmniSharp.Extensions.LanguageServer.Shared;
using Xunit;
using Xunit.Abstractions;
using Arg = NSubstitute.Arg;
using ISerializer = OmniSharp.Extensions.JsonRpc.ISerializer;
using Request = OmniSharp.Extensions.JsonRpc.Server.Request;

namespace Lsp.Tests
{
    public class TestLanguageServerRegistry : JsonRpcOptionsRegistryBase<ILanguageServerRegistry>, ILanguageServerRegistry
    {
        internal List<IJsonRpcHandler> Handlers = new List<IJsonRpcHandler>();
        public ISerializer Serializer => new Serializer();

        public ILanguageServerRegistry AddTextDocumentIdentifier(params ITextDocumentIdentifier[] handlers) => this;

        public ILanguageServerRegistry AddTextDocumentIdentifier<T>() where T : ITextDocumentIdentifier => this;

        public ILanguageServerRegistry AddHandler<T>(Func<IServiceProvider, T> handlerFunc, JsonRpcHandlerOptions options = null) where T : IJsonRpcHandler
        {
            var sp = new ServiceCollection()
                    .AddSingleton(
                         Substitute
                            .For<Func<CodeActionParams, CancellationToken, Task<CommandOrCodeActionContainer>>>()
                     )
                    .AddSingleton(Substitute.For<IServerWorkDoneManager>())
                    .AddSingleton(Substitute.For<Action<CodeActionCapability>>())
                    .AddSingleton(new CodeActionRegistrationOptions())
                    .BuildServiceProvider();
            AddHandler(handlerFunc(sp));
            return this;
        }
    }

    public class LspRequestRouterTests : AutoTestBase
    {
        public LspRequestRouterTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            Container = LspTestContainer.Create(testOutputHelper);
            Container.RegisterMany<TextDocumentMatcher>(nonPublicServiceTypes: true);
        }

        [Fact]
        public async Task ShouldRouteToCorrect_Notification()
        {
            var textDocumentSyncHandler =
                TextDocumentSyncHandlerExtensions.With(DocumentSelector.ForPattern("**/*.cs"), "csharp");
            textDocumentSyncHandler.Handle(Arg.Any<DidSaveTextDocumentParams>(), Arg.Any<CancellationToken>())
                                   .Returns(Unit.Value);

            var collection =
                new SharedHandlerCollection(
                        SupportedCapabilitiesFixture.AlwaysTrue, new TextDocumentIdentifiers(), Substitute.For<IResolverContext>(),
                        new LspHandlerTypeDescriptorProvider(
                            new[] {
                                typeof(FoundationTests).Assembly, typeof(LanguageServer).Assembly, typeof(LanguageClient).Assembly, typeof(IRegistrationManager).Assembly,
                                typeof(LspRequestRouter).Assembly
                            }
                        )
                    )
                    { textDocumentSyncHandler };
            AutoSubstitute.Provide<IHandlerCollection>(collection);
            AutoSubstitute.Provide<IEnumerable<ILspHandlerDescriptor>>(collection);
            var mediator = AutoSubstitute.Resolve<LspRequestRouter>();

            var @params = new DidSaveTextDocumentParams {
                TextDocument = new TextDocumentIdentifier(new Uri("file:///c:/test/123.cs"))
            };

            var request = new Notification(
                TextDocumentNames.DidSave,
                JObject.Parse(JsonConvert.SerializeObject(@params, new Serializer(ClientVersion.Lsp3).Settings))
            );

            await mediator.RouteNotification(mediator.GetDescriptors(request), request, CancellationToken.None);

            await textDocumentSyncHandler.Received(1)
                                         .Handle(Arg.Any<DidSaveTextDocumentParams>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task ShouldRouteToCorrect_Notification_WithManyHandlers()
        {
            var textDocumentSyncHandler =
                TextDocumentSyncHandlerExtensions.With(DocumentSelector.ForPattern("**/*.cs"), "csharp");
            var textDocumentSyncHandler2 =
                TextDocumentSyncHandlerExtensions.With(DocumentSelector.ForPattern("**/*.cake"), "csharp");
            textDocumentSyncHandler.Handle(Arg.Any<DidSaveTextDocumentParams>(), Arg.Any<CancellationToken>())
                                   .Returns(Unit.Value);
            textDocumentSyncHandler2.Handle(Arg.Any<DidSaveTextDocumentParams>(), Arg.Any<CancellationToken>())
                                    .Returns(Unit.Value);

            var textDocumentIdentifiers = new TextDocumentIdentifiers();
            AutoSubstitute.Provide(textDocumentIdentifiers);
            var collection =
                new SharedHandlerCollection(
                        SupportedCapabilitiesFixture.AlwaysTrue, textDocumentIdentifiers, Substitute.For<IResolverContext>(),
                        new LspHandlerTypeDescriptorProvider(
                            new[] {
                                typeof(FoundationTests).Assembly, typeof(LanguageServer).Assembly, typeof(LanguageClient).Assembly, typeof(IRegistrationManager).Assembly,
                                typeof(LspRequestRouter).Assembly
                            }
                        )
                    )
                    { textDocumentSyncHandler, textDocumentSyncHandler2 };
            AutoSubstitute.Provide<IHandlerCollection>(collection);
            AutoSubstitute.Provide<IEnumerable<ILspHandlerDescriptor>>(collection);
            AutoSubstitute.Provide<IHandlerMatcher>(new TextDocumentMatcher(LoggerFactory.CreateLogger<TextDocumentMatcher>(), textDocumentIdentifiers));
            var mediator = AutoSubstitute.Resolve<LspRequestRouter>();

            var @params = new DidSaveTextDocumentParams {
                TextDocument = new TextDocumentIdentifier(new Uri("file:///c:/test/123.cake"))
            };

            var request = new Notification(
                TextDocumentNames.DidSave,
                JObject.Parse(JsonConvert.SerializeObject(@params, new Serializer(ClientVersion.Lsp3).Settings))
            );

            await mediator.RouteNotification(mediator.GetDescriptors(request), request, CancellationToken.None);

            await textDocumentSyncHandler.Received(0)
                                         .Handle(Arg.Any<DidSaveTextDocumentParams>(), Arg.Any<CancellationToken>());
            await textDocumentSyncHandler2.Received(1)
                                          .Handle(Arg.Any<DidSaveTextDocumentParams>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task ShouldRouteToCorrect_Request()
        {
            var textDocumentSyncHandler =
                TextDocumentSyncHandlerExtensions.With(DocumentSelector.ForPattern("**/*.cs"), "csharp");
            textDocumentSyncHandler.Handle(Arg.Any<DidSaveTextDocumentParams>(), Arg.Any<CancellationToken>())
                                   .Returns(Unit.Value);

            var codeActionHandler = Substitute.For<ICodeActionHandler>();
            codeActionHandler.GetRegistrationOptions().Returns(new CodeActionRegistrationOptions { DocumentSelector = DocumentSelector.ForPattern("**/*.cs") });
            codeActionHandler
               .Handle(Arg.Any<CodeActionParams>(), Arg.Any<CancellationToken>())
               .Returns(new CommandOrCodeActionContainer());

            var collection =
                new SharedHandlerCollection(
                        SupportedCapabilitiesFixture.AlwaysTrue, new TextDocumentIdentifiers(), Substitute.For<IResolverContext>(),
                        new LspHandlerTypeDescriptorProvider(
                            new[] {
                                typeof(FoundationTests).Assembly, typeof(LanguageServer).Assembly, typeof(LanguageClient).Assembly, typeof(IRegistrationManager).Assembly,
                                typeof(LspRequestRouter).Assembly
                            }
                        )
                    )
                    { textDocumentSyncHandler, codeActionHandler };
            AutoSubstitute.Provide<IHandlerCollection>(collection);
            AutoSubstitute.Provide<IEnumerable<ILspHandlerDescriptor>>(collection);
            var mediator = AutoSubstitute.Resolve<LspRequestRouter>();

            var id = Guid.NewGuid().ToString();
            var @params = new DidSaveTextDocumentParams {
                TextDocument = new TextDocumentIdentifier(new Uri("file:///c:/test/123.cs"))
            };

            var request = new Request(
                id, TextDocumentNames.CodeAction,
                JObject.Parse(JsonConvert.SerializeObject(@params, new Serializer(ClientVersion.Lsp3).Settings))
            );

            await mediator.RouteRequest(mediator.GetDescriptors(request), request, CancellationToken.None);

            await codeActionHandler.Received(1).Handle(Arg.Any<CodeActionParams>(), Arg.Any<CancellationToken>());
        }

        [Fact(Skip = "Check this later")]
        public async Task ShouldRouteToCorrect_Request_WithManyHandlers()
        {
            var textDocumentSyncHandler =
                TextDocumentSyncHandlerExtensions.With(DocumentSelector.ForPattern("**/*.cs"), "csharp");
            var textDocumentSyncHandler2 =
                TextDocumentSyncHandlerExtensions.With(DocumentSelector.ForPattern("**/*.cake"), "csharp");
            textDocumentSyncHandler.Handle(Arg.Any<DidSaveTextDocumentParams>(), Arg.Any<CancellationToken>())
                                   .Returns(Unit.Value);
            textDocumentSyncHandler2.Handle(Arg.Any<DidSaveTextDocumentParams>(), Arg.Any<CancellationToken>())
                                    .Returns(Unit.Value);

            var codeActionHandler = Substitute.For<ICodeActionHandler>();
            codeActionHandler.GetRegistrationOptions().Returns(new CodeActionRegistrationOptions { DocumentSelector = DocumentSelector.ForPattern("**/*.cs") });
            codeActionHandler
               .Handle(Arg.Any<CodeActionParams>(), Arg.Any<CancellationToken>())
               .Returns(new CommandOrCodeActionContainer());

            var registry = new TestLanguageServerRegistry();
            var codeActionDelegate =
                Substitute.For<Func<CodeActionParams, CancellationToken, Task<CommandOrCodeActionContainer>>>();
            codeActionDelegate.Invoke(Arg.Any<CodeActionParams>(), Arg.Any<CancellationToken>())
                              .Returns(new CommandOrCodeActionContainer());
            registry.OnCodeAction(
                codeActionDelegate,
                new CodeActionRegistrationOptions { DocumentSelector = DocumentSelector.ForPattern("**/*.cake") }
            );

            var textDocumentIdentifiers = new TextDocumentIdentifiers();
            AutoSubstitute.Provide(textDocumentIdentifiers);
            var handlerCollection =
                new SharedHandlerCollection(
                        SupportedCapabilitiesFixture.AlwaysTrue, textDocumentIdentifiers, Substitute.For<IResolverContext>(),
                        new LspHandlerTypeDescriptorProvider(
                            new[] {
                                typeof(FoundationTests).Assembly, typeof(LanguageServer).Assembly, typeof(LanguageClient).Assembly, typeof(IRegistrationManager).Assembly,
                                typeof(LspRequestRouter).Assembly
                            }
                        )
                    )
                    { textDocumentSyncHandler, textDocumentSyncHandler2, codeActionHandler };
            handlerCollection.Add(registry.Handlers.ToArray());
            AutoSubstitute.Provide<IHandlerCollection>(handlerCollection);
            AutoSubstitute.Provide<IEnumerable<ILspHandlerDescriptor>>(handlerCollection);
            AutoSubstitute.Provide<IHandlerMatcher>(new TextDocumentMatcher(LoggerFactory.CreateLogger<TextDocumentMatcher>(), textDocumentIdentifiers));
            var mediator = AutoSubstitute.Resolve<LspRequestRouter>();

            var id = Guid.NewGuid().ToString();
            var @params = new CodeActionParams {
                TextDocument = new TextDocumentIdentifier(new Uri("file:///c:/test/123.cake"))
            };

            var request = new Request(
                id, TextDocumentNames.CodeAction,
                JObject.Parse(JsonConvert.SerializeObject(@params, new Serializer(ClientVersion.Lsp3).Settings))
            );

            await mediator.RouteRequest(mediator.GetDescriptors(request), request, CancellationToken.None);

            await codeActionHandler.Received(0).Handle(Arg.Any<CodeActionParams>(), Arg.Any<CancellationToken>());
            await codeActionDelegate.Received(1).Invoke(Arg.Any<CodeActionParams>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task ShouldRouteToCorrect_Request_WithManyHandlers_CodeLensHandler()
        {
            var textDocumentSyncHandler =
                TextDocumentSyncHandlerExtensions.With(DocumentSelector.ForPattern("**/*.cs"), "csharp");
            var textDocumentSyncHandler2 =
                TextDocumentSyncHandlerExtensions.With(DocumentSelector.ForPattern("**/*.cake"), "csharp");
            textDocumentSyncHandler.Handle(Arg.Any<DidSaveTextDocumentParams>(), Arg.Any<CancellationToken>())
                                   .Returns(Unit.Value);
            textDocumentSyncHandler2.Handle(Arg.Any<DidSaveTextDocumentParams>(), Arg.Any<CancellationToken>())
                                    .Returns(Unit.Value);

            var codeActionHandler = Substitute.For<ICodeLensHandler>();
            codeActionHandler.GetRegistrationOptions().Returns(new CodeLensRegistrationOptions { DocumentSelector = DocumentSelector.ForPattern("**/*.cs") });
            codeActionHandler
               .Handle(Arg.Any<CodeLensParams>(), Arg.Any<CancellationToken>())
               .Returns(new CodeLensContainer());

            var codeActionHandler2 = Substitute.For<ICodeLensHandler>();
            codeActionHandler2.GetRegistrationOptions().Returns(new CodeLensRegistrationOptions { DocumentSelector = DocumentSelector.ForPattern("**/*.cake") });
            codeActionHandler2
               .Handle(Arg.Any<CodeLensParams>(), Arg.Any<CancellationToken>())
               .Returns(new CodeLensContainer());

            var tdi = new TextDocumentIdentifiers();
            var collection =
                new SharedHandlerCollection(
                        SupportedCapabilitiesFixture.AlwaysTrue, tdi, Substitute.For<IResolverContext>(),
                        new LspHandlerTypeDescriptorProvider(
                            new[] {
                                typeof(FoundationTests).Assembly, typeof(LanguageServer).Assembly, typeof(LanguageClient).Assembly, typeof(IRegistrationManager).Assembly,
                                typeof(LspRequestRouter).Assembly
                            }
                        )
                    )
                    { textDocumentSyncHandler, textDocumentSyncHandler2, codeActionHandler, codeActionHandler2 };
            AutoSubstitute.Provide<IHandlerCollection>(collection);
            AutoSubstitute.Provide<IEnumerable<ILspHandlerDescriptor>>(collection);
            AutoSubstitute.Provide<IHandlerMatcher>(new TextDocumentMatcher(LoggerFactory.CreateLogger<TextDocumentMatcher>(), tdi));
            var mediator = AutoSubstitute.Resolve<LspRequestRouter>();

            var id = Guid.NewGuid().ToString();
            var @params = new CodeLensParams {
                TextDocument = new TextDocumentIdentifier(new Uri("file:///c:/test/123.cs"))
            };

            var request = new Request(
                id, TextDocumentNames.CodeLens,
                JObject.Parse(JsonConvert.SerializeObject(@params, new Serializer(ClientVersion.Lsp3).Settings))
            );

            await mediator.RouteRequest(mediator.GetDescriptors(request), request, CancellationToken.None);

            await codeActionHandler2.Received(0).Handle(Arg.Any<CodeLensParams>(), Arg.Any<CancellationToken>());
            await codeActionHandler.Received(1).Handle(Arg.Any<CodeLensParams>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task ShouldRouteTo_CorrectRequestWhenGivenNullParams()
        {
            var handler = Substitute.For<IShutdownHandler>();
            handler
               .Handle(Arg.Any<ShutdownParams>(), Arg.Any<CancellationToken>())
               .Returns(Unit.Value);

            var collection =
                new SharedHandlerCollection(
                        SupportedCapabilitiesFixture.AlwaysTrue, new TextDocumentIdentifiers(), Substitute.For<IResolverContext>(),
                        new LspHandlerTypeDescriptorProvider(
                            new[] {
                                typeof(FoundationTests).Assembly, typeof(LanguageServer).Assembly, typeof(LanguageClient).Assembly, typeof(IRegistrationManager).Assembly,
                                typeof(LspRequestRouter).Assembly
                            }
                        )
                    )
                    { handler };
            AutoSubstitute.Provide<IHandlerCollection>(collection);
            AutoSubstitute.Provide<IEnumerable<ILspHandlerDescriptor>>(collection);
            var mediator = AutoSubstitute.Resolve<LspRequestRouter>();

            var id = Guid.NewGuid().ToString();
            var request = new Request(id, GeneralNames.Shutdown, new JObject());

            await mediator.RouteRequest(mediator.GetDescriptors(request), request, CancellationToken.None);

            await handler.Received(1).Handle(Arg.Any<ShutdownParams>(), Arg.Any<CancellationToken>());
        }
    }
}
