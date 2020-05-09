using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Serialization;
using OmniSharp.Extensions.JsonRpc.Server;
using Xunit;
using Xunit.Abstractions;

namespace JsonRpc.Tests
{
    public class MediatorTestsNotificationHandler : AutoTestBase
    {
        [Method("exit")]
        public interface IExitHandler : IJsonRpcNotificationHandler { }

        public MediatorTestsNotificationHandler(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            Services
                .AddJsonRpcMediatR(new [] { typeof(MediatorTestsNotificationHandler).Assembly })
                .AddSingleton<ISerializer>(new JsonRpcSerializer());
        }

        [Fact]
        public async Task ExecutesHandler()
        {
            var exitHandler = Substitute.For<IExitHandler>();

            var collection = new HandlerCollection { exitHandler };
            AutoSubstitute.Provide(collection);
            var router = AutoSubstitute.Resolve<RequestRouter>();

            var notification = new Notification("exit", null);

            await router.RouteNotification(router.GetDescriptor(notification), notification, CancellationToken.None);

            await exitHandler.Received(1).Handle(Arg.Any<EmptyRequest>(), CancellationToken.None);
        }

    }
}
