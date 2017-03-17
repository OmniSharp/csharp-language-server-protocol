using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using JsonRpc.Server;
using JsonRpc.Server.Messages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using NSubstitute;
using Xunit;
using Xunit.Sdk;

namespace JsonRpc.Tests
{
    public class MediatorTestsRequestHandlerOfTRequest
    {
        [Method("workspace/executeCommand")]
        public interface IExecuteCommandHandler : IRequestHandler<ExecuteCommandParams> { }

        [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
        public class ExecuteCommandParams
        {
            public string Command { get; set; }
        }

        [Fact]
        public async Task ExecutesHandler()
        {
            var serviceProvider = Substitute.For<IServiceProvider>();
            var executeCommandHandler = Substitute.For<IExecuteCommandHandler>();
            serviceProvider
                .GetService(typeof(IExecuteCommandHandler))
                .Returns(executeCommandHandler);
            var mediator = new Mediator(new HandlerResolver(typeof(HandlerResolverTests).GetTypeInfo().Assembly), serviceProvider);

            var id = Guid.NewGuid().ToString();
            var @params = new ExecuteCommandParams() { Command = "123" };
            var request = new Request(id, "workspace/executeCommand", JObject.Parse(JsonConvert.SerializeObject(@params)));

            var response = await mediator.HandleRequest(request);

            await executeCommandHandler.Received(1).Handle(Arg.Any<ExecuteCommandParams>(), Arg.Any<CancellationToken>());


        }


        [Fact]
        public async Task RequestsCancellation()
        {
            var serviceProvider = Substitute.For<IServiceProvider>();
            var executeCommandHandler = Substitute.For<IExecuteCommandHandler>();
            executeCommandHandler
                .Handle(Arg.Any<ExecuteCommandParams>(), Arg.Any<CancellationToken>())
                .Returns(async (c) => {
                    await Task.Delay(1000, c.Arg<CancellationToken>());
                    throw new XunitException("Task was not cancelled in time!");
                });
            serviceProvider
                .GetService(typeof(IExecuteCommandHandler))
                .Returns(executeCommandHandler);
            var mediator = new Mediator(new HandlerResolver(typeof(HandlerResolverTests).GetTypeInfo().Assembly), serviceProvider);

            var id = Guid.NewGuid().ToString();
            var @params = new ExecuteCommandParams() { Command = "123" };
            var request = new Request(id, "workspace/executeCommand", JObject.Parse(JsonConvert.SerializeObject(@params)));

            var response = mediator.HandleRequest(request);
            mediator.CancelRequest(id);
            var result = await response;

            result.IsError.Should().BeTrue();
            result.Error.ShouldBeEquivalentTo(new RequestCancelled());
        }

    }
}