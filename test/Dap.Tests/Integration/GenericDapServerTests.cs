using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using OmniSharp.Extensions.DebugAdapter.Testing;
using OmniSharp.Extensions.JsonRpc.Testing;
using Xunit;
using Xunit.Abstractions;

namespace Dap.Tests.Integration
{
    public class GenericDapServerTests : DebugAdapterProtocolTestBase
    {
        public GenericDapServerTests(ITestOutputHelper outputHelper) : base(new JsonRpcTestOptions().ConfigureForXUnit(outputHelper))
        {
        }

        [Fact]
        public async Task Supports_Multiple_Handlers_On_A_Single_Class()
        {
            var handler = new Handler();
            var (client, server) = await Initialize(options => { }, options => { options.AddHandler(handler); });

            server.ServerSettings.SupportsStepBack.Should().Be(true);
            server.ServerSettings.SupportsStepInTargetsRequest.Should().Be(true);

            await client.RequestStepBack(new StepBackArguments());
            await client.RequestReverseContinue(new ReverseContinueArguments());
            await client.RequestStepInTargets(new StepInTargetsArguments());
            await client.RequestStepOut(new StepOutArguments());
            await client.RequestStepIn(new StepInArguments());
            await client.RequestNext(new NextArguments());

            handler.Count.Should().Be(6);
        }

        class Handler : IStepBackHandler, IStepInTargetsHandler, IStepInHandler, IStepOutHandler, INextHandler, IReverseContinueHandler
        {
            public int Count { get; set; } = 0;

            public Task<StepBackResponse> Handle(StepBackArguments request, CancellationToken cancellationToken)
            {
                Count++;
                return Task.FromResult(new StepBackResponse());
            }

            public Task<StepInTargetsResponse> Handle(StepInTargetsArguments request, CancellationToken cancellationToken)
            {
                Count++;
                return Task.FromResult(new StepInTargetsResponse());
            }

            public Task<StepInResponse> Handle(StepInArguments request, CancellationToken cancellationToken)
            {
                Count++;
                return Task.FromResult(new StepInResponse());
            }

            public Task<StepOutResponse> Handle(StepOutArguments request, CancellationToken cancellationToken)
            {
                Count++;
                return Task.FromResult(new StepOutResponse());
            }

            public Task<NextResponse> Handle(NextArguments request, CancellationToken cancellationToken)
            {
                Count++;
                return Task.FromResult(new NextResponse());
            }

            public Task<ReverseContinueResponse> Handle(ReverseContinueArguments request, CancellationToken cancellationToken)
            {
                Count++;
                return Task.FromResult(new ReverseContinueResponse());
            }
        }
    }
}
