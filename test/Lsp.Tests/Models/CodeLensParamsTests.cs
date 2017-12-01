using System;
using FluentAssertions;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Xunit;

namespace Lsp.Tests.Models
{
    public class CodeLensParamsTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new CodeLensParams() {
                TextDocument = new TextDocumentIdentifier(new Uri("file:///abc/123/d.cs")),
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<CodeLensParams>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
