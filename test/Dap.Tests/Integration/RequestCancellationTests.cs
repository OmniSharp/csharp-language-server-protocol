using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using OmniSharp.Extensions.DebugAdapter.Client;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using OmniSharp.Extensions.DebugAdapter.Server;
using OmniSharp.Extensions.DebugAdapter.Testing;
using OmniSharp.Extensions.JsonRpc.Server;
using OmniSharp.Extensions.JsonRpc.Testing;
using TestingUtils;
using Xunit;
using Xunit.Abstractions;

namespace Dap.Tests.Integration
{
    public class RequestCancellationTests : DebugAdapterProtocolTestBase
    {
        public RequestCancellationTests(ITestOutputHelper outputHelper) : base(new JsonRpcTestOptions().ConfigureForXUnit(outputHelper))
        {
        }

        [Fact]
        public async Task Should_Cancel_Pending_Requests()
        {
            var (client, _) = await Initialize(ConfigureClient, ConfigureServer);

            Func<Task<CompletionsResponse>> action = () => {
                var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(100));
                CancellationToken.Register(cts.Cancel);
                return client.RequestCompletions(new CompletionsArguments(), cts.Token);
            };
            action.Should().Throw<OperationCanceledException>();
        }

        [Fact(Skip = "Needs Work")]
        public void Should_Cancel_Requests_After_Timeout()
        {
            Func<Task<CompletionsResponse>> action = async () => {
                var (client, _) = await Initialize(
                    ConfigureClient, x => {
                        ConfigureServer(x);
                        x.WithMaximumRequestTimeout(TimeSpan.FromMilliseconds(1000));
                    }
                );

                return await client.RequestCompletions(new CompletionsArguments());
            };
            action.Should().Throw<RequestCancelledException>();
        }

        private void ConfigureClient(DebugAdapterClientOptions options)
        {
        }

        private void ConfigureServer(DebugAdapterServerOptions options) =>
            options.OnCompletions(
                async (x, ct) => {
                    await Task.Delay(50000, ct);
                    return new CompletionsResponse();
                }
            );
    }
}
