using System;
using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using Xunit;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Lsp.Tests.Models
{
    public class LocationOrLocationLinksTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new LocationOrLocationLinks();
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new Serializer(ClientVersion.Lsp3).DeserializeObject<LocationOrLocationLinks>(expected);
            deresult.Should().BeEquivalentTo(model);
        }

        [Theory, JsonFixture]
        public void LocationTest(string expected)
        {
            var model = new LocationOrLocationLinks(new Location()
            {
                Range = new Range(new Position(1, 1), new Position(3, 3)),

            });
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new Serializer(ClientVersion.Lsp3).DeserializeObject<LocationOrLocationLinks>(expected);
            deresult.Should().BeEquivalentTo(model);
        }

        [Theory, JsonFixture]
        public void LocationsTest(string expected)
        {
            var model = new LocationOrLocationLinks(new Location()
            {
                Range = new Range(new Position(1, 1), new Position(3, 3)),

            }, new Location()
            {
                Range = new Range(new Position(1, 1), new Position(3, 3)),

            });
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new Serializer(ClientVersion.Lsp3).DeserializeObject<LocationOrLocationLinks>(expected);
            deresult.Should().BeEquivalentTo(model);
        }

        [Theory, JsonFixture]
        public void LocationLinkTest(string expected)
        {
            var model = new LocationOrLocationLinks(new LocationLink()
            {
                TargetSelectionRange = new Range(new Position(1, 1), new Position(3, 3)),
                TargetRange = new Range(new Position(1, 1), new Position(3, 3)),
                TargetUri = new Uri("file:///asdfasdf/a.tst"),
                OriginSelectionRange = new Range(new Position(1, 1), new Position(3, 3)),
            });
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new Serializer(ClientVersion.Lsp3).DeserializeObject<LocationOrLocationLinks>(expected);
            deresult.Should().BeEquivalentTo(model, x => x
                .ComparingByMembers<LocationOrLocationLink>()
            );
        }

        [Theory, JsonFixture]
        public void LocationLinksTest(string expected)
        {
            var model = new LocationOrLocationLinks(new LocationLink()
            {
                TargetSelectionRange = new Range(new Position(1, 1), new Position(3, 3)),
                TargetRange = new Range(new Position(1, 1), new Position(3, 3)),
                TargetUri = new Uri("file:///asdfasdf/a.tst"),
                OriginSelectionRange = new Range(new Position(1, 1), new Position(3, 3)),

            },
            new LocationLink()
            {
                TargetSelectionRange = new Range(new Position(1, 1), new Position(3, 3)),
                TargetRange = new Range(new Position(1, 1), new Position(3, 3)),
                TargetUri = new Uri("file:///asdfasdf/a.tst"),
                OriginSelectionRange = new Range(new Position(1, 1), new Position(3, 3)),

            });
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new Serializer(ClientVersion.Lsp3).DeserializeObject<LocationOrLocationLinks>(expected);
            deresult.Should().BeEquivalentTo(model, x => x
                .ComparingByMembers<LocationOrLocationLink>()
                .ComparingByMembers<Location>()
                .ComparingByMembers<LocationLink>()
            );
        }
    }
}
