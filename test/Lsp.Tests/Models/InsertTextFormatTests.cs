using System;
using FluentAssertions;
using Lsp.Models;
using Newtonsoft.Json;
using Xunit;

namespace Lsp.Tests.Models
{
    public class InsertTextFormatTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new InsertTextFormat();
            var result = Fixture.SerializeObject(model);
            
            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<InsertTextFormat>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
