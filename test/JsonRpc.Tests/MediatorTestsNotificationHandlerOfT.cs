using System;
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
    public class MediatorTestsNotificationHandlerOfT
    {
        [Method("$/cancelRequest")]
        public interface ICancelRequestHandler : INotificationHandler<CancelParams> { }

        [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
        public class CancelParams
        {
            public object Id { get; set; }
        }

        [Fact]
        public async Task ExecutesHandler()
        {
            var serviceProvider = Substitute.For<IServiceProvider>();
            var cancelRequestHandler = Substitute.For<ICancelRequestHandler>();
            serviceProvider
                .GetService(typeof(ICancelRequestHandler))
                .Returns(cancelRequestHandler);
            var mediator = new Mediator(new HandlerResolver(typeof(HandlerResolverTests).GetTypeInfo().Assembly), serviceProvider);

            var @params = new CancelParams() { Id = Guid.NewGuid() };
            var notification = new Notification("$/cancelRequest", JObject.Parse(JsonConvert.SerializeObject(@params)));

            mediator.HandleNotification(notification);

            await cancelRequestHandler.Received(1).Handle(Arg.Any<CancelParams>());
        }

    }
}