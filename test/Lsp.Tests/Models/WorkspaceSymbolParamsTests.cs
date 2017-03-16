using System;
using FluentAssertions;
using Lsp.Models;
using Newtonsoft.Json;
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
