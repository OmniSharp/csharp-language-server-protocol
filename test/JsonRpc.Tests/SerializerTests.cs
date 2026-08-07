using FluentAssertions;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc.Serialization;
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
