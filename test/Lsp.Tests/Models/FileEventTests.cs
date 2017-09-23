using System;
using FluentAssertions;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Models;
using Xunit;

namespace Lsp.Tests.Models
{
    public class FileEventTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new FileEvent() {
                Type = FileChangeType.Deleted,
                Uri = new Uri("file:///abc/123.cs")
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<FileEvent>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
