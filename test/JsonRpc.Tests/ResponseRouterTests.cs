using System;
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

namespace JsonRpc.Tests
{
    public class ResponseRouterTests
    {
        [Fact]
        public async Task WorksWithResultType()
        {
            var outputHandler = Substitute.For<IOutputHandler>();
            var router = new ResponseRouter(new Lazy<IOutputHandler>(() => outputHandler), new JsonRpcSerializer(), new AssemblyScanningHandlerTypeDescriptorProvider(new [] { typeof(AssemblyScanningHandlerTypeDescriptorProvider).Assembly, typeof(HandlerResolverTests).Assembly }));

            outputHandler
               .When(x => x.Send(Arg.Is<object>(z => z.GetType() == typeof(OutgoingRequest))))
               .Do(
                    call => {
                        router.TryGetRequest((long) call.Arg<OutgoingRequest>().Id!, out _, out var tcs);
                        tcs.TrySetResult(new JObject());
                    }
                );

            var response = await router.SendRequest(new ItemParams(), CancellationToken.None);

            var request = outputHandler.ReceivedCalls().Single().GetArguments()[0] as OutgoingRequest;
            request!.Method.Should().Be("abcd");

            response.Should().NotBeNull();
            response.Should().BeOfType<ItemResult>();
        }

        [Fact]
        public async Task WorksWithUnitType()
        {
            var outputHandler = Substitute.For<IOutputHandler>();
            var router = new ResponseRouter(new Lazy<IOutputHandler>(() => outputHandler), new JsonRpcSerializer(), new AssemblyScanningHandlerTypeDescriptorProvider(new [] { typeof(AssemblyScanningHandlerTypeDescriptorProvider).Assembly, typeof(HandlerResolverTests).Assembly }));

            outputHandler
               .When(x => x.Send(Arg.Is<object>(z => z.GetType() == typeof(OutgoingRequest))))
               .Do(
                    call => {
                        router.TryGetRequest((long) call.Arg<OutgoingRequest>().Id!, out _, out var tcs);
                        tcs.SetResult(new JObject());
                    }
                );

            await router.SendRequest(new UnitParams(), CancellationToken.None);

            var request = outputHandler.ReceivedCalls().Single().GetArguments()[0] as OutgoingRequest;
            request!.Method.Should().Be("unit");
        }

        [Fact]
        public async Task WorksWithNotification()
        {
            var outputHandler = Substitute.For<IOutputHandler>();
            var router = new ResponseRouter(new Lazy<IOutputHandler>(() => outputHandler), new JsonRpcSerializer(), new AssemblyScanningHandlerTypeDescriptorProvider(new [] { typeof(AssemblyScanningHandlerTypeDescriptorProvider).Assembly, typeof(HandlerResolverTests).Assembly }));

            router.SendNotification(new NotificationParams());

            var request = outputHandler.ReceivedCalls().Single().GetArguments()[0] as OutgoingNotification;
            request!.Method.Should().Be("notification");
        }

        [Method("abcd")]
        public class ItemParams : IRequest<ItemResult>
        {
        }

        public class ItemResult
        {
        }

        [Method("unit")]
        public class UnitParams : IRequest<Unit>
        {
        }

        [Method("notification")]
        public class NotificationParams : IRequest<Unit>
        {
        }
    }
}
