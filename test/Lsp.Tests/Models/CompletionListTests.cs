using System;
using System.Collections.Generic;
using FluentAssertions;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Models;
using Xunit;

namespace Lsp.Tests.Models
{
    public class CompletionListTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new CompletionList(new List<CompletionItem>() { new CompletionItem()
            {
                Kind = CompletionItemKind.Class,
                Detail = "details"
            } });
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<CompletionList>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }

        [Theory, JsonFixture]
        public void ComplexTest(string expected)
        {
            var model = new CompletionList(new List<CompletionItem>() {
                new CompletionItem()
            {
                Kind = CompletionItemKind.Class,
                Detail = "details"
            } }, true);
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<CompletionList>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
