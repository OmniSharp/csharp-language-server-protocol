using System;
using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
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

            var deresult = new Serializer(ClientVersion.Lsp3).DeserializeObject<FileEvent>(expected);
            deresult.Should().BeEquivalentTo(model);
        }
    }
}
