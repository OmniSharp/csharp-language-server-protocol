using System;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Serialization;
using OmniSharp.Extensions.JsonRpc.Server;
using Xunit;
using Xunit.Abstractions;
using Request = OmniSharp.Extensions.JsonRpc.Server.Request;
using Response = OmniSharp.Extensions.JsonRpc.Client.Response;

namespace JsonRpc.Tests
{
    [Collection("InputHandlers")]
    public class InputHandlerTests
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly TestLoggerFactory _loggerFactory;

        public InputHandlerTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;

            _loggerFactory = new TestLoggerFactory(_testOutputHelper);
        }

        private static InputHandler NewHandler(
            PipeReader inputStream,
            IOutputHandler outputHandler,
            IReceiver receiver,
            IRequestProcessIdentifier requestProcessIdentifier,
            IRequestRouter<IHandlerDescriptor> requestRouter,
            ILoggerFactory loggerFactory,
            IResponseRouter responseRouter)
        {
            return new InputHandler(
                inputStream,
                outputHandler,
                receiver,
                requestProcessIdentifier,
                requestRouter,
                responseRouter,
                loggerFactory,
                new JsonRpcSerializer(),
                null
            );
        }

        [Fact]
        public async Task Should_Pass_In_Requests()
        {
            var pipe = new Pipe(new PipeOptions());

            var outputHandler = Substitute.For<IOutputHandler>();
            var reciever = Substitute.For<IReceiver>();

            using var handler = NewHandler(pipe.Reader, outputHandler, reciever,
                Substitute.For<IRequestProcessIdentifier>(), Substitute.For<IRequestRouter<IHandlerDescriptor>>(),
                _loggerFactory, Substitute.For<IResponseRouter>());
            await pipe.Writer.WriteAsync(Encoding.UTF8.GetBytes("Content-Length: 2\r\n\r\n{}"));

            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(5));
            var processTask = handler.ProcessInputStream(cts.Token);

            await pipe.Writer.CompleteAsync();
            await processTask;

            reciever.Received().IsValid(Arg.Is<JToken>(x => x.ToString() == "{}"));
        }

        [Fact]
        public async Task Should_Handle_Multiple_Requests_At_Once()
        {
            var pipe = new Pipe(new PipeOptions());

            var outputHandler = Substitute.For<IOutputHandler>();
            var reciever = Substitute.For<IReceiver>();

            using var handler = NewHandler(pipe.Reader, outputHandler, reciever,
                Substitute.For<IRequestProcessIdentifier>(), Substitute.For<IRequestRouter<IHandlerDescriptor>>(),
                _loggerFactory, Substitute.For<IResponseRouter>());
            await pipe.Writer.WriteAsync(
                Encoding.UTF8.GetBytes("Content-Length: 2\r\n\r\n{}")
                    .Concat(Encoding.UTF8.GetBytes("Content-Length: 2\r\n\r\n{}"))
                    .Concat(Encoding.UTF8.GetBytes("Content-Length: 2\r\n\r\n{}"))
                    .ToArray()
            );

            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(5));
            var processTask = handler.ProcessInputStream(cts.Token);

            await pipe.Writer.CompleteAsync();
            await processTask;

            reciever.Received(3).IsValid(Arg.Is<JToken>(x => x.ToString() == "{}"));
        }

        [Theory]
        [InlineData(
            "Content-Length:                    2                       \r\nContent-Type: application/json\r\n\r\n{}")]
        [InlineData("Content-Type: application/json\r\nContent-Length: 2\r\n\r\n{}")]
        [InlineData("Content-Type: application/json\r\nNot-A-Header: really\r\nContent-Length: 2\r\n\r\n{}")]
        [InlineData(
            "Content-Type: application/json\r\nNot-A-Header: really\r\nContent-Length:2                                                                                               \r\n\r\n{}")]
        [InlineData(
            "Content-Type: application/json\r\nNot-A-Header: really\r\nContent-Length:                                                                                                2\r\n\r\n{}")]
        public async Task Should_Handle_Different_Additional_Headers_and_Whitespace(string data)
        {
            var pipe = new Pipe(new PipeOptions());

            var outputHandler = Substitute.For<IOutputHandler>();
            var reciever = Substitute.For<IReceiver>();

            using var handler = NewHandler(pipe.Reader, outputHandler, reciever,
                Substitute.For<IRequestProcessIdentifier>(), Substitute.For<IRequestRouter<IHandlerDescriptor>>(),
                _loggerFactory, Substitute.For<IResponseRouter>());
            await pipe.Writer.WriteAsync(Encoding.UTF8.GetBytes(data));

            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(5));
            var processTask = handler.ProcessInputStream(cts.Token);

            await pipe.Writer.FlushAsync();

            await pipe.Writer.CompleteAsync();
            await processTask;

            reciever.Received(1).IsValid(Arg.Is<JToken>(x => x.ToString() == "{}"));
        }

        [Fact]
        public async Task Should_Handle_Multiple_Requests_Back_To_Back()
        {
            var pipe = new Pipe(new PipeOptions());

            var outputHandler = Substitute.For<IOutputHandler>();
            var reciever = Substitute.For<IReceiver>();

            using var handler = NewHandler(pipe.Reader, outputHandler, reciever,
                Substitute.For<IRequestProcessIdentifier>(), Substitute.For<IRequestRouter<IHandlerDescriptor>>(),
                _loggerFactory, Substitute.For<IResponseRouter>());

            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(5));
            var processTask = handler.ProcessInputStream(cts.Token);

            await pipe.Writer.WriteAsync(Encoding.UTF8.GetBytes("Content-Length: 2\r\n\r\n{}"), cts.Token);
            await Task.Delay(20);
            await pipe.Writer.WriteAsync(Encoding.UTF8.GetBytes("Content-Length: 2\r\n\r\n{}"), cts.Token);
            await Task.Delay(20);
            await pipe.Writer.WriteAsync(Encoding.UTF8.GetBytes("Content-Length: 2\r\n\r\n{}"), cts.Token);
            await Task.Delay(20);

            await pipe.Writer.FlushAsync(cts.Token);
            await pipe.Writer.CompleteAsync();
            await processTask;

            reciever.Received(3).IsValid(Arg.Is<JToken>(x => x.ToString() == "{}"));
        }

        [Fact]
        public async Task Should_Handle_Multiple_Requests_In_Pieces()
        {
            var pipe = new Pipe(new PipeOptions());

            var outputHandler = Substitute.For<IOutputHandler>();
            var reciever = Substitute.For<IReceiver>();

            using var handler = NewHandler(pipe.Reader, outputHandler, reciever,
                Substitute.For<IRequestProcessIdentifier>(), Substitute.For<IRequestRouter<IHandlerDescriptor>>(),
                _loggerFactory, Substitute.For<IResponseRouter>());

            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(5));
            var processTask = handler.ProcessInputStream(cts.Token);

            await pipe.Writer.WriteAsync(Encoding.UTF8.GetBytes("Content-Leng"));
            await Task.Delay(50);
            await pipe.Writer.WriteAsync(Encoding.UTF8.GetBytes("th: 2\r"));
            await Task.Delay(50);
            await pipe.Writer.WriteAsync(Encoding.UTF8.GetBytes("\n\r\n{}"));
            await Task.Delay(50);
            await pipe.Writer.WriteAsync(Encoding.UTF8.GetBytes("Cont"));
            await Task.Delay(50);
            await pipe.Writer.WriteAsync(Encoding.UTF8.GetBytes("ent-Length: 2\r\n\r\n{}"));
            await Task.Delay(50);
            await pipe.Writer.WriteAsync(Encoding.UTF8.GetBytes("Content-Length: 2\r\n\r\n{"));
            await Task.Delay(50);
            await pipe.Writer.WriteAsync(Encoding.UTF8.GetBytes("}"));

            await pipe.Writer.FlushAsync();
            await pipe.Writer.CompleteAsync();
            await processTask;

            reciever.Received(3).IsValid(Arg.Is<JToken>(x => x.ToString() == "{}"));
        }

        [Theory]
        [InlineData("{\"changes\": [{\"uri\": \"file:///Mörkö.cs\",\"type\": 1}]}")]
        [InlineData("{\"textDocument\": {\"uri\": \"file://abc/123/树.cs\"}}")]
        public async Task ShouldPassAdditionalUtf8EncodedRequests(string data)
        {
            var pipe = new Pipe(new PipeOptions());

            var outputHandler = Substitute.For<IOutputHandler>();
            var reciever = Substitute.For<IReceiver>();

            using var handler = NewHandler(pipe.Reader, outputHandler, reciever,
                Substitute.For<IRequestProcessIdentifier>(), Substitute.For<IRequestRouter<IHandlerDescriptor>>(),
                _loggerFactory, Substitute.For<IResponseRouter>());

            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(5));
            var processTask = handler.ProcessInputStream(cts.Token);

            await pipe.Writer.WriteAsync(
                Encoding.UTF8.GetBytes($"Content-Length: {Encoding.UTF8.GetBytes(data).Length}\r\n\r\n{data}"));
            await Task.Yield();

            await pipe.Writer.FlushAsync();
            await pipe.Writer.CompleteAsync();
            await processTask;

            var calls = reciever.ReceivedCalls();
            var call = calls.Single();
            call.GetMethodInfo().Name.Should().Be("IsValid");
            call.GetArguments()[0].Should().BeAssignableTo<JToken>();
            var arg = call.GetArguments()[0] as JToken;
            arg.ToString().Should().Be(JToken.Parse(data).ToString());
        }

        [Fact]
        public async Task ShouldCancelRequest()
        {
            var reciever = Substitute.For<IReceiver>();
            var incomingRequestRouter = Substitute.For<IRequestRouter<IHandlerDescriptor>>();
            var requestDescription = Substitute.For<IHandlerDescriptor>();
            requestDescription.Method.Returns("abc");
            var cancelDescription = Substitute.For<IHandlerDescriptor>();
            cancelDescription.Method.Returns(JsonRpcNames.CancelRequest);

            var req = new Request(1, "abc", null);
            var cancel = new Notification(JsonRpcNames.CancelRequest, JObject.Parse("{\"id\":1}"));
            reciever.IsValid(Arg.Any<JToken>()).Returns(true);
            reciever.GetRequests(Arg.Any<JToken>())
                .Returns(c => (new Renor[] {req, cancel}, false));

            incomingRequestRouter.When(z => z.CancelRequest(Arg.Any<object>()));
            incomingRequestRouter.GetDescriptor(cancel).Returns(cancelDescription);
            incomingRequestRouter.GetDescriptor(req).Returns(requestDescription);

            incomingRequestRouter.RouteRequest(requestDescription, req, CancellationToken.None)
                .Returns(new Response(1, req));

            incomingRequestRouter.RouteNotification(cancelDescription, cancel, CancellationToken.None)
                .Returns(Task.CompletedTask);

            var pipe = new Pipe(new PipeOptions());

            var outputHandler = Substitute.For<IOutputHandler>();

            using var handler = NewHandler(pipe.Reader, outputHandler, reciever,
                Substitute.For<IRequestProcessIdentifier>(),
                incomingRequestRouter,
                _loggerFactory,
                Substitute.For<IResponseRouter>());

            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(5));
            var processTask = handler.ProcessInputStream(cts.Token);

            await pipe.Writer.WriteAsync(Encoding.UTF8.GetBytes("Content-Length: 2\r\n\r\n{}"));

            await pipe.Writer.CompleteAsync();
            await processTask;

            incomingRequestRouter.Received().CancelRequest(1L);
        }
    }
}
