using System;
using FluentAssertions;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Capabilities.Server;
using Xunit;

namespace Lsp.Tests.Capabilities.Server
{
    public class SaveOptionsTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new SaveOptions() {
                IncludeText = false
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<SaveOptions>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
