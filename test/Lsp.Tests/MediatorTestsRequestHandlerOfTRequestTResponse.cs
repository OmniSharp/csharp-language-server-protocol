using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.LanguageServer.Protocol;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc.Server;
using OmniSharp.Extensions.LanguageServer;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;
using HandlerCollection = OmniSharp.Extensions.LanguageServer.Server.HandlerCollection;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Server;
using OmniSharp.Extensions.LanguageServer.Server.Abstractions;
using OmniSharp.Extensions.LanguageServer.Server.Messages;
using Serializer = OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Serializer;

namespace Lsp.Tests
{
    public class MediatorTestsRequestHandlerOfTRequestTResponse
    {
        private readonly TestLoggerFactory _testLoggerFactory;

        public MediatorTestsRequestHandlerOfTRequestTResponse(ITestOutputHelper testOutputHelper)
        {
            _testLoggerFactory = new TestLoggerFactory(testOutputHelper);
        }

        [Fact]
        public async Task RequestsCancellation()
        {
            var textDocumentSyncHandler = TextDocumentSyncHandlerExtensions.With(DocumentSelector.ForPattern("**/*.cs"));
            var mediator = Substitute.For<IMediator>();
            var serviceScopeFactory = Substitute.For<IServiceScopeFactory>();
            var serviceScope = Substitute.For<IServiceScope>();
            serviceScopeFactory.CreateScope().Returns(serviceScope);
            serviceScope.ServiceProvider.GetService(typeof(IMediator)).Returns(mediator);

            textDocumentSyncHandler.Handle(Arg.Any<DidSaveTextDocumentParams>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

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
            var router = new LspRequestRouter(collection, _testLoggerFactory, new List<IHandlerMatcher>(), new Serializer(), serviceScopeFactory);

            var id = Guid.NewGuid().ToString();
            var @params = new CodeActionParams() {
                TextDocument = new TextDocumentIdentifier(new Uri("file:///c:/test/123.cs")),
                Range = new Range(new Position(1, 1), new Position(2, 2)),
                Context = new CodeActionContext() {
                    Diagnostics = new Container<Diagnostic>()
                }
            };

            var request = new Request(id, "textDocument/codeAction", JObject.Parse(JsonConvert.SerializeObject(@params, new Serializer(ClientVersion.Lsp3).Settings)));

            var response = ((IRequestRouter)router).RouteRequest(request);
            router.CancelRequest(id);
            var result = await response;

            result.IsError.Should().BeTrue();
            result.Error.Should().BeEquivalentTo(new RequestCancelled());
        }
    }
}
