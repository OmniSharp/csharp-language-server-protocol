using System;
using FluentAssertions;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Models;
using Xunit;

namespace Lsp.Tests.Models
{
    public class FormattingOptionsTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new FormattingOptions() {
                {  "tabSize", 4 },
                { "insertSpaces", true },
                { "somethingElse", "cool" }
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<FormattingOptions>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
