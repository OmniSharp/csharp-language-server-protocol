using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Server;
using OmniSharp.Extensions.JsonRpc.Testing;
using Xunit;
using Xunit.Abstractions;

namespace JsonRpc.Tests.Server
{
    public class SpecifictionIdTests
    {
        [Theory]
        [ClassData(typeof(SimpleTestMessages))]
        public void ShouldParse_SimpleMessages(string message, Type outputType, object expectedResult)
        {
            var receiver = new Receiver();
            var (requests, _) = receiver.GetRequests(JToken.Parse(message));
            var result = requests.Single().Request;

            result.Id.Should().Be(expectedResult);
            if (expectedResult != null)
            {
                result.Id.Should().BeOfType(outputType);
            }
        }

        class SimpleTestMessages : TheoryData<string, Type, object>
        {
            public SimpleTestMessages()
            {
                Add (
                    @"{ ""jsonrpc"": ""2.0"", ""method"": ""method1"", ""id"": ""canbestring"" }",
                    typeof(string),
                    "canbestring" as object);
                Add (
                    @"{ ""jsonrpc"": ""2.0"", ""method"": ""method1"", ""id"": 12345 }",
                    typeof(long),
                    12345L as object);
                Add (
                    @"{ ""jsonrpc"": ""2.0"", ""method"": ""method1"", ""id"": null }",
                    typeof(object),
                    (object)null);
            }
        }
    }

    public class ServerShouldThrowCorrectExceptions : JsonRpcServerTestBase
    {
        public ServerShouldThrowCorrectExceptions(ITestOutputHelper outputHelper) : base(new JsonRpcTestOptions().ConfigureForXUnit(outputHelper))
        {
        }

        class Data
        {
            public string Value { get; set; }
        }

        [Fact]
        public async Task Should_Throw_Method_Not_Supported()
        {
            var (client, server) = await Initialize(
                clientOptions => {
                },
                serverOptions => {
                    serverOptions.OnRequest("method", async (Data data) => new Data() { Value = data.Value});
                });

            Func<Task> action = () => client.SendRequest("method2", new Data() {
                Value = "Echo"
            }).Returning<Data>(CancellationToken);
            await action.Should().ThrowAsync<MethodNotSupportedException>();
        }
    }
}
