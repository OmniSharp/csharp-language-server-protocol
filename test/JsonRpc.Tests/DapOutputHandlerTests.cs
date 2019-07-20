using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Client;
using OmniSharp.Extensions.JsonRpc.Serialization;
using OmniSharp.Extensions.JsonRpc.Server;
using OmniSharp.Extensions.JsonRpc.Server.Messages;
using Xunit;

namespace JsonRpc.Tests
{
    public class DapOutputHandlerTests
    {
        private static (OutputHandler handler, Func<Task> wait) NewHandler(Stream Writer, Action<CancellationTokenSource> action)
        {
            var cts = new CancellationTokenSource();
            if (!System.Diagnostics.Debugger.IsAttached)
                cts.CancelAfter(TimeSpan.FromSeconds(120));
            action(cts);

            var handler = new OutputHandler(
                Writer,
                new DapSerializer());
            handler.Start();
            return (handler, () => {
                        cts.Wait();
                        return Task.Delay(50);
                    }
                );
        }

        [Fact]
        public async Task ShouldSerializeResponses()
        {
            var w = Substitute.For<Stream>();
            var received = "";
            w.CanWrite.Returns(true);

            var (handler, wait) = NewHandler(w, cts => {
                w.When(x => x.Write(Arg.Any<byte[]>(), Arg.Any<int>(), Arg.Any<int>()))
                    .Do(c => {
                        received = System.Text.Encoding.UTF8.GetString(c.ArgAt<byte[]>(0), 0, c.ArgAt<int>(2));
                        cts.Cancel();
                    });
            });
            var value = new Response(1, new object(), new OmniSharp.Extensions.JsonRpc.Server.Request(1, "command", new JObject()));

            using (handler)
            {

                handler.Send(value);
                await wait();
                const string send = "Content-Length: 73\r\n\r\n{\"seq\":0,\"type\":\"response\",\"request_seq\":1,\"command\":\"command\",\"body\":{}}";
                received.Should().Be(send);
                var b = System.Text.Encoding.UTF8.GetBytes(send);
                w.Received().Write(Arg.Any<byte[]>(), 0, b.Length); // can't compare b here, because it is only value-equal and this test tests reference equality
            }
        }

        [Fact]
        public async Task ShouldSerializeNotifications()
        {
            var w = Substitute.For<Stream>();
            var received = "";
            w.CanWrite.Returns(true);

            var (handler, wait) = NewHandler(w, cts => {
                w.When(x => x.Write(Arg.Any<byte[]>(), Arg.Any<int>(), Arg.Any<int>()))
                    .Do(c => {
                        received = System.Text.Encoding.UTF8.GetString(c.ArgAt<byte[]>(0), 0, c.ArgAt<int>(2));
                        cts.Cancel();
                    });
            });
            var value = new OmniSharp.Extensions.JsonRpc.Client.Notification() {
                Method = "method",
                Params = new object()
            };

            using (handler)
            {

                handler.Send(value);
                await wait();
                const string send = "Content-Length: 51\r\n\r\n{\"seq\":0,\"type\":\"event\",\"event\":\"method\",\"body\":{}}";
                received.Should().Be(send);
                var b = System.Text.Encoding.UTF8.GetBytes(send);
                w.Received().Write(Arg.Any<byte[]>(), 0, b.Length); // can't compare b here, because it is only value-equal and this test tests reference equality
            }
        }

        [Fact]
        public async Task ShouldSerializeRequests()
        {
            var w = Substitute.For<Stream>();
            var received = "";
            w.CanWrite.Returns(true);

            var (handler, wait) = NewHandler(w, cts => {
                w.When(x => x.Write(Arg.Any<byte[]>(), Arg.Any<int>(), Arg.Any<int>()))
                    .Do(c => {
                        received = System.Text.Encoding.UTF8.GetString(c.ArgAt<byte[]>(0), 0, c.ArgAt<int>(2));
                        cts.Cancel();
                    });
            });
            var value = new OmniSharp.Extensions.JsonRpc.Client.Request() {
                Method = "method",
                Id = 1,
                Params = new object(),
            };

            using (handler)
            {

                handler.Send(value);
                await wait();
                const string send = "Content-Length: 60\r\n\r\n{\"seq\":1,\"type\":\"request\",\"command\":\"method\",\"arguments\":{}}";
                received.Should().Be(send);
                var b = System.Text.Encoding.UTF8.GetBytes(send);
                w.Received().Write(Arg.Any<byte[]>(), 0, b.Length); // can't compare b here, because it is only value-equal and this test tests reference equality
            }
        }

        [Fact]
        public async Task ShouldSerializeErrors()
        {
            var w = Substitute.For<Stream>();
            var received = "";
            w.CanWrite.Returns(true);

            var (handler, wait) = NewHandler(w, cts => {
                w.When(x => x.Write(Arg.Any<byte[]>(), Arg.Any<int>(), Arg.Any<int>()))
                    .Do(c => {
                        received = System.Text.Encoding.UTF8.GetString(c.ArgAt<byte[]>(0), 0, c.ArgAt<int>(2));
                        cts.Cancel();
                    });
            });
            var value = new RpcError(1, new ErrorMessage(1, "something", new object()));

            using (handler)
            {

                handler.Send(value);
                await wait();
                const string send = "Content-Length: 47\r\n\r\n{\"seq\":0,\"request_seq\":1,\"message\":\"something\"}";
                received.Should().Be(send);
                var b = System.Text.Encoding.UTF8.GetBytes(send);
                w.Received().Write(Arg.Any<byte[]>(), 0, b.Length); // can't compare b here, because it is only value-equal and this test tests reference equality
            }
        }
    }
}
