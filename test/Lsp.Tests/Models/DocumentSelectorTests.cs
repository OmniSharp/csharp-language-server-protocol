using System;
using FluentAssertions;
using Lsp.Models;
using Newtonsoft.Json;
using Xunit;

namespace Lsp.Tests.Models
{
    public class DocumentSelectorTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new DocumentSelector(new DocumentFilter[]
            {
                new DocumentFilter()
                {
                    Language = "csharp",
                },
                new DocumentFilter()
                {
                    Pattern = "**/*.vb"
                },
                new DocumentFilter()
                {
                    Scheme = "visualbasic"
                },
            });
            var result = Fixture.SerializeObject(model);
            
            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<DocumentSelector>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
