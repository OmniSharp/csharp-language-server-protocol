using NSubstitute;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Testing;
using OmniSharp.Extensions.LanguageProtocol.Testing;
using Serilog.Events;
using Xunit.Abstractions;

namespace Lsp.Integration.Tests
{
    public class ActivityTracingTests : LanguageProtocolTestBase
    {
        public ActivityTracingTests(ITestOutputHelper outputHelper) : base(new JsonRpcTestOptions().ConfigureForXUnit(outputHelper, LogEventLevel.Verbose))
        {
        }

        [Fact]
        public async Task Should_Have_Activity_Information()
        {
            var clientStub = Substitute.For<IActivityTracingStrategy>();
            clientStub.ApplyInbound(Arg.Any<ITraceData>()).Returns(System.Reactive.Disposables.Disposable.Empty);
            var serverStub = Substitute.For<IActivityTracingStrategy>();
            serverStub.ApplyInbound(Arg.Any<ITraceData>()).Returns(System.Reactive.Disposables.Disposable.Empty);

            var (client, server) = await Initialize(
                options => options.WithActivityTracingStrategy(clientStub),
                options => options.WithActivityTracingStrategy(serverStub)
            );

            serverStub.Received().ApplyInbound(Arg.Any<ITraceData>());
            clientStub.Received().ApplyOutgoing(Arg.Any<ITraceData>());
        }

        [Fact]
        public async Task Should_Have_Activity_Information_Exchanging_Data()
        {
            var clientStub = Substitute.For<IActivityTracingStrategy>();
            clientStub.ApplyInbound(Arg.Any<ITraceData>()).Returns(System.Reactive.Disposables.Disposable.Empty);
            var serverStub = Substitute.For<IActivityTracingStrategy>();
            serverStub.ApplyInbound(Arg.Any<ITraceData>()).Returns(System.Reactive.Disposables.Disposable.Empty);

            var (client, server) = await Initialize(
                options => options
                          .WithActivityTracingStrategy(clientStub)
                          .OnRequest("test", (Func<CancellationToken, Task>)( ct => Task.CompletedTask )),
                options => options
                          .WithActivityTracingStrategy(serverStub)
                          .OnRequest("test", (Func<CancellationToken, Task>)( ct => Task.CompletedTask ))
            );

            await client.SendRequest("test").ReturningVoid(CancellationToken);
            await server.SendRequest("test").ReturningVoid(CancellationToken);

            serverStub.Received().ApplyInbound(Arg.Any<ITraceData>());
            clientStub.Received().ApplyOutgoing(Arg.Any<ITraceData>());
            serverStub.Received().ApplyOutgoing(Arg.Any<ITraceData>());
            clientStub.Received().ApplyInbound(Arg.Any<ITraceData>());
        }
    }
}
