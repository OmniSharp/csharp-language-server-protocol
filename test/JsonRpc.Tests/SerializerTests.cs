using System.Text.Json;
using FluentAssertions;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Client;
using OmniSharp.Extensions.JsonRpc.Serialization;
using OmniSharp.Extensions.JsonRpc.Server;
using Xunit;

namespace JsonRpc.Tests
{
    public class SerializerTests
    {
        private readonly JsonRpcSerializer _serializer = new JsonRpcSerializer();

        [Fact]
        public void Should_use_system_text_json_by_default()
        {
            var options = new JsonRpcServerOptions();

            options.Serializer.Should().BeOfType<SystemTextJsonSerializer>();
        }

        [Fact]
        public void Should_deserialize_json_values_without_exposing_the_underlying_serializer()
        {
            var value = JsonSerializer.SerializeToElement(new { value = "expected" });

            var result = _serializer.DeserializeObject<Data>(value);

            result.Value.Should().Be("expected");
        }

        [Fact]
        public void Should_preserve_declared_type_serialization_behavior()
        {
            var result = _serializer.SerializeObject(new DerivedData { Value = "expected", Extra = "excluded" }, typeof(Data));

            using var document = JsonDocument.Parse(result);
            document.RootElement.GetProperty("Value").GetString().Should().Be("expected");
            document.RootElement.GetProperty("Extra").GetString().Should().Be("excluded");
        }

        [Fact]
        public void Should_populate_an_existing_object()
        {
            var target = new Data { Value = "initial" };

            _serializer.PopulateObject("{\"Value\":\"expected\"}", target);

            target.Value.Should().Be("expected");
        }

        [Fact]
        public void System_text_json_should_match_core_json_rpc_envelopes()
        {
            var systemTextJsonSerializer = new SystemTextJsonSerializer();
            var values = new object[]
            {
                new OutgoingRequest
                {
                    Id = 1,
                    Method = "example/request",
                    Params = JsonSerializer.SerializeToElement(new { value = "expected" }),
                    TraceParent = "00-0123456789abcdef0123456789abcdef-0123456789abcdef-01",
                    TraceState = "vendor=value"
                },
                new OutgoingNotification
                {
                    Method = "example/notification",
                    Params = JsonSerializer.SerializeToElement(new object[] { 1, "two" })
                },
                new OutgoingResponse(1, JsonSerializer.SerializeToElement(new { value = "expected" }), new Request(1, "example/request", null)),
                new OutgoingResponse(2, null, new Request(2, "example/request", null))
            };

            foreach (var value in values)
            {
                using var expected = JsonDocument.Parse(_serializer.SerializeObject(value));
                using var actual = JsonDocument.Parse(systemTextJsonSerializer.SerializeObject(value));

                JsonElement.DeepEquals(actual.RootElement, expected.RootElement).Should().BeTrue($"the wire JSON for {value.GetType().Name} should match");
            }
        }

        private class Data
        {
            public string Value { get; set; } = string.Empty;
        }

        private class DerivedData : Data
        {
            public string Extra { get; set; } = string.Empty;
        }
    }
}
