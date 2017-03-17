using System;
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
    public class MediatorTests_NotificationHandlerOfT
    {
        [Method("$/cancelRequest")]
        interface ICancelRequestHandler : INotificationHandler<CancelParams> { }

        [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
        class CancelParams
        {
            public object Id { get; set; }
        }

        [Fact]
        public async Task Test1()
        {
            var serviceProvider = Substitute.For<IServiceProvider>();
            var mediator = new Mediator(new HandlerResolver(typeof(HandlerResolverTests).GetTypeInfo().Assembly), serviceProvider);

            var @params = new CancelParams() { Id = Guid.NewGuid() };
            var notification = new Notification("$/cancelRequest", JObject.Parse(JsonConvert.SerializeObject(@params)));

            mediator.HandleNotification(notification);
        }

    }
}