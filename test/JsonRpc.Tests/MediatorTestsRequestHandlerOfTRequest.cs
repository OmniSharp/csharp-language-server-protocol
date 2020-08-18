using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Server;
using Xunit;
using Xunit.Abstractions;

namespace JsonRpc.Tests
{
    public class MediatorTestsRequestHandlerOfTRequest : AutoTestBase
    {
        [Method("workspace/executeCommand")]
        public interface IExecuteCommandHandler : IJsonRpcRequestHandler<ExecuteCommandParams>
        {
        }

        public class ExecuteCommandParams : IRequest
        {
            public string Command { get; set; }
        }

        public MediatorTestsRequestHandlerOfTRequest(ITestOutputHelper testOutputHelper) : base(testOutputHelper) => Container = JsonRpcTestContainer.Create(testOutputHelper);

        [Fact]
        public async Task ExecutesHandler()
        {
            var executeCommandHandler = Substitute.For<IExecuteCommandHandler>();
            var mediator = Substitute.For<IMediator>();

            var collection = new HandlerCollection(new ServiceCollection().BuildServiceProvider(), new HandlerTypeDescriptorProvider(new [] { typeof(HandlerTypeDescriptorProvider).Assembly, typeof(HandlerResolverTests).Assembly })) { executeCommandHandler };
            AutoSubstitute.Provide<IHandlersManager>(collection);
            var router = AutoSubstitute.Resolve<RequestRouter>();

            var id = Guid.NewGuid().ToString();
            var @params = new ExecuteCommandParams { Command = "123" };
            var request = new Request(id, "workspace/executeCommand", JObject.Parse(JsonConvert.SerializeObject(@params)));

            var response = await router.RouteRequest(router.GetDescriptors(request), request, CancellationToken.None);

            await executeCommandHandler.Received(1).Handle(Arg.Any<ExecuteCommandParams>(), Arg.Any<CancellationToken>());
        }
    }
}
