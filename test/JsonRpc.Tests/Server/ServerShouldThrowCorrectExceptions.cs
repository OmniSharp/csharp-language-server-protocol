using System;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc.Server;
using OmniSharp.Extensions.JsonRpc.Testing;
using Xunit;
using Xunit.Abstractions;

namespace JsonRpc.Tests.Server
{
    public class ServerShouldThrowCorrectExceptions : JsonRpcServerTestBase
    {
        public ServerShouldThrowCorrectExceptions(ITestOutputHelper outputHelper) : base(new JsonRpcTestOptions().ConfigureForXUnit(outputHelper))
        {
        }

        private class Data
        {
            public string Value { get; set; } = null!;
        }

        [Fact]
        public async Task Should_Throw_Method_Not_Supported()
        {
            var (client, _) = await Initialize(
                clientOptions => { },
                serverOptions => { serverOptions.OnRequest("method", async (Data data) => new Data { Value = data.Value }); }
            );

            Func<Task> action = () => client.SendRequest(
                "method2", new Data {
                    Value = "Echo"
                }
            ).Returning<Data>(CancellationToken);
            await action.Should().ThrowAsync<MethodNotSupportedException>();
        }
    }
}
