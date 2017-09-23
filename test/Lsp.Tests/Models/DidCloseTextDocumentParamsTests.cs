using System;
using FluentAssertions;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServerProtocol.Models;
using Xunit;

namespace Lsp.Tests.Models
{
    public class DidCloseTextDocumentParamsTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new DidCloseTextDocumentParams() {
                TextDocument = new TextDocumentIdentifier(new Uri("file:///abc/def.cs"))
            };
            var result = Fixture.SerializeObject(model);
            
            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<DidCloseTextDocumentParams>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
