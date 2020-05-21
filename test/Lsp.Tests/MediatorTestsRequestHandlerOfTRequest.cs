using System;
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
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Server;
using ISerializer = OmniSharp.Extensions.LanguageServer.Protocol.Serialization.ISerializer;
using Serializer = OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Serializer;
using System.Collections.Generic;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Shared;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using OmniSharp.Extensions.LanguageServer.Shared;

namespace Lsp.Tests
{
    public class MediatorTestsRequestHandlerOfTRequest : AutoTestBase
    {
        public MediatorTestsRequestHandlerOfTRequest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            Services
                .AddJsonRpcMediatR(new[] { typeof(LspRequestRouterTests).Assembly })
                .AddSingleton<ISerializer>(new Serializer(ClientVersion.Lsp3));
        }

        [Fact]
        public async Task RequestsCancellation()
        {
            var executeCommandHandler = Substitute.For<IExecuteCommandHandler>();
            executeCommandHandler
                .Handle(Arg.Any<ExecuteCommandParams>(), Arg.Any<CancellationToken>())
                .Returns(async (c) => {
                    await Task.Delay(1000, c.Arg<CancellationToken>());
                    throw new XunitException("Task was not cancelled in time!");
                    return Unit.Value;
                });

            var collection = new SharedHandlerCollection(SupportedCapabilitiesFixture.AlwaysTrue, new TextDocumentIdentifiers()) { executeCommandHandler };
            AutoSubstitute.Provide<IHandlerCollection>(collection);
            AutoSubstitute.Provide<IEnumerable<ILspHandlerDescriptor>>(collection);
            var mediator = AutoSubstitute.Resolve<LspRequestRouter>();

            var id = Guid.NewGuid().ToString();
            var @params = new ExecuteCommandParams() { Command = "123" };
            var request = new Request(id, "workspace/executeCommand", JObject.Parse(JsonConvert.SerializeObject(@params, new Serializer(ClientVersion.Lsp3).Settings)));

            var response = ((IRequestRouter<ILspHandlerDescriptor>)mediator).RouteRequest(mediator.GetDescriptor(request),  request, CancellationToken.None, CancellationToken.None);
            mediator.CancelRequest(id);
            var result = await response;

            result.IsError.Should().BeTrue();
            result.Error.Should().BeEquivalentTo(new RequestCancelled(id));
        }
    }
}
