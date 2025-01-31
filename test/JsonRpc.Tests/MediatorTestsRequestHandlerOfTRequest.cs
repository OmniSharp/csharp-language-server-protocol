using System;
using System.Threading;
using System.Threading.Tasks;
using DryIoc;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc;
using Xunit;
using Xunit.Abstractions;
using Arg = NSubstitute.Arg;
using Request = OmniSharp.Extensions.JsonRpc.Server.Request;

namespace JsonRpc.Tests
{
    public class MediatorTestsRequestHandlerOfTRequest : AutoTestBase
    {
        [Method("workspace/executeCommand")]
        public interface IExecuteCommandHandler : IJsonRpcRequestHandler<ExecuteCommandParams>
        {
        }

        public class ExecuteCommandParams : IRequest<Unit>
        {
            public string Command { get; set; } = null!;
        }

        public MediatorTestsRequestHandlerOfTRequest(ITestOutputHelper testOutputHelper) : base(testOutputHelper) => Container = JsonRpcTestContainer.Create(testOutputHelper);

        [Fact]
        public async Task ExecutesHandler()
        {
            var executeCommandHandler = Substitute.For<IExecuteCommandHandler>();

            var collection = new HandlerCollection(Substitute.For<IResolverContext>(), new AssemblyScanningHandlerTypeDescriptorProvider(new [] { typeof(AssemblyScanningHandlerTypeDescriptorProvider).Assembly, typeof(HandlerResolverTests).Assembly })) { executeCommandHandler };
            AutoSubstitute.Provide<IHandlersManager>(collection);
            var router = AutoSubstitute.Resolve<RequestRouter>();

            var id = Guid.NewGuid().ToString();
            var @params = new ExecuteCommandParams { Command = "123" };
            var request = new Request(id, "workspace/executeCommand", JObject.Parse(JsonConvert.SerializeObject(@params)));

            await router.RouteRequest(router.GetDescriptors(request), request, CancellationToken.None);

            await executeCommandHandler.Received(1).Handle(Arg.Any<ExecuteCommandParams>(), Arg.Any<CancellationToken>());
        }
    }
}
