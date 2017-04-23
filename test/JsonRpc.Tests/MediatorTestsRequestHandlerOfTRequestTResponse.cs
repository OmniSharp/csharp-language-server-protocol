using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task ExecutesHandler()
        {
            var codeActionHandler = Substitute.For<ICodeActionHandler>();

            var collection = new HandlerCollection { codeActionHandler };
            var mediator = new RequestRouter(collection);

            var id = Guid.NewGuid().ToString();
            var @params = new CodeActionParams() { TextDocument = "TextDocument", Range = "Range", Context = "Context" };
            var request = new Request(id, "textDocument/codeAction", JObject.Parse(JsonConvert.SerializeObject(@params)));

            var response = await mediator.RouteRequest(request);

            await codeActionHandler.Received(1).Handle(Arg.Any<CodeActionParams>(), Arg.Any<CancellationToken>());
        }
    }
}