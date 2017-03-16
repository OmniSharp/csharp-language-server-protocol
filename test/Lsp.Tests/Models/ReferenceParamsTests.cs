using System;
using FluentAssertions;
using Lsp.Models;
using Newtonsoft.Json;
using Xunit;

namespace Lsp.Tests.Models
{
    public class ReferenceParamsTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new ReferenceParams() {
                Context = new ReferenceContext() {
                    IncludeDeclaration = true,
                },
                Position = new Position(1,2),
                TextDocument = new TextDocumentIdentifier(new Uri("file:///abc/123.cs"))
            };
            var result = Fixture.SerializeObject(model);
            
            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<ReferenceParams>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
