using System.IO;
using System.IO.Pipelines;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Client;
using OmniSharp.Extensions.JsonRpc.Serialization;
using OmniSharp.Extensions.JsonRpc.Server.Messages;
using Xunit;

namespace JsonRpc.Tests
{
    public class DapOutputHandlerTests
    {
        private static OutputHandler NewHandler(PipeWriter writer)
        {
            return new OutputHandler(writer, new DapSerializer(), NullLogger<OutputHandler>.Instance);
        }

        [Fact]
        public async Task ShouldSerializeResponses()
        {
            var pipe = new Pipe(new PipeOptions());
            using var handler = NewHandler(pipe.Writer);

            var value = new Response(1, new object(),
                new OmniSharp.Extensions.JsonRpc.Server.Request(1, "command", new JObject()));

            handler.Send(value);
            await handler.WriteAndFlush();

            using var reader = new StreamReader(pipe.Reader.AsStream());
            var received = await reader.ReadToEndAsync();

            const string send =
                "Content-Length: 88\r\n\r\n{\"seq\":1,\"type\":\"response\",\"request_seq\":1,\"success\":true,\"command\":\"command\",\"body\":{}}";
            received.Should().Be(send);
        }

        [Fact]
        public async Task ShouldSerializeNotifications()
        {
            var pipe = new Pipe(new PipeOptions());
            using var handler = NewHandler(pipe.Writer);

            var value = new OmniSharp.Extensions.JsonRpc.Client.Notification() {
                Method = "method",
                Params = new object()
            };

            handler.Send(value);
            await handler.WriteAndFlush();

            using var reader = new StreamReader(pipe.Reader.AsStream());
            var received = await reader.ReadToEndAsync();

            const string send =
                "Content-Length: 51\r\n\r\n{\"seq\":1,\"type\":\"event\",\"event\":\"method\",\"body\":{}}";
            received.Should().Be(send);
        }

        [Fact]
        public async Task ShouldSerializeRequests()
        {
            var pipe = new Pipe(new PipeOptions());
            using var handler = NewHandler(pipe.Writer);

            var value = new OmniSharp.Extensions.JsonRpc.Client.Request() {
                Method = "method",
                Id = 1,
                Params = new object(),
            };

            handler.Send(value);
            await handler.WriteAndFlush();

            using var reader = new StreamReader(pipe.Reader.AsStream());
            var received = await reader.ReadToEndAsync();

            const string send =
                "Content-Length: 60\r\n\r\n{\"seq\":1,\"type\":\"request\",\"command\":\"method\",\"arguments\":{}}";
            received.Should().Be(send);
        }

        [Fact]
        public async Task ShouldSerializeErrors()
        {
            var pipe = new Pipe(new PipeOptions());
            using var handler = NewHandler(pipe.Writer);

            var value = new RpcError(1, new ErrorMessage(1, "something", "data"));

            handler.Send(value);
            await handler.WriteAndFlush();

            using var reader = new StreamReader(pipe.Reader.AsStream());
            var received = await reader.ReadToEndAsync();

            const string send =
                "Content-Length: 76\r\n\r\n{\"seq\":1,\"type\":\"response\",\"request_seq\":1,\"success\":false,\"message\":\"data\"}";
            received.Should().Be(send);
        }
    }
}
