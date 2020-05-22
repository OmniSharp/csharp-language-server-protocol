using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Serialization;
using OmniSharp.Extensions.JsonRpc.Server;
using Xunit;
using Xunit.Abstractions;

namespace JsonRpc.Tests
{
    public class MediatorTestsRequestHandlerOfTRequestTResponse : AutoTestBase
    {
        [Method("textDocument/codeAction")]
        public interface ICodeActionHandler : IJsonRpcRequestHandler<CodeActionParams, IEnumerable<Command>> { }

        public class CodeActionParams : IRequest<IEnumerable<Command>>
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

        public MediatorTestsRequestHandlerOfTRequestTResponse(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            Services
                .AddJsonRpcMediatR(new[] { typeof(MediatorTestsNotificationHandler).Assembly })
                .AddSingleton<ISerializer>(new JsonRpcSerializer());
        }

        [Fact]
        public async Task ExecutesHandler()
        {
            var codeActionHandler = Substitute.For<ICodeActionHandler>();
            var mediator = Substitute.For<IMediator>();

            var collection = new HandlerCollection { codeActionHandler };
            AutoSubstitute.Provide(collection);
            var router = AutoSubstitute.Resolve<RequestRouter>();

            var id = Guid.NewGuid().ToString();
            var @params = new CodeActionParams() { TextDocument = "TextDocument", Range = "Range", Context = "Context" };
            var request = new Request(id, "textDocument/codeAction", JObject.Parse(JsonConvert.SerializeObject(@params)));

            var response = await router.RouteRequest(router.GetDescriptor(request), request, CancellationToken.None);

            await codeActionHandler.Received(1).Handle(Arg.Any<CodeActionParams>(), Arg.Any<CancellationToken>());
        }
    }
}
