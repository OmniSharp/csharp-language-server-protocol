using System;
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
using Serializer = OmniSharp.Extensions.LanguageServer.Protocol.Serializer;

namespace Lsp.Tests
{
    public class MediatorTestsRequestHandlerOfTRequest
    {
        private readonly TestLoggerFactory _testLoggerFactory;
        private readonly IHandlerMatcherCollection _handlerMatcherCollection = new HandlerMatcherCollection();

        public MediatorTestsRequestHandlerOfTRequest(ITestOutputHelper testOutputHelper)
        {
            _testLoggerFactory = new TestLoggerFactory(testOutputHelper);
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
                });

            var collection = new HandlerCollection { executeCommandHandler };
            var mediator = new LspRequestRouter(collection, _testLoggerFactory, _handlerMatcherCollection);

            var id = Guid.NewGuid().ToString();
            var @params = new ExecuteCommandParams() { Command = "123" };
            var request = new Request(id, "workspace/executeCommand", JObject.Parse(JsonConvert.SerializeObject(@params, new Serializer(ClientVersion.Lsp3).Settings)));

            var response = ((IRequestRouter)mediator).RouteRequest(request);
            mediator.CancelRequest(id);
            var result = await response;

            result.IsError.Should().BeTrue();
            result.Error.ShouldBeEquivalentTo(new RequestCancelled());
        }
    }
}
