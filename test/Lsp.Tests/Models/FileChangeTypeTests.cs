using System;
using FluentAssertions;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
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
