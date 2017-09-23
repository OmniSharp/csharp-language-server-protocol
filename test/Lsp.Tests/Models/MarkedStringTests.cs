using System;
using FluentAssertions;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServerProtocol.Models;
using Xunit;

namespace Lsp.Tests.Models
{
    public class MarkedStringTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new MarkedString("csharp", "some documented text...");
            var result = Fixture.SerializeObject(model);
            
            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<MarkedString>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
