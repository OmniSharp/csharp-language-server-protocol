using System;
using FluentAssertions;
using Lsp.Models;
using Newtonsoft.Json;
using Xunit;

namespace Lsp.Tests.Models
{
    public class FileChangeTypeTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new FileChangeType();
            var result = Fixture.SerializeObject(model);
            
            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<FileChangeType>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
