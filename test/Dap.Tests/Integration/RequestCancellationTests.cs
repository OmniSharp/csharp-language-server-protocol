using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using NSubstitute;
using OmniSharp.Extensions.DebugAdapter.Client;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using OmniSharp.Extensions.DebugAdapter.Server;
using OmniSharp.Extensions.DebugAdapter.Testing;
using OmniSharp.Extensions.JsonRpc.Server;
using OmniSharp.Extensions.JsonRpc.Testing;
using Xunit;
using Xunit.Abstractions;

namespace Dap.Tests.Integration
{
    public class RequestCancellationTests : DebugAdapterProtocolTestBase
    {
        public RequestCancellationTests(ITestOutputHelper outputHelper) : base(new JsonRpcTestOptions()
            .ConfigureForXUnit(outputHelper)
            .WithSettleTimeSpan(TimeSpan.FromMilliseconds(200))
        )
        {
        }

        [Fact]
        public async Task Should_Cancel_Pending_Requests()
        {
            var (client, server) = await Initialize(ConfigureClient, ConfigureServer);

            Func<Task<CompletionsResponse>> action = () => {
                var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(100));
                CancellationToken.Register(cts.Cancel);
                return client.RequestCompletions(new CompletionsArguments(), cts.Token);
            };
            action.Should().Throw<OperationCanceledException>();
        }

        [Fact]
        public async Task Should_Cancel_Requests_After_Timeout()
        {
            var (client, server) = await Initialize(ConfigureClient, x => {
                ConfigureServer(x);
                x.WithMaximumRequestTimeout(TimeSpan.FromMilliseconds(500));
            });

            Func<Task<CompletionsResponse>> action = () => client.RequestCompletions(new CompletionsArguments());
            action.Should().Throw<RequestCancelledException>();
        }

        private void ConfigureClient(DebugAdapterClientOptions options)
        {
        }

        private void ConfigureServer(DebugAdapterServerOptions options)
        {
            options.OnCompletions(async  (x, ct) => {
                await Task.Delay(50000, ct);
                return new CompletionsResponse();
            });
        }
    }
}
