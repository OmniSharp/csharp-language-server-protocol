using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using Xunit;

namespace JsonRpc.Tests.Server
{
    public class SpecifictionIdTests
    {
        [Theory]
        [ClassData(typeof(SimpleTestMessages))]
        public void ShouldParse_SimpleMessages(string message, Type outputType, object expectedResult)
        {
            var reciever = new Reciever();
            var (requests, _) = reciever.GetRequests(JToken.Parse(message));
            var result = requests.Single().Request;

            result.Id.Should().Be(expectedResult);
            if (expectedResult != null)
            {
                result.Id.Should().BeOfType(outputType);
            }
        }

        class SimpleTestMessages : TheoryData<string, Type, object>
        {
            public override IEnumerable<ValueTuple<string, Type, object>> GetValues()
            {
                yield return (
                    @"{ ""jsonrpc"": ""2.0"", ""method"": ""method1"", ""id"": ""canbestring"" }",
                    typeof(string),
                    "canbestring" as object);
                yield return (
                    @"{ ""jsonrpc"": ""2.0"", ""method"": ""method1"", ""id"": 12345 }",
                    typeof(long),
                    12345L as object);
                yield return (
                    @"{ ""jsonrpc"": ""2.0"", ""method"": ""method1"", ""id"": null }",
                    typeof(object),
                    (object)null);
            }
        }
    }
}
