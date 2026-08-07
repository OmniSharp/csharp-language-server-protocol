using FluentAssertions;
using Newtonsoft.Json.Linq;
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
        public void Should_deserialize_json_values_without_exposing_the_underlying_serializer()
        {
            var value = JObject.Parse("{\"value\":\"expected\"}");

            var result = _serializer.DeserializeObject<Data>(value);

            result.Value.Should().Be("expected");
        }

        [Fact]
        public void Should_preserve_declared_type_serialization_behavior()
        {
            var result = _serializer.SerializeObject(new DerivedData { Value = "expected", Extra = "excluded" }, typeof(Data));

            var value = JObject.Parse(result);
            value["Value"]!.Value<string>().Should().Be("expected");
            value["Extra"]!.Value<string>().Should().Be("excluded");
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
                    Params = JObject.Parse("{\"value\":\"expected\"}"),
                    TraceParent = "00-0123456789abcdef0123456789abcdef-0123456789abcdef-01",
                    TraceState = "vendor=value"
                },
                new OutgoingNotification
                {
                    Method = "example/notification",
                    Params = JArray.Parse("[1,\"two\"]")
                },
                new OutgoingResponse(1, JObject.Parse("{\"value\":\"expected\"}"), new Request(1, "example/request", null)),
                new OutgoingResponse(2, null, new Request(2, "example/request", null))
            };

            foreach (var value in values)
            {
                var expected = JToken.Parse(_serializer.SerializeObject(value));
                var actual = JToken.Parse(systemTextJsonSerializer.SerializeObject(value));

                JToken.DeepEquals(actual, expected).Should().BeTrue($"the wire JSON for {value.GetType().Name} should match");
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
