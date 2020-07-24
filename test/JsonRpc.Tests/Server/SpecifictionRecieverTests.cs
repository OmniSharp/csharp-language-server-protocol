using System.Linq;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Server;
using OmniSharp.Extensions.JsonRpc.Server.Messages;
using Xunit;

namespace JsonRpc.Tests.Server
{
    public class SpecificationReceiverTests
    {
        [Theory]
        [ClassData(typeof(SpecificationMessages))]
        public void ShouldRespond_AsExpected2(string json, Renor[] request)
        {
            var receiver = new Receiver();
            var (requests, _) = receiver.GetRequests(JToken.Parse(json));
            var result = requests.ToArray();
            request.Length.Should().Be(result.Length);

            for (var i = 0; i < request.Length; i++)
            {
                var r = request[i];
                var response = result[i];

                JsonConvert.SerializeObject(response)
                    .Should().Be(JsonConvert.SerializeObject(r));
            }
        }

        class SpecificationMessages : TheoryData<string, Renor[]>
        {
            public SpecificationMessages()
            {
                Add (
                    @"{""jsonrpc"": ""2.0"", ""method"": ""subtract"", ""params"": [42, 23], ""id"": 1}",
                    new Renor[]
                    {
                        new Request(1, "subtract", new JArray(new [] {42, 23}))
                    }
                );

                Add (
                    @"{""jsonrpc"": ""2.0"", ""method"": ""subtract"", ""params"": {""subtrahend"": 23, ""minuend"": 42}, ""id"": 3}",
                    new Renor[]
                    {
                        new Request(3, "subtract", JObject.FromObject(new {subtrahend = 23, minuend = 42}))
                    });

                Add (
                    @"{""jsonrpc"": ""2.0"", ""method"": ""subtract"", ""params"": {""minuend"": 42, ""subtrahend"": 23 }, ""id"": 4}",
                    new Renor[]
                    {
                        new Request(4, "subtract", JObject.FromObject(new {minuend = 42, subtrahend = 23}))
                    });

                Add (
                    @"{""jsonrpc"": ""2.0"", ""method"": ""subtract"", ""id"": 4}",
                    new Renor[]
                    {
                        new Request(4, "subtract", null)
                    });

                // http://www.jsonrpc.org/specification says:
                //      If present, parameters for the rpc call MUST be provided as a Structured value.
                // Some clients may serialize params as null, instead of omitting it
                // We're going to pretend we never got the null in the first place.
                Add (
                    @"{""jsonrpc"": ""2.0"", ""method"": ""subtract"", ""params"": null, ""id"": 4}",
                    new Renor[]
                    {
                        new Request(4, "subtract", new JObject())
                    });

                Add (
                    @"{""jsonrpc"": ""2.0"", ""method"": ""update"", ""params"": [1,2,3,4,5]}",
                    new Renor[]
                    {
                        new Notification("update", new JArray(new [] {1,2,3,4,5}))
                    });

                Add (
                    @"{""jsonrpc"": ""2.0"", ""method"": ""foobar""}",
                    new Renor[]
                    {
                        new Notification("foobar", null)
                    });

                // http://www.jsonrpc.org/specification says:
                //      If present, parameters for the rpc call MUST be provided as a Structured value.
                // Some clients may serialize params as null, instead of omitting it
                // We're going to pretend we never got the null in the first place.
                Add (
                    @"{""jsonrpc"": ""2.0"", ""method"": ""foobar"", ""params"": null}",
                    new Renor[]
                    {
                        new Notification("foobar", new JObject())
                    });

                Add (
                    @"{""jsonrpc"":""2.0"",""method"":""initialized"",""params"":{}}",
                    new Renor[] {
                        new Notification("initialized", new JObject()),
                    }
                );

                Add (
                    @"{""jsonrpc"": ""2.0"", ""method"": 1, ""params"": ""bar""}",
                    new Renor[]
                    {
                        new InvalidRequest("Invalid params")
                    });

                Add (
                    @"[1]",
                    new Renor[]
                    {
                        new InvalidRequest("Not an object")
                    });

                Add (
                    @"[1,2,3]",
                    new Renor[]
                    {
                        new InvalidRequest("Not an object"),
                        new InvalidRequest("Not an object"),
                        new InvalidRequest("Not an object")
                    });

                Add (
                    @"[
                        {""jsonrpc"": ""2.0"", ""method"": ""sum"", ""params"": [1,2,4], ""id"": ""1""},
                        {""jsonrpc"": ""2.0"", ""method"": ""notify_hello"", ""params"": [7]},
                        {""jsonrpc"": ""2.0"", ""method"": ""subtract"", ""params"": [42,23], ""id"": ""2""},
                        {""foo"": ""boo""},
                        {""jsonrpc"": ""2.0"", ""method"": ""foo.get"", ""params"": {""name"": ""myself""}, ""id"": ""5""},
                        {""jsonrpc"": ""2.0"", ""method"": ""get_data"", ""id"": ""9""}
                    ]",
                    new Renor[]
                    {
                        new Request("1", "sum", new JArray(new [] {1,2,4})),
                        new Notification("notify_hello", new JArray(new [] {7})),
                        new Request("2", "subtract", new JArray(new [] {42,23})),
                        new InvalidRequest("Unexpected protocol"),
                        new Request("5", "foo.get", JObject.FromObject(new {name = "myself"})),
                        new Request("9", "get_data", null),
                    });

                Add (
                    @"[
                      {""jsonrpc"": ""2.0"", ""error"": {""code"": -32600, ""message"": ""Invalid Request""}, ""id"": null},
                      {""jsonrpc"": ""2.0"", ""error"": {""code"": -32600, ""message"": ""Invalid Request""}, ""id"": null},
                      {""jsonrpc"": ""2.0"", ""error"": {""code"": -32600, ""message"": ""Invalid Request""}, ""id"": null}
                    ]",
                    new Renor[] {
                        new ServerError(new ServerErrorResult(-32600, "Invalid Request")),
                        new ServerError(new ServerErrorResult(-32600, "Invalid Request")),
                        new ServerError(new ServerErrorResult(-32600, "Invalid Request")),
                    });
            }
        }

        [Theory]
        [ClassData(typeof(InvalidMessages))]
        public void Should_ValidateInvalidMessages(string json, bool expected)
        {
            var receiver = new Receiver();
            var result = receiver.IsValid(JToken.Parse(json));
            result.Should().Be(expected);
        }

        class InvalidMessages : TheoryData<string, bool>
        {
            public InvalidMessages()
            {
                Add (@"[]", false);
                Add (@"""""", false);
                Add (@"1", false);
                Add (@"true", false);
                Add (@"[{}]", true);
                Add (@"{}", true);
            }
        }
    }
}
