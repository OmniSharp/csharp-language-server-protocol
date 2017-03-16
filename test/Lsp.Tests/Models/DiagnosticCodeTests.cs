using System;
using FluentAssertions;
using Lsp.Models;
using Newtonsoft.Json;
using Xunit;

namespace Lsp.Tests.Models
{
    public class DiagnosticCodeTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new DiagnosticCode();
            var result = Fixture.SerializeObject(model);
            
            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<DiagnosticCode>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
