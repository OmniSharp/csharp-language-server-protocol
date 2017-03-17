using System;
using System.Reflection;
using System.Threading.Tasks;
using JsonRpc.Server;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using NSubstitute;
using Xunit;

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
        public async Task Test1()
        {
            var serviceProvider = Substitute.For<IServiceProvider>();
            var executeCommandHandler = Substitute.For<IExecuteCommandHandler>();
            serviceProvider
                .GetService(typeof(IExecuteCommandHandler))
                .Returns(executeCommandHandler);
            var mediator = new Mediator(new HandlerResolver(typeof(HandlerResolverTests).GetTypeInfo().Assembly), serviceProvider);

            var id = Guid.NewGuid();
            var @params = new ExecuteCommandParams() { Command = "123" };
            var request = new Request(id, "workspace/executeCommand", JObject.Parse(JsonConvert.SerializeObject(@params)));

            var response = await mediator.HandleRequest(request);

            await executeCommandHandler.Received(1).Handle(Arg.Any<ExecuteCommandParams>());


        }

    }
}