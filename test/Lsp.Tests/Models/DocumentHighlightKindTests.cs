using System;
using FluentAssertions;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Xunit;

namespace Lsp.Tests.Models
{
    public class DocumentHighlightKindTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new DocumentHighlightKind();
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<DocumentHighlightKind>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
