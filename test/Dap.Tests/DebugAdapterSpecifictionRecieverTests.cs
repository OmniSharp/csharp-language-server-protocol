using System.Linq;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.DebugAdapter.Protocol;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Server;
using OmniSharp.Extensions.JsonRpc.Server.Messages;
using Xunit;

namespace Dap.Tests
{
    public class DebugAdapterSpecificationReceiverTests
    {
        [Theory]
        [ClassData(typeof(SpecificationMessages))]
        public void ShouldRespond_AsExpected(string json, Renor[] request)
        {
            var receiver = new DapReceiver();
            var inSerializer = new DapProtocolSerializer();
            var outSerializer = new DapProtocolSerializer();
            var (requests, _) = receiver.GetRequests(JToken.Parse(json));
            var result = requests.ToArray();
            request.Length.Should().Be(result.Length);

            for (var i = 0; i < request.Length; i++)
            {
                var r = request[i];
                var response = result[i];

                inSerializer.SerializeObject(response)
                            .Should().Be(outSerializer.SerializeObject(r));
            }
        }

        [Fact]
        public void Should_Camel_Case_As_Expected()
        {
            var serializer = new DapProtocolSerializer();
            var response = serializer.SerializeObject(
                new InitializeResponse() {
                    SupportsCancelRequest = true
                }
            );

            response.Should().Be(@"{""supportsCancelRequest"":true}");
        }

        private class SpecificationMessages : TheoryData<string, Renor[]>
        {
            public SpecificationMessages()
            {
                Add(
                    @"{""seq"": ""0"", ""type"": ""request"", ""command"": ""attach"", ""arguments"": { ""__restart"": 3 }}",
                    new Renor[] {
                        new Request(0, "attach", new JObject { { "__restart", 3 } })
                    }
                );

                Add(
                    @"{""seq"": ""1"", ""type"": ""request"", ""command"": ""attach""}",
                    new Renor[] {
                        new Request(1, "attach", new JObject())
                    }
                );

                Add(
                    @"{""seq"": ""0"", ""type"": ""event"", ""event"": ""breakpoint"", ""body"": { ""reason"": ""new"" }}",
                    new Renor[] {
                        new Notification("breakpoint", new JObject { { "reason", "new" } }),
                    }
                );

                Add(
                    @"{""seq"": ""1"", ""type"": ""event"", ""event"": ""breakpoint""}",
                    new Renor[] {
                        new Notification("breakpoint", null)
                    }
                );

                Add(
                    @"{""seq"": ""1"", ""type"": ""response"", ""request_seq"": 3, ""success"": true, ""command"": ""attach"", ""body"": {  }}",
                    new Renor[] {
                        new ServerResponse(3, new JObject()),
                    }
                );

                Add(
                    @"{""seq"": ""1"", ""type"": ""response"", ""request_seq"": 3, ""success"": true, ""command"": ""attach"", ""body"": null}",
                    new Renor[] {
                        new ServerResponse(3, null),
                    }
                );

                // Add (
                //     @"{""seq"": ""1"", ""type"": ""response"", ""request_seq"": 3, ""success"": false, ""command"": ""attach"", ""body"": {  }}",
                //     new Renor[]
                //     {
                //         new ServerError(3, new ServerErrorResult()),
                //     }
                // );

                Add(
                    @"{""seq"": ""1"", ""type"": ""response"", ""request_seq"": 3, ""success"": false, ""command"": ""attach"", ""body"": null}",
                    new Renor[] {
                        new ServerError(3, new ServerErrorResult(-1, "Unknown Error", new JObject())),
                    }
                );

                Add(
                    @"[1]",
                    new Renor[] {
                        new InvalidRequest(string.Empty, "Not an object")
                    }
                );
            }
        }

        [Theory]
        [ClassData(typeof(InvalidMessages))]
        public void Should_ValidateInvalidMessages(string json, bool expected)
        {
            var receiver = new DapReceiver();
            var result = receiver.IsValid(JToken.Parse(json));
            result.Should().Be(expected);
        }

        private class InvalidMessages : TheoryData<string, bool>
        {
            public InvalidMessages()
            {
                Add(@"[]", false);
                Add(@"""""", false);
                Add(@"1", false);
                Add(@"true", false);
                Add(@"{}", true);
            }
        }
    }
}
