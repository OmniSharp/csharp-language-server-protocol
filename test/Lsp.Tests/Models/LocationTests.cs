using System;
using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using Xunit;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Lsp.Tests.Models
{
    public class LocationTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new Location() {
                Range = new Range(new Position(1,2), new Position(3,4)),
                Uri = new Uri("file:///abc/123.cs")
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new Serializer(ClientVersion.Lsp3).DeserializeObject<Location>(expected);
            deresult.Should().BeEquivalentTo(model);
        }
    }
}
