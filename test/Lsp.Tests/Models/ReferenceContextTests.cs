using System;
using FluentAssertions;
using Lsp.Models;
using Newtonsoft.Json;
using Xunit;

namespace Lsp.Tests.Models
{
    public class ReferenceContextTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new ReferenceContext() {
                IncludeDeclaration = true
            };
            var result = Fixture.SerializeObject(model);
            
            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<ReferenceContext>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
