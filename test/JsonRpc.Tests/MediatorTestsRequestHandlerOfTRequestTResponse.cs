using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Server;
using Xunit;
using Xunit.Sdk;

namespace JsonRpc.Tests
{
    public class MediatorTestsRequestHandlerOfTRequestTResponse
    {
        [Method("textDocument/codeAction")]
        public interface ICodeActionHandler : IRequestHandler<CodeActionParams, IEnumerable<Command>> { }

        public class CodeActionParams
        {
            public string TextDocument { get; set; }
            public string Range { get; set; }
            public string Context { get; set; }
        }

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
            IRequestRouter mediator = new RequestRouter(collection, new Serializer());

            var id = Guid.NewGuid().ToString();
            var @params = new CodeActionParams() { TextDocument = "TextDocument", Range = "Range", Context = "Context" };
            var request = new Request(id, "textDocument/codeAction", JObject.Parse(JsonConvert.SerializeObject(@params)));

            var response = await mediator.RouteRequest(request);

            await codeActionHandler.Received(1).Handle(Arg.Any<CodeActionParams>(), Arg.Any<CancellationToken>());
        }
    }
}
