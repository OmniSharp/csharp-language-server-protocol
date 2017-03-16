using System;
using FluentAssertions;
using Lsp.Capabilities.Server;
using Newtonsoft.Json;
using Xunit;

namespace Lsp.Tests.Capabilities.Server
{
    public class CodeLensOptionsTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new CodeLensOptions() {
                ResolveProvider = true,
            };
            var result = Fixture.SerializeObject(model);
            
            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<CodeLensOptions>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
