using System;
using FluentAssertions;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServerProtocol.Messages;
using Xunit;

namespace Lsp.Tests.Messages
{
    public class ServerNotInitializedTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new ServerNotInitialized();
            var result = Fixture.SerializeObject(model);
            
            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<ServerNotInitialized>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
