using System;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc;
using Xunit;
using Xunit.Abstractions;

namespace JsonRpc.Tests
{
    public class ServiceCollectionSupportTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public ServiceCollectionSupportTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task Should_Bootstrap_Server_Through_Service_Collection()
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(30));
            var pipe = new Pipe();
            var services = new ServiceCollection()
                .AddJsonRpcServer(options => options
                    .WithInput(pipe.Reader)
                    .WithOutput(pipe.Writer)
                    .WithServices(s => s.AddSingleton(new OutsideService(2)))
                    .AddHandler<Handler>(new JsonRpcHandlerOptions() { RequestProcessType = RequestProcessType.Serial})
                )
                .AddSingleton(new OutsideService(3))
                .AddSingleton<ILoggerFactory>(new TestLoggerFactory(_testOutputHelper))
                .BuildServiceProvider();

            var server = services.GetRequiredService<JsonRpcServer>();
            await server.Initialize(cts.Token);

            var response = await server.SendRequest(new Request(), cts.Token);
            response.Value.Should().Be(3);
            response.Value.Should().NotBe(2);

            server.Dispose();
        }

        [Method("outside")]
        class Request : IRequest<Response> {}

        class Response
        {
            public int Value { get; }

            public Response(int value)
            {
                Value = value;
            }
        }

        class Handler : IJsonRpcRequestHandler<Request, Response>
        {
            private readonly OutsideService _outsideService;

            public Handler(OutsideService outsideService)
            {
                _outsideService = outsideService;
            }
            public Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                return Task.FromResult(new Response(_outsideService.Value));
            }
        }

        class OutsideService
        {
            public OutsideService(int value)
            {
                Value = value;
            }

            public int Value { get; }
        }
    }
}
