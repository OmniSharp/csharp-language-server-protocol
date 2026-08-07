using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Serialization;
using Xunit;

namespace JsonRpc.Tests
{
    public class DelegatingHandlerTests
    {
        [Fact]
        public void Should_deserialize_raw_json_with_the_configured_serializer()
        {
            var result = new JsonRpcSerializer().DeserializeObject<JsonElement>("{\"value\":\"expected\"}");

            result.GetProperty("value").GetString().Should().Be("expected");
        }

        [Fact]
        public async Task Should_handle_raw_json_requests()
        {
            var handler = new DelegatingJsonRequestHandler(
                (value, _) => Task.FromResult(JsonTestHelper.ToElement(new { echoed = value.GetProperty("value").GetString() }))
            );

            var result = await handler.Handle(
                new DelegatingRequest<JsonElement>(JsonTestHelper.ToElement(new { value = "expected" })),
                CancellationToken.None
            );

            result.GetProperty("echoed").GetString().Should().Be("expected");
        }

        [Fact]
        public async Task Should_handle_raw_json_notifications()
        {
            string? received = null;
            var handler = new DelegatingJsonNotificationHandler(
                (value, _) =>
                {
                    received = value.GetProperty("value").GetString();
                    return Task.CompletedTask;
                }
            );

            await handler.Handle(
                new DelegatingNotification<JsonElement>(JsonTestHelper.ToElement(new { value = "expected" })),
                CancellationToken.None
            );

            received.Should().Be("expected");
        }

        [Fact]
        public async Task Should_convert_typed_request_responses_to_json_elements()
        {
            var handler = new DelegatingRequestHandler<Data, Data>(
                new JsonRpcSerializer(),
                (value, _) => Task.FromResult(new Data { Value = value.Value + " response" })
            );

            var result = await handler.Handle(
                new DelegatingRequest<Data>(JsonTestHelper.ToElement(new { value = "expected" })),
                CancellationToken.None
            );

            result.GetProperty("Value").GetString().Should().Be("expected response");
        }

        private class Data
        {
            public string Value { get; set; } = string.Empty;
        }
    }
}