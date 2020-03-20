using System;
using System.Collections.Generic;
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
    public class DebugAdapterSpecifictionRecieverTests
    {
        [Theory]
        [ClassData(typeof(SpecificationMessages))]
        public void ShouldRespond_AsExpected(string json, Renor[] request)
        {
            var reciever = new DapReceiver();
            var (requests, _) = reciever.GetRequests(JToken.Parse(json));
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
            public override IEnumerable<ValueTuple<string, Renor[]>> GetValues()
            {
                yield return (
                    @"{""seq"": ""0"", ""type"": ""request"", ""command"": ""attach"", ""arguments"": { ""__restart"": 3 }}",
                    new Renor[]
                    {
                        new Request(0, "attach", new JObject() { { "__restart", 3 } })
                    }
                );

                yield return (
                    @"{""seq"": ""1"", ""type"": ""request"", ""command"": ""attach""}",
                    new Renor[]
                    {
                        new Request(1, "attach", new JObject())
                    }
                );

                yield return (
                    @"{""seq"": ""0"", ""type"": ""event"", ""event"": ""breakpoint"", ""body"": { ""reason"": ""new"" }}",
                    new Renor[]
                    {
                        new Notification("breakpoint", new JObject() { { "reason", "new" } }),
                    }
                );

                yield return (
                    @"{""seq"": ""1"", ""type"": ""event"", ""event"": ""breakpoint""}",
                    new Renor[]
                    {
                        new Notification("breakpoint", null)
                    }
                );

                yield return (
                    @"{""seq"": ""1"", ""type"": ""response"", ""request_seq"": 3, ""success"": true, ""command"": ""attach"", ""body"": {  }}",
                    new Renor[]
                    {
                        new ServerResponse(3, new JObject()),
                    }
                );

                yield return (
                    @"{""seq"": ""1"", ""type"": ""response"", ""request_seq"": 3, ""success"": true, ""command"": ""attach"", ""body"": null}",
                    new Renor[]
                    {
                        new ServerResponse(3, null),
                    }
                );

                yield return (
                    @"{""seq"": ""1"", ""type"": ""response"", ""request_seq"": 3, ""success"": false, ""command"": ""attach"", ""body"": {  }}",
                    new Renor[]
                    {
                        new ServerError(3, new JObject()),
                    }
                );

                yield return (
                    @"{""seq"": ""1"", ""type"": ""response"", ""request_seq"": 3, ""success"": false, ""command"": ""attach"", ""body"": null}",
                    new Renor[]
                    {
                        new ServerError(3, null),
                    }
                );

                yield return (
                    @"[1]",
                    new Renor[]
                    {
                        new InvalidRequest("Not an object")
                    });
            }
        }

        [Theory]
        [ClassData(typeof(InvalidMessages))]
        public void Should_ValidateInvalidMessages(string json, bool expected)
        {
            var reciever = new DapReceiver();
            var result = reciever.IsValid(JToken.Parse(json));
            result.Should().Be(expected);
        }

        class InvalidMessages : TheoryData<string, bool>
        {
            public override IEnumerable<ValueTuple<string, bool>> GetValues()
            {
                yield return (@"[]", false);
                yield return (@"""""", false);
                yield return (@"1", false);
                yield return (@"true", false);
                yield return (@"{}", true);
            }
        }
    }
}
