using System.Linq;
using System.Text.Json;
using FluentAssertions;
using OmniSharp.Extensions.DebugAdapter.Protocol;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
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
            var inSerializer = new DapSerializer();
            var outSerializer = new DapSerializer();
            var (requests, _) = receiver.GetRequests(JsonTestHelper.Parse(json));
            var result = requests.ToArray();
            request.Length.Should().Be(result.Length);

            for (var i = 0; i < request.Length; i++)
            {
                var r = request[i];
                var response = result[i];

                var actual = JsonTestHelper.Parse(inSerializer.SerializeObject(response));
                var expected = JsonTestHelper.Parse(outSerializer.SerializeObject(r));
                JsonElement.DeepEquals(actual, expected).Should().BeTrue();
            }
        }

        [Fact]
        public void Should_Camel_Case_As_Expected()
        {
            var serializer = new DapSerializer();
            var response = serializer.SerializeObject(
                new InitializeResponse
                {
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
                    new Renor[]
                    {
                        new Request(0, "attach", JsonTestHelper.ToElement(new { __restart = 3 }))
                    }
                );

                Add(
                    @"{""seq"": ""1"", ""type"": ""request"", ""command"": ""attach""}",
                    new Renor[]
                    {
                        new Request(1, "attach", JsonTestHelper.Parse("{}"))
                    }
                );

                Add(
                    @"{""seq"": ""0"", ""type"": ""event"", ""event"": ""breakpoint"", ""body"": { ""reason"": ""new"" }}",
                    new Renor[]
                    {
                        new Notification("breakpoint", JsonTestHelper.ToElement(new { reason = "new" })),
                    }
                );

                Add(
                    @"{""seq"": ""1"", ""type"": ""event"", ""event"": ""breakpoint""}",
                    new Renor[]
                    {
                        new Notification("breakpoint", null)
                    }
                );

                Add(
                    @"{""seq"": ""1"", ""type"": ""response"", ""request_seq"": 3, ""success"": true, ""command"": ""attach"", ""body"": {  }}",
                    new Renor[]
                    {
                        new ServerResponse(3, JsonTestHelper.Parse("{}")),
                    }
                );

                Add(
                    @"{""seq"": ""1"", ""type"": ""response"", ""request_seq"": 3, ""success"": true, ""command"": ""attach"", ""body"": null}",
                    new Renor[]
                    {
                        new ServerResponse(3, JsonTestHelper.Parse("null")),
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
                    new Renor[]
                    {
                        new ServerError(3, new ServerErrorResult(-1, "Unknown Error", JsonTestHelper.Parse("{}"))),
                    }
                );

                Add(
                    @"[1]",
                    new Renor[]
                    {
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
            var result = receiver.IsValid(JsonTestHelper.Parse(json));
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
