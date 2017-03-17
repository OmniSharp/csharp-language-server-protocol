using System;
using System.Collections.Generic;
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
    public class MediatorTestsRequestHandlerOfTRequestTResponse
    {
        [Method("textDocument/codeAction")]
        public interface ICodeActionHandler : IRequestHandler<CodeActionParams, IEnumerable<Command>> { }

        [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
        public class CodeActionParams
        {
            public string TextDocument { get; set; }
            public string Range { get; set; }
            public string Context { get; set; }
        }

        [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
        public class Command
        {
            public string Title { get; set; }
            [JsonProperty("command")]
            public string Name { get; set; }
        }

        [Fact]
        public async Task Test1()
        {
            var serviceProvider = Substitute.For<IServiceProvider>();
            var codeActionHandler = Substitute.For<ICodeActionHandler>();
            serviceProvider
                .GetService(typeof(ICodeActionHandler))
                .Returns(codeActionHandler);
            var mediator = new Mediator(new HandlerResolver(typeof(HandlerResolverTests).GetTypeInfo().Assembly), serviceProvider);

            var id = Guid.NewGuid();
            var @params = new CodeActionParams() { TextDocument = "TextDocument", Range = "Range", Context = "Context" };
            var request = new Request(id, "textDocument/codeAction", JObject.Parse(JsonConvert.SerializeObject(@params)));

            var response = await mediator.HandleRequest(request);

            await codeActionHandler.Received(1).Handle(Arg.Any<CodeActionParams>());
        }

    }
}