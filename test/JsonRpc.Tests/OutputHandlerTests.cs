using System;
using System.IO;
using System.IO.Pipelines;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Client;
using OmniSharp.Extensions.JsonRpc.Serialization;
using OmniSharp.Extensions.JsonRpc.Server;
using OmniSharp.Extensions.JsonRpc.Server.Messages;
using Xunit;

namespace JsonRpc.Tests
{
    public class OutputHandlerTests
    {
        private static OutputHandler NewHandler(PipeWriter writer)
        {
            var rec = Substitute.For<IReceiver>();
            rec.ShouldFilterOutput(Arg.Any<object>()).Returns(true);
            return new OutputHandler(writer, new JsonRpcSerializer(), rec, Scheduler.Immediate, NullLogger<OutputHandler>.Instance);
        }

        private static OutputHandler NewHandler(PipeWriter writer, Func<object, bool> filter)
        {
            var rec = Substitute.For<IReceiver>();
            rec.ShouldFilterOutput(Arg.Any<object>()).Returns(_ => filter(_.ArgAt<object>(0)));
            return new OutputHandler(writer, new JsonRpcSerializer(), rec, Scheduler.Immediate, NullLogger<OutputHandler>.Instance);
        }

        [Fact]
        public async Task ShouldSerializeResponses()
        {
            var pipe = new Pipe(new PipeOptions());
            using var handler = NewHandler(pipe.Writer);

            var value = new OutgoingResponse(1, 1, new Request(1, "a", null));


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

            var value = new OutgoingNotification {
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

            var value = new OutgoingRequest {
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

        [Fact]
        public async Task ShouldFilterMessages()
        {
            var pipe = new Pipe(new PipeOptions());
            using var handler = NewHandler(pipe.Writer, _ => _ is RpcError e && e.Id.Equals(2));

            var value = new RpcError(1, new ErrorMessage(1, "something", new object()));
            var value2 = new RpcError(2, new ErrorMessage(1, "something", new object()));
            var value3 = new RpcError(3, new ErrorMessage(1, "something", new object()));

            handler.Send(value);
            handler.Send(value2);
            handler.Send(value3);
            await handler.WriteAndFlush();

            using var reader = new StreamReader(pipe.Reader.AsStream());
            var received = await reader.ReadToEndAsync();

            const string send =
                "Content-Length: 75\r\n\r\n{\"jsonrpc\":\"2.0\",\"id\":2,\"error\":{\"code\":1,\"data\":{},\"message\":\"something\"}}";
            received.Should().Be(send);
        }
    }
}
