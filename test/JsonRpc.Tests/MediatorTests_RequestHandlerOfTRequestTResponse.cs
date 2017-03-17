using System;
using System.Collections.Generic;
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
    public class MediatorTests_RequestHandlerOfTRequestTResponse
    {
        [Method("textDocument/codeAction")]
        interface ICodeActionHandler : IRequestHandler<CodeActionParams, IEnumerable<Command>> { }

        [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
        class CodeActionParams
        {
            public string TextDocument { get; set; }
            public string Range { get; set; }
            public string Context { get; set; }
        }

        [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
        class Command
        {
            public string Title { get; set; }
            [JsonProperty("command")]
            public string Name { get; set; }
        }

        [Fact]
        public async Task Test1()
        {
            var serviceProvider = Substitute.For<IServiceProvider>();
            var mediator = new Mediator(new HandlerResolver(typeof(HandlerResolverTests).GetTypeInfo().Assembly), serviceProvider);
            
            var id = Guid.NewGuid();
            var @params = new CodeActionParams() { TextDocument = "TextDocument", Range = "Range", Context = "Context" };
            var request = new Request(id, "textDocument/codeAction", JObject.Parse(JsonConvert.SerializeObject(@params)));

            var response = await mediator.HandleRequest(request);
        }

    }
}