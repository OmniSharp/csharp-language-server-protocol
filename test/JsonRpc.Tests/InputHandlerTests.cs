using System;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Client;
using OmniSharp.Extensions.JsonRpc.Serialization;
using OmniSharp.Extensions.JsonRpc.Server;
using Xunit;
using Xunit.Abstractions;

namespace JsonRpc.Tests
{
    [Collection("InputHandlers")]
    public class InputHandlerTests
    {
        private readonly TestLoggerFactory _loggerFactory;
        private readonly OnUnhandledExceptionHandler _unhandledException = Substitute.For<OnUnhandledExceptionHandler>();

        public InputHandlerTests(ITestOutputHelper testOutputHelper) => _loggerFactory = new TestLoggerFactory(testOutputHelper);

        private InputHandler NewHandler(
            PipeReader inputStream,
            IOutputHandler outputHandler,
            IReceiver receiver,
            IRequestProcessIdentifier requestProcessIdentifier,
            IRequestRouter<IHandlerDescriptor?> requestRouter,
            ILoggerFactory loggerFactory,
            IResponseRouter responseRouter,
            IScheduler? scheduler = null
        ) =>
            new InputHandler(
                inputStream,
                outputHandler,
                receiver,
                requestProcessIdentifier,
                requestRouter,
                responseRouter,
                loggerFactory,
                _unhandledException,
                null,
                TimeSpan.FromSeconds(30),
                true,
                null,
                scheduler ?? TaskPoolScheduler.Default
            );

        [Fact]
        public async Task Should_Pass_In_Requests()
        {
            var pipe = new Pipe(new PipeOptions());

            var outputHandler = Substitute.For<IOutputHandler>();
            var receiver = Substitute.For<IReceiver>();

            using var handler = NewHandler(
                pipe.Reader, outputHandler, receiver,
                Substitute.For<IRequestProcessIdentifier>(),
                Substitute.For<IRequestRouter<IHandlerDescriptor?>>(),
                _loggerFactory, Substitute.For<IResponseRouter>()
            );
            await pipe.Writer.WriteAsync(Encoding.UTF8.GetBytes("Content-Length: 2\r\n\r\n{}"));

            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(5));
            var processTask = handler.ProcessInputStream(cts.Token);

            await pipe.Writer.CompleteAsync();
            await processTask;

            receiver.Received().IsValid(Arg.Is<JToken>(x => x.ToString() == "{}"));
        }

        [Fact]
        public async Task Should_Handle_Multiple_Requests_At_Once()
        {
            var pipe = new Pipe(new PipeOptions());

            var outputHandler = Substitute.For<IOutputHandler>();
            var receiver = Substitute.For<IReceiver>();

            using var handler = NewHandler(
                pipe.Reader, outputHandler, receiver,
                Substitute.For<IRequestProcessIdentifier>(),
                Substitute.For<IRequestRouter<IHandlerDescriptor?>>(),
                _loggerFactory, Substitute.For<IResponseRouter>()
            );
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

            receiver.Received(3).IsValid(Arg.Is<JToken>(x => x.ToString() == "{}"));
        }

        [Theory]
        [InlineData(
            "Content-Length:                    2                       \r\nContent-Type: application/json\r\n\r\n{}"
        )]
        [InlineData("Content-Type: application/json\r\nContent-Length: 2\r\n\r\n{}")]
        [InlineData("Content-Type: application/json\r\nNot-A-Header: really\r\nContent-Length: 2\r\n\r\n{}")]
        [InlineData(
            "Content-Type: application/json\r\nNot-A-Header: really\r\nContent-Length:2                                                                                               \r\n\r\n{}"
        )]
        [InlineData(
            "Content-Type: application/json\r\nNot-A-Header: really\r\nContent-Length:                                                                                                2\r\n\r\n{}"
        )]
        public async Task Should_Handle_Different_Additional_Headers_and_Whitespace(string data)
        {
            var pipe = new Pipe(new PipeOptions());

            var outputHandler = Substitute.For<IOutputHandler>();
            var receiver = Substitute.For<IReceiver>();

            using var handler = NewHandler(
                pipe.Reader, outputHandler, receiver,
                Substitute.For<IRequestProcessIdentifier>(),
                Substitute.For<IRequestRouter<IHandlerDescriptor?>>(),
                _loggerFactory, Substitute.For<IResponseRouter>()
            );
            await pipe.Writer.WriteAsync(Encoding.UTF8.GetBytes(data));

            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(5));
            var processTask = handler.ProcessInputStream(cts.Token);

            await pipe.Writer.FlushAsync();

            await pipe.Writer.CompleteAsync();
            await processTask;

            receiver.Received(1).IsValid(Arg.Is<JToken>(x => x.ToString() == "{}"));
        }

        [Fact]
        public async Task Should_Handle_Multiple_Requests_Back_To_Back()
        {
            var pipe = new Pipe(new PipeOptions());

            var outputHandler = Substitute.For<IOutputHandler>();
            var receiver = Substitute.For<IReceiver>();

            using var handler = NewHandler(
                pipe.Reader, outputHandler, receiver,
                Substitute.For<IRequestProcessIdentifier>(),
                Substitute.For<IRequestRouter<IHandlerDescriptor?>>(),
                _loggerFactory, Substitute.For<IResponseRouter>()
            );

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

            receiver.Received(3).IsValid(Arg.Is<JToken>(x => x.ToString() == "{}"));
        }

        [Fact]
        public async Task Should_Handle_Multiple_Requests_In_Pieces()
        {
            var pipe = new Pipe(new PipeOptions());

            var outputHandler = Substitute.For<IOutputHandler>();
            var receiver = Substitute.For<IReceiver>();

            using var handler = NewHandler(
                pipe.Reader, outputHandler, receiver,
                Substitute.For<IRequestProcessIdentifier>(),
                Substitute.For<IRequestRouter<IHandlerDescriptor?>>(),
                _loggerFactory, Substitute.For<IResponseRouter>()
            );

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

            receiver.Received(3).IsValid(Arg.Is<JToken>(x => x.ToString() == "{}"));
        }

        [Theory]
        [InlineData(
            "Content-Length: 894\r\n\r\n{\"edit\":{\"documentChanges\":[{\"textDocument\":{\"version\":1,\"uri\":\"file:///abc/123/d.cs\"},\"edits\":[{\"range\":{\"start\":{\"line\":1,\"character\":1},\"end\":{\"line\":2,\"character\":2}},\"newText\":\"new text\"},{\"range\":{\"start\":{\"line\":3,\"character\":3},\"end\":{\"line\":4,\"character\":4}},\"newText\":\"new text2\"}]},{\"textDocument\":{\"version\":1,\"uri\":\"file:///abc/123/b.cs\"},\"edits\":[{\"range\":{\"start\":{\"line\":1,\"character\":1},\"end\":{\"line\":2,\"character\":2}},\"newText\":\"new text2\"},{\"range\":{\"start\":{\"line\":3,\"character\":3},\"end\":{\"line\":4,\"character\":4}},\"newText\":\"new text3\"}]},{\"kind\":\"create\",\"uri\":\"file:///abc/123/b.cs\",\"options\":{\"overwrite\":true,\"ignoreIfExists\":true}},{\"kind\":\"rename\",\"oldUri\":\"file:///abc/123/b.cs\",\"newUri\":\"file:///abc/123/c.cs\",\"options\":{\"overwrite\":true,\"ignoreIfExists\":true}},{\"kind\":\"delete\",\"uri\":\"file:///abc/123/c.cs\",\"options\":{\"recursive\":false,\"ignoreIfNotExists\":true}}]}}"
        )]
        public async Task Should_Handle_Multiple_Chunked_Requests(string content)
        {
            var pipe = new Pipe(new PipeOptions());

            var outputHandler = Substitute.For<IOutputHandler>();
            var receiver = Substitute.For<IReceiver>();

            using var handler = NewHandler(
                pipe.Reader, outputHandler, receiver,
                Substitute.For<IRequestProcessIdentifier>(),
                Substitute.For<IRequestRouter<IHandlerDescriptor?>>(),
                _loggerFactory, Substitute.For<IResponseRouter>()
            );

            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromMinutes(2));
            var processTask = handler.ProcessInputStream(cts.Token);

            for (var i = 0; i < content.Length; i += 3)
            {
                await pipe.Writer.FlushAsync();
                await pipe.Writer.WriteAsync(Encoding.UTF8.GetBytes(content.Substring(i, Math.Min(3, content.Length - i))));
                await Task.Delay(5);
            }

            await pipe.Writer.FlushAsync();
            await pipe.Writer.CompleteAsync();
            await processTask;

            receiver.Received(1).IsValid(Arg.Any<JToken>());
        }

        [Fact]
        public async Task Should_Handle_Header_Terminiator_Being_Incomplete()
        {
            var pipe = new Pipe(new PipeOptions(readerScheduler: PipeScheduler.ThreadPool, writerScheduler: PipeScheduler.Inline, useSynchronizationContext: false));

            var outputHandler = Substitute.For<IOutputHandler>();
            var receiver = Substitute.For<IReceiver>();

            using var handler = NewHandler(
                pipe.Reader, outputHandler, receiver,
                Substitute.For<IRequestProcessIdentifier>(),
                Substitute.For<IRequestRouter<IHandlerDescriptor?>>(),
                _loggerFactory, Substitute.For<IResponseRouter>()
            );

            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromMinutes(5));
            var processTask = handler.ProcessInputStream(cts.Token);

            await pipe.Writer.WriteAsync(Encoding.UTF8.GetBytes("Content-Leng"));
            await pipe.Writer.FlushAsync();
            await Task.Delay(50);
            await pipe.Writer.WriteAsync(Encoding.UTF8.GetBytes("th: 2\r"));
            await pipe.Writer.FlushAsync();
            await Task.Delay(50);
            await pipe.Writer.WriteAsync(Encoding.UTF8.GetBytes("\n\r\n{}"));
            await pipe.Writer.FlushAsync();
            await Task.Delay(50);
            await pipe.Writer.WriteAsync(Encoding.UTF8.GetBytes("Cont"));
            await pipe.Writer.FlushAsync();
            await Task.Delay(50);
            await pipe.Writer.WriteAsync(Encoding.UTF8.GetBytes("ent-Length: 2\r\n\r\n{}"));
            await pipe.Writer.FlushAsync();
            await Task.Delay(50);
            await pipe.Writer.WriteAsync(Encoding.UTF8.GetBytes("Content-Length: 2\r\n\r"));
            await pipe.Writer.FlushAsync();
            await Task.Delay(50);
            await pipe.Writer.WriteAsync(Encoding.UTF8.GetBytes("\n{"));
            await pipe.Writer.FlushAsync();
            await Task.Delay(50);
            await pipe.Writer.WriteAsync(Encoding.UTF8.GetBytes("}"));
            await pipe.Writer.FlushAsync();

            await pipe.Writer.CompleteAsync();
            await processTask;

            receiver.Received(3).IsValid(Arg.Is<JToken>(x => x.ToString() == "{}"));
        }

        [Theory]
        // Mörkö
        [InlineData("{\"changes\": [{\"uri\": \"file:///M%C3%B6rk%C3%B6.cs\",\"type\": 1}]}")]
        // 树
        [InlineData("{\"textDocument\": {\"uri\": \"file://abc/123/%E6%A0%91.cs\"}}")]
        public async Task ShouldPassAdditionalUtf8EncodedRequests(string data)
        {
            var pipe = new Pipe(new PipeOptions());

            var outputHandler = Substitute.For<IOutputHandler>();
            var receiver = Substitute.For<IReceiver>();

            using var handler = NewHandler(
                pipe.Reader, outputHandler, receiver,
                Substitute.For<IRequestProcessIdentifier>(),
                Substitute.For<IRequestRouter<IHandlerDescriptor?>>(),
                _loggerFactory, Substitute.For<IResponseRouter>()
            );

            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(5));
            var processTask = handler.ProcessInputStream(cts.Token);

            await pipe.Writer.WriteAsync(
                Encoding.UTF8.GetBytes($"Content-Length: {Encoding.UTF8.GetBytes(data).Length}\r\n\r\n{data}")
            );
            await Task.Yield();

            await pipe.Writer.FlushAsync();
            await pipe.Writer.CompleteAsync();
            await processTask;

            var calls = receiver.ReceivedCalls();
            var call = calls.Single();
            call.GetMethodInfo().Name.Should().Be("IsValid");
            call.GetArguments()[0].Should().BeAssignableTo<JToken>();
            var arg = call.GetArguments()[0] as JToken;
            arg!.ToString().Should().Be(JToken.Parse(data).ToString());
        }

        [Theory]
        [ClassData(typeof(JsonRpcLogs))]
        public async Task Should_Parse_Logs(string name, Func<PipeReader> createPipeReader, ILookup<string, string> messageTypes)
        {
            var logger = _loggerFactory.CreateLogger<InputHandlerTests>();
            using var scope = logger.BeginScope(name);

            logger.LogInformation("Start");

            var reader = createPipeReader();
            var receiver = new Receiver();
            var incomingRequestRouter = Substitute.For<IRequestRouter<IHandlerDescriptor?>>();
            var outputHandler = Substitute.For<IOutputHandler>();
            var responseRouter = Substitute.For<IResponseRouter>();

            using var handler = NewHandler(
                reader, outputHandler, receiver,
                new ParallelRequestProcessIdentifier(),
                incomingRequestRouter,
                _loggerFactory,
                responseRouter
            );

            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromMinutes(1));
            var processTask = handler.ProcessInputStream(cts.Token);

            await processTask;

            foreach (var group in messageTypes)
            {
                {
                    var count = group.Count(x => x == "request");
                    await incomingRequestRouter.Received(count).RouteRequest(
                        Arg.Any<IRequestDescriptor<IHandlerDescriptor>>(),
                        Arg.Is<Request>(n => group.Key == n.Method),
                        Arg.Any<CancellationToken>()
                    );
                }


                {
                    var count = group.Count(x => x == "notification");
                    await incomingRequestRouter.Received(count).RouteNotification(
                        Arg.Any<IRequestDescriptor<IHandlerDescriptor>>(),
                        Arg.Is<Notification>(n => group.Key == n.Method),
                        Arg.Any<CancellationToken>()
                    );
                }
            }

            logger.LogInformation("End");
        }

        private class JsonRpcLogs : TheoryData<string, Func<PipeReader>, ILookup<string, string>>
        {
            public JsonRpcLogs()
            {
                var assembly = GetType().Assembly;
                foreach (var streamName in assembly.GetManifestResourceNames().Where(z => z.EndsWith(".jsrpc")))
                {
                    var data = GetData(assembly, streamName);

                    var msgTypes = data
                                  .Select(
                                       z => {
                                           if (z.MsgKind.EndsWith("response"))
                                           {
                                               return ( type: "response", kind: z.MsgType );
                                           }

                                           if (z.MsgKind.EndsWith("request"))
                                           {
                                               return ( type: "request", kind: z.MsgType );
                                           }

                                           if (z.MsgKind.EndsWith("notification") && z.MsgType != JsonRpcNames.CancelRequest)
                                           {
                                               return ( type: "notification", kind: z.MsgType );
                                           }

                                           return ( type: null, kind: null );
                                       }
                                   )
                                  .Where(z => z.type != null)
                                  .ToLookup(z => z.kind!, z => z.type!);

                    Add(streamName, () => CreateReader(data), msgTypes!);
                }
            }

            private DataItem[] GetData(Assembly assembly, string name)
            {
                var stream = assembly.GetManifestResourceStream(name);
                using var streamReader = new StreamReader(stream!);
                using var jsonReader = new JsonTextReader(streamReader);
                var serializer = new JsonSerializer();
                return serializer.Deserialize<DataItem[]>(jsonReader);
            }

            private PipeReader CreateReader(DataItem[] data)
            {
                var outputData = data
                   .Select<DataItem, object>(
                        z => {
                            if (z.MsgKind.EndsWith("response"))
                            {
                                return new OutgoingResponse(z.MsgId, z.Arg, new Request(z.MsgId, z.MsgType, JValue.CreateNull()));
                            }

                            if (z.MsgKind.EndsWith("request"))
                            {
                                return new OutgoingRequest {
                                    Id = z.MsgId,
                                    Method = z.MsgType,
                                    Params = z.Arg
                                };
                            }

                            if (z.MsgKind.EndsWith("notification"))
                            {
                                return new OutgoingNotification {
                                    Method = z.MsgType,
                                    Params = z.Arg
                                };
                            }

                            throw new NotSupportedException("unknown message kind " + z.MsgKind);
                        }
                    );

                var pipeIn = new Pipe();

                var serializer = new JsonRpcSerializer();

                Task.Run(
                    async () => {
                        foreach (var item in outputData)
                        {
                            var content = serializer.SerializeObject(item);
                            var contentBytes = Encoding.UTF8.GetBytes(content).AsMemory();

                            await pipeIn.Writer.WriteAsync(
                                Encoding.UTF8.GetBytes($"Content-Length: {contentBytes.Length}\r\n\r\n")
                            );
                            await pipeIn.Writer.WriteAsync(contentBytes);
                            await pipeIn.Writer.FlushAsync();
                        }

                        await pipeIn.Writer.CompleteAsync();
                    }
                );


                return pipeIn.Reader;
            }

            private class DataItem
            {
                // ReSharper disable once UnusedMember.Local
                public string Time { get; set; } = null!;

                // ReSharper disable once UnusedMember.Local
                public string Msg { get; set; } = null!;
                public string MsgKind { get; set; } = null!;
                public string MsgType { get; set; } = null!;
                public string MsgId { get; set; } = null!;
                public JToken Arg { get; set; } = null!;
            }
        }
    }
}
