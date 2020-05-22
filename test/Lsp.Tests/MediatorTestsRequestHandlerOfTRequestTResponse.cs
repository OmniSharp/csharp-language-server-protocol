using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Server;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;
using HandlerCollection = OmniSharp.Extensions.LanguageServer.Server.HandlerCollection;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Server;
using OmniSharp.Extensions.LanguageServer.Server.Abstractions;
using ISerializer = OmniSharp.Extensions.LanguageServer.Protocol.Serialization.ISerializer;
using Serializer = OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Serializer;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;
using TextDocumentIdentifiers = OmniSharp.Extensions.LanguageServer.Server.TextDocumentIdentifiers;

namespace Lsp.Tests
{
    public class MediatorTestsRequestHandlerOfTRequestTResponse : AutoTestBase
    {
        public MediatorTestsRequestHandlerOfTRequestTResponse(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            Services
                .AddJsonRpcMediatR(new[] { typeof(LspRequestRouterTests).Assembly })
                .AddSingleton<ISerializer>(new Serializer(ClientVersion.Lsp3));
        }

        [Fact]
        public async Task RequestsCancellation()
        {
            var textDocumentSyncHandler = TextDocumentSyncHandlerExtensions.With(DocumentSelector.ForPattern("**/*.cs"), "csharp");
            textDocumentSyncHandler.Handle(Arg.Any<DidSaveTextDocumentParams>(), Arg.Any<CancellationToken>()).Returns(Unit.Value);

            var codeActionHandler = Substitute.For<ICodeActionHandler>();
            codeActionHandler.GetRegistrationOptions().Returns(new CodeActionRegistrationOptions() { DocumentSelector = DocumentSelector.ForPattern("**/*.cs") });
            codeActionHandler
                .Handle(Arg.Any<CodeActionParams>(), Arg.Any<CancellationToken>())
                .Returns(async (c) =>
                {
                    await Task.Delay(1000, c.Arg<CancellationToken>());
                    throw new XunitException("Task was not cancelled in time!");
                    return new CommandOrCodeActionContainer();
                });

            var collection = new HandlerCollection(SupportedCapabilitiesFixture.AlwaysTrue, new TextDocumentIdentifiers()) { textDocumentSyncHandler, codeActionHandler };
            AutoSubstitute.Provide<IHandlerCollection>(collection);
            AutoSubstitute.Provide<IEnumerable<ILspHandlerDescriptor>>(collection);
            var mediator = AutoSubstitute.Resolve<LspRequestRouter>();

            var id = Guid.NewGuid().ToString();
            var @params = new CodeActionParams()
            {
                TextDocument = new TextDocumentIdentifier(new Uri("file:///c:/test/123.cs")),
                Range = new Range(new Position(1, 1), new Position(2, 2)),
                Context = new CodeActionContext()
                {
                    Diagnostics = new Container<Diagnostic>()
                }
            };

            var request = new Request(id, "textDocument/codeAction", JObject.Parse(JsonConvert.SerializeObject(@params, new Serializer(ClientVersion.Lsp3).Settings)));

            var response = ((IRequestRouter<ILspHandlerDescriptor>)mediator).RouteRequest(request, CancellationToken.None);
            mediator.CancelRequest(id);
            var result = await response;

            result.IsError.Should().BeTrue();
            result.Error.Should().BeEquivalentTo(new RequestCancelled());
        }
    }
}
