using System;
using FluentAssertions;
using Lsp.Models;
using Newtonsoft.Json;
using Xunit;

namespace Lsp.Tests.Models
{
    public class DiagnosticSeverityTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new DiagnosticSeverity();
            var result = Fixture.SerializeObject(model);
            
            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<DiagnosticSeverity>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
