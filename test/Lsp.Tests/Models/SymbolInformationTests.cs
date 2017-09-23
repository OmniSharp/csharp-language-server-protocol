using System;
using FluentAssertions;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServerProtocol.Models;
using Xunit;

namespace Lsp.Tests.Models
{
    public class SymbolInformationTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new SymbolInformation() {
                ContainerName = "abc",
                Kind = SymbolKind.Boolean,
                Location = new Location() {
                    Range = new Range(new Position(1, 2), new Position(3, 4)),
                    Uri = new Uri("file:///abc/123.cs")
                },
                Name = "name"
            };
            var result = Fixture.SerializeObject(model);
            
            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<SymbolInformation>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
