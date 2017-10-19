using System;
using FluentAssertions;
using Newtonsoft.Json;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Messages;
using Xunit;

namespace Lsp.Tests.Messages
{
    public class ServerErrorEndTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new ServerErrorEnd();
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<RpcError>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
