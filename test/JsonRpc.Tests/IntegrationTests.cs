using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using FluentAssertions;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Testing;
using Xunit;

namespace JsonRpc.Tests
{
    public class IntegrationTests : JsonRpcServerTestBase
    {
        public IntegrationTests() : base(new JsonRpcTestOptions())
        {
        }

        class Data
        {
            public string Value { get; set; }
        }

        [Fact]
        public async Task Should_Send_and_recieve_requests()
        {
            var (client, server) = await Initialize(
                client => {
                    client.OnRequest("myrequest", async () => new Data() {Value = "myresponse"});
                },
                server => {
                    server.OnRequest("myrequest", async () => new Data() {Value = string.Join("", "myresponse".Reverse())});
                }
            );

            var serverResponse = await client.SendRequest("myrequest").Returning<Data>(CancellationToken);
            serverResponse.Value.Should().Be("esnopserym");

            var clientResponse = await server.SendRequest("myrequest").Returning<Data>(CancellationToken);
            clientResponse.Value.Should().Be("myresponse");
        }

        [Fact]
        public async Task Should_Send_and_recieve_notifications()
        {
            var clientNotification = new AsyncSubject<Data>();
            var serverNotification = new AsyncSubject<Data>();
            var (client, server) = await Initialize(
                client => {
                    client.OnNotification("mynotification",  (Data data) => {
                        clientNotification.OnNext(data);
                        clientNotification.OnCompleted();
                    });
                },
                server => {
                    server.OnNotification("mynotification",  (Data data) => {

                        serverNotification.OnNext(data);
                        serverNotification.OnCompleted();
                    });
                }
            );

            client.SendNotification("mynotification", new Data() {Value = "myresponse"});
            var serverResponse = await serverNotification;
            serverResponse.Value.Should().Be("myresponse");

            server.SendNotification("mynotification", new Data() {Value = string.Join("", "myresponse".Reverse())});
            var clientResponse = await clientNotification;
            clientResponse.Value.Should().Be("esnopserym");
        }
    }
}
