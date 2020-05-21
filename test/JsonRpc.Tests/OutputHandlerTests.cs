using System.IO;
using System.IO.Pipelines;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Client;
using OmniSharp.Extensions.JsonRpc.Serialization;
using OmniSharp.Extensions.JsonRpc.Server.Messages;
using Xunit;

namespace JsonRpc.Tests
{
    public class OutputHandlerTests
    {
        private static OutputHandler NewHandler(PipeWriter writer)
        {
            return new OutputHandler(writer, new JsonRpcSerializer(), NullLogger<OutputHandler>.Instance);
        }

        [Fact]
        public async Task ShouldSerializeResponses()
        {
            var pipe = new Pipe(new PipeOptions());
            using var handler = NewHandler(pipe.Writer);

            var value = new Response(1, 1, new OmniSharp.Extensions.JsonRpc.Server.Request(1, "a", null));


            handler.Send(value);
            await handler.WriteAndFlush();

            using var reader = new StreamReader(pipe.Reader.AsStream());
            var received = await reader.ReadToEndAsync();

            const string send = "Content-Length: 35\r\n\r\n{\"jsonrpc\":\"2.0\",\"id\":1,\"result\":1}";
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

            const string send = "Content-Length: 47\r\n\r\n{\"jsonrpc\":\"2.0\",\"method\":\"method\",\"params\":{}}";
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
                "Content-Length: 54\r\n\r\n{\"jsonrpc\":\"2.0\",\"id\":1,\"method\":\"method\",\"params\":{}}";
            received.Should().Be(send);
        }

        [Fact]
        public async Task ShouldSerializeErrors()
        {
            var pipe = new Pipe(new PipeOptions());
            using var handler = NewHandler(pipe.Writer);

            var value = new RpcError(1, new ErrorMessage(1, "something", new object()));


            handler.Send(value);
            await handler.WriteAndFlush();

            using var reader = new StreamReader(pipe.Reader.AsStream());
            var received = await reader.ReadToEndAsync();

            const string send =
                "Content-Length: 75\r\n\r\n{\"jsonrpc\":\"2.0\",\"id\":1,\"error\":{\"code\":1,\"data\":{},\"message\":\"something\"}}";
            received.Should().Be(send);
        }
    }
}
