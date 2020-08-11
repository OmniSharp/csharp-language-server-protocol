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

        public ServiceCollectionSupportTests(ITestOutputHelper testOutputHelper) => _testOutputHelper = testOutputHelper;

        [Fact]
        public async Task Should_Bootstrap_Server_Through_Service_Collection()
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(30));
            var services = new ServiceCollection()
                          .AddJsonRpcServer(
                               options => {
                                   var pipe = new Pipe();
                                   options
                                      .WithInput(pipe.Reader)
                                      .WithOutput(pipe.Writer)
                                      .AddHandler<Handler>(new JsonRpcHandlerOptions { RequestProcessType = RequestProcessType.Serial });
                               }
                           )
                          .AddSingleton(new OutsideService("servername"))
                          .AddSingleton<ILoggerFactory>(new TestLoggerFactory(_testOutputHelper))
                          .BuildServiceProvider();

            var server = services.GetRequiredService<JsonRpcServer>();
            await server.Initialize(cts.Token);

            var response = await server.SendRequest(new Request(), cts.Token);
            response.Value.Should().Be("servername");
            response.Value.Should().NotBe("servernamnot");

            server.Dispose();
        }

        [Fact]
        public async Task Inner_Services_Should_Override_Outer_Services()
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(30));
            var services = new ServiceCollection()
                          .AddJsonRpcServer(
                               options => {
                                   var pipe = new Pipe();
                                   options
                                      .WithInput(pipe.Reader)
                                      .WithOutput(pipe.Writer)
                                      .WithServices(s => s.AddSingleton(new OutsideService("override")))
                                      .AddHandler<Handler>(new JsonRpcHandlerOptions { RequestProcessType = RequestProcessType.Serial });
                               }
                           )
                          .AddSingleton(new OutsideService("servername"))
                          .AddSingleton<ILoggerFactory>(new TestLoggerFactory(_testOutputHelper))
                          .BuildServiceProvider();

            var server = services.GetRequiredService<JsonRpcServer>();
            await server.Initialize(cts.Token);

            var response = await server.SendRequest(new Request(), cts.Token);
            response.Value.Should().Be("override");

            server.Dispose();
        }

        [Fact]
        public async Task Handlers_Can_Be_Added_From_The_Service_Collection()
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(30));
            var services = new ServiceCollection()
                          .AddJsonRpcServer(
                               options => {
                                   var pipe = new Pipe();
                                   options
                                      .WithInput(pipe.Reader)
                                      .WithOutput(pipe.Writer)
                                      .WithServices(
                                           services =>
                                               services.AddJsonRpcHandler<Handler>(new JsonRpcHandlerOptions { RequestProcessType = RequestProcessType.Serial })
                                       );
                               }
                           )
                          .AddSingleton(new OutsideService("servername"))
                          .AddSingleton<ILoggerFactory>(new TestLoggerFactory(_testOutputHelper))
                          .BuildServiceProvider();

            var server = services.GetRequiredService<JsonRpcServer>();
            await server.Initialize(cts.Token);

            var response = await server.SendRequest(new Request(), cts.Token);
            response.Value.Should().Be("servername");

            server.Dispose();
        }

        [Fact]
        public async Task Handlers_Can_Access_External_Service_Provider()
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(30));
            var services = new ServiceCollection()
                          .AddJsonRpcServer(
                               options => {
                                   var pipe = new Pipe();
                                   options
                                      .WithInput(pipe.Reader)
                                      .WithOutput(pipe.Writer)
                                      .WithServices(
                                           services =>
                                               services
                                                  .AddSingleton(new OutsideService("inside"))
                                                  .AddJsonRpcHandler<Handler>(new JsonRpcHandlerOptions { RequestProcessType = RequestProcessType.Serial })
                                                  .AddJsonRpcHandler<ExternalHandler>(new JsonRpcHandlerOptions { RequestProcessType = RequestProcessType.Serial })
                                       );
                               }
                           )
                          .AddSingleton(new OutsideService("outside"))
                          .AddSingleton<ILoggerFactory>(new TestLoggerFactory(_testOutputHelper))
                          .BuildServiceProvider();

            var server = services.GetRequiredService<JsonRpcServer>();
            await server.Initialize(cts.Token);

            var response = await server.SendRequest(new Request(), cts.Token);
            response.Value.Should().Be("inside");

            var response2 = await server.SendRequest(new ExternalRequest(), cts.Token);
            response2.Value.Should().Be("outside");

            server.Dispose();
        }

        [Fact]
        public async Task Handlers_Can_Access_External_Service_Provider_And_Access_New_Values()
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(30));
            var count = 1;
            var services = new ServiceCollection()
                          .AddJsonRpcServer(
                               options => {
                                   var pipe = new Pipe();
                                   options
                                      .WithInput(pipe.Reader)
                                      .WithOutput(pipe.Writer)
                                      .WithServices(
                                           services =>
                                               services
                                                  .AddSingleton(new OutsideService("inside"))
                                                  .AddJsonRpcHandler<Handler>(new JsonRpcHandlerOptions { RequestProcessType = RequestProcessType.Serial })
                                                  .AddJsonRpcHandler<ExternalHandler>(new JsonRpcHandlerOptions { RequestProcessType = RequestProcessType.Serial })
                                       );
                               }
                           )
                          .AddTransient(_ => new OutsideService($"outside{count++}"))
                          .AddSingleton<ILoggerFactory>(new TestLoggerFactory(_testOutputHelper))
                          .BuildServiceProvider();

            var server = services.GetRequiredService<JsonRpcServer>();
            await server.Initialize(cts.Token);

            var response = await server.SendRequest(new Request(), cts.Token);
            response.Value.Should().Be("inside");

            var response2 = await server.SendRequest(new ExternalRequest(), cts.Token);
            response2.Value.Should().Be("outside1");

            var response3 = await server.SendRequest(new ExternalRequest(), cts.Token);
            response3.Value.Should().Be("outside2");

            server.Dispose();
        }

        [Fact]
        public void Should_Bootstrap_Multiple_Servers_Through_Service_Collection()
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(30));
            var services = new ServiceCollection()
                          .AddJsonRpcServer(
                               "serial", options => {
                                   var pipe = new Pipe();
                                   options
                                      .WithInput(pipe.Reader)
                                      .WithOutput(pipe.Writer)
                                      .AddHandler<Handler>(new JsonRpcHandlerOptions { RequestProcessType = RequestProcessType.Serial });
                               }
                           )
                          .AddJsonRpcServer(
                               "parallel", options => {
                                   var pipe = new Pipe();
                                   options
                                      .WithInput(pipe.Reader)
                                      .WithOutput(pipe.Writer)
                                      .AddHandler<Handler>(new JsonRpcHandlerOptions { RequestProcessType = RequestProcessType.Parallel });
                               }
                           )
                          .AddSingleton(new OutsideService("outside"))
                          .AddSingleton<ILoggerFactory>(new TestLoggerFactory(_testOutputHelper))
                          .BuildServiceProvider();

            var resolver = services.GetRequiredService<JsonRpcServerResolver>();
            var serialServer = resolver.Get("serial").Should().NotBeNull().And.Subject;
            var parallelServer = resolver.Get("parallel").Should().NotBeNull().And.Subject;
            serialServer.Should().NotBe(parallelServer);

            resolver.Get("serial").Should().Be(serialServer);
            resolver.Get("parallel").Should().Be(parallelServer);
        }

        [Fact]
        public void Should_Throw_When_Multiple_Servers_Are_Added_And_Attempt_To_Resolve_Server()
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(30));

            var services = new ServiceCollection()
                          .AddJsonRpcServer(
                               "serial", options => {
                                   var pipe = new Pipe();
                                   options
                                      .WithInput(pipe.Reader)
                                      .WithOutput(pipe.Writer)
                                      .AddHandler<Handler>(new JsonRpcHandlerOptions { RequestProcessType = RequestProcessType.Serial });
                               }
                           )
                          .AddJsonRpcServer(
                               "parallel", options => {
                                   var pipe = new Pipe();
                                   options
                                      .WithInput(pipe.Reader)
                                      .WithOutput(pipe.Writer)
                                      .AddHandler<Handler>(new JsonRpcHandlerOptions { RequestProcessType = RequestProcessType.Parallel });
                               }
                           )
                          .AddSingleton<ILoggerFactory>(new TestLoggerFactory(_testOutputHelper))
                          .BuildServiceProvider();

            Action a = () => services.GetRequiredService<JsonRpcServer>();
            a.Should().Throw<NotSupportedException>();
        }

        [Method("outside")]
        private class Request : IRequest<Response>
        {
        }

        [Method("ext-outside")]
        private class ExternalRequest : IRequest<Response>
        {
        }

        private class Response
        {
            public string Value { get; }

            public Response(string value) => Value = value;
        }

        private class Handler : IJsonRpcRequestHandler<Request, Response>
        {
            private readonly OutsideService _outsideService;

            public Handler(OutsideService outsideService) => _outsideService = outsideService;

            public Task<Response> Handle(Request request, CancellationToken cancellationToken) => Task.FromResult(new Response(_outsideService.Value));
        }

        private class ExternalHandler : IJsonRpcRequestHandler<ExternalRequest, Response>
        {
            private readonly IExternalServiceProvider _outsideService;

            public ExternalHandler(IExternalServiceProvider outsideService) => _outsideService = outsideService;

            public Task<Response> Handle(ExternalRequest request, CancellationToken cancellationToken) =>
                Task.FromResult(new Response(_outsideService.GetRequiredService<OutsideService>().Value));
        }

        private class OutsideService
        {
            public OutsideService(string value) => Value = value;

            public string Value { get; }
        }
    }
}
