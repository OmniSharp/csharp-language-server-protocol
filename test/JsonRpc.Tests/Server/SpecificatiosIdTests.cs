using System;
using System.Linq;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using Xunit;

namespace JsonRpc.Tests.Server
{
    public class SpecificatiosIdTests
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

        private class SimpleTestMessages : TheoryData<string, Type, object>
        {
            public SimpleTestMessages()
            {
                Add(
                    @"{ ""jsonrpc"": ""2.0"", ""method"": ""method1"", ""id"": ""canbestring"" }",
                    typeof(string),
                    "canbestring"
                );
                Add(
                    @"{ ""jsonrpc"": ""2.0"", ""method"": ""method1"", ""id"": 12345 }",
                    typeof(long),
                    12345L
                );
                Add(
                    @"{ ""jsonrpc"": ""2.0"", ""method"": ""method1"", ""id"": null }",
                    typeof(object),
                    null
                );
            }
        }
    }
}
