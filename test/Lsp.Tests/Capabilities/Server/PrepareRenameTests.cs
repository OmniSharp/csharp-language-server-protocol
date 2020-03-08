using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Xunit;

namespace Lsp.Tests.Capabilities.Server
{
    public class PrepareRenameTests
    {
        [Theory, JsonFixture]
        public void Range(string expected)
        {
            var range = new Range(
                new Position(1, 2),
                new Position(3, 4)
            );

            var model = new RangeOrPlaceholderRange(range);

            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);
        }

        [Theory, JsonFixture]
        public void PlaceholderRange(string expected)
        {
            var placeholderRange = new PlaceholderRange
            {
                Range = new Range(
                    new Position(1, 2),
                    new Position(3, 4)
                ),
                Placeholder = "placeholder"
            };

            var model = new RangeOrPlaceholderRange(placeholderRange);

            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);
        }
    }
}
