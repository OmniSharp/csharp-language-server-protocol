using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using Xunit;

namespace Lsp.Tests.Models
{
    public class FoldingRangeKindTests
    {
        [Fact]
        public void CommentTest()
        {
            var serializer = new Serializer();
            var json = serializer.SerializeObject(FoldingRangeKind.Comment);
            json.Should().Be("\"comment\"");
        }

        [Fact]
        public void ImportsTest()
        {
            var serializer = new Serializer();
            var json = serializer.SerializeObject(FoldingRangeKind.Imports);
            json.Should().Be("\"imports\"");
        }

        [Fact]
        public void RegionTest()
        {
            var serializer = new Serializer();
            var json = serializer.SerializeObject(FoldingRangeKind.Region);
            json.Should().Be("\"region\"");
        }
    }
}
