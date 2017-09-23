using System;
using FluentAssertions;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Models;
using Xunit;

namespace Lsp.Tests.Models
{
    public class WorkspaceSymbolParamsTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new WorkspaceSymbolParams() {
                Query = "query"
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<WorkspaceSymbolParams>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
