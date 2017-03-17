using System;
using System.Reflection;
using System.Threading.Tasks;
using JsonRPC;
using JsonRPC.Server;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using NSubstitute;
using Xunit;

namespace Lsp.Tests
{
    public class MediatorTests_RequestHandlerOfTRequest
    {
        [Method("workspace/executeCommand")]
        interface IExecuteCommandHandler : IRequestHandler<ExecuteCommandParams> { }

        [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
        class ExecuteCommandParams
        {
            public string Command { get; set; }
        }

        [Fact]
        public async Task Test1()
        {
            var serviceProvider = Substitute.For<IServiceProvider>();
            var mediator = new Mediator(new HandlerResolver(typeof(HandlerResolverTests).GetTypeInfo().Assembly), serviceProvider);

            var id = Guid.NewGuid();
            var @params = new ExecuteCommandParams() { Command = "123" };
            var request = new Request(id, "workspace/executeCommand", JObject.Parse(JsonConvert.SerializeObject(@params)));

            var response = await mediator.HandleRequest(request);


        }

    }
}