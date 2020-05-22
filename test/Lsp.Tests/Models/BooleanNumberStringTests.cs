using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using Xunit;

namespace Lsp.Tests.Models
{
    public class BooleanNumberStringTests
    {
        [Fact]
        public void Should_BeNull()
        {
            var model = new BooleanNumberString();
            var result = Fixture.SerializeObject(model);

            result.Should().Be("null");

            var deresult = new Serializer(ClientVersion.Lsp3).DeserializeObject<BooleanNumberString>("null");
            deresult.Should().BeEquivalentTo(model);
        }

        [Fact]
        public void Should_AcceptANumber()
        {
            var model = new BooleanNumberString(1);
            var result = Fixture.SerializeObject(model);

            result.Should().Be("1");

            var deresult = new Serializer(ClientVersion.Lsp3).DeserializeObject<BooleanNumberString>("1");
            deresult.Should().BeEquivalentTo(model);
        }

        [Fact]
        public void Should_AcceptABoolean()
        {
            var model = new BooleanNumberString(true);
            var result = Fixture.SerializeObject(model);

            result.Should().Be("true");

            var deresult = new Serializer(ClientVersion.Lsp3).DeserializeObject<BooleanNumberString>("true");
            deresult.Should().BeEquivalentTo(model);
        }

        [Fact]
        public void Should_AcceptAString()
        {
            var model = new BooleanNumberString("abc");
            var result = Fixture.SerializeObject(model);

            result.Should().Be("\"abc\"");

            var deresult = new Serializer(ClientVersion.Lsp3).DeserializeObject<BooleanNumberString>("\"abc\"");
            deresult.Should().BeEquivalentTo(model);
        }
    }
}
