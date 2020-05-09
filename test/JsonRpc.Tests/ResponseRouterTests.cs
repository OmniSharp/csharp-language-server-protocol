using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Newtonsoft.Json.Linq;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Client;
using OmniSharp.Extensions.JsonRpc.Serialization;
using Xunit;
using Notification = OmniSharp.Extensions.JsonRpc.Client.Notification;

namespace Lsp.Tests
{
    public class ResponseRouterTests
    {
        [Fact]
        public async Task WorksWithResultType()
        {
            var outputHandler = Substitute.For<IOutputHandler>();
            var router = new ResponseRouter(outputHandler, new JsonRpcSerializer());

            outputHandler
                .When(x => x.Send(Arg.Is<object>(x => x.GetType() == typeof(Request))))
                .Do(call =>
                {
                    var tcs = router.GetRequest((long) call.Arg<Request>().Id);
                    tcs.SetResult(new JObject());
                });

            var response = await router.SendRequest(new ItemParams(), CancellationToken.None);

            var request = outputHandler.ReceivedCalls().Single().GetArguments()[0] as Request;
            request.Method.Should().Be("abcd");

            response.Should().NotBeNull();
            response.Should().BeOfType<ItemResult>();
        }

        [Fact]
        public async Task WorksWithUnitType()
        {
            var outputHandler = Substitute.For<IOutputHandler>();
            var router = new ResponseRouter(outputHandler, new JsonRpcSerializer());

            outputHandler
                .When(x => x.Send(Arg.Is<object>(x => x.GetType() == typeof(Request))))
                .Do(call =>
                {
                    var tcs = router.GetRequest((long) call.Arg<Request>().Id);
                    tcs.SetResult(new JObject());
                });

            await router.SendRequest(new UnitParams(), CancellationToken.None);

            var request = outputHandler.ReceivedCalls().Single().GetArguments()[0] as Request;
            request.Method.Should().Be("unit");
        }

        [Fact]
        public async Task WorksWithNotification()
        {
            var outputHandler = Substitute.For<IOutputHandler>();
            var router = new ResponseRouter(outputHandler, new JsonRpcSerializer());

            router.SendNotification(new NotificationParams());

            var request = outputHandler.ReceivedCalls().Single().GetArguments()[0] as Notification;
            request.Method.Should().Be("notification");
        }

        [Method("abcd")]
        public class ItemParams : IRequest<ItemResult>
        {
        }

        public class ItemResult
        {
        }

        [Method("unit")]
        public class UnitParams : IRequest
        {
        }

        [Method("notification")]
        public class NotificationParams : IRequest
        {
        }
    }
}
