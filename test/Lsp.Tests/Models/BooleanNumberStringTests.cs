using System;
using FluentAssertions;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
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

            var deresult = JsonConvert.DeserializeObject<BooleanNumberString>("null", Serializer.CreateSerializerSettings(ClientVersion.Lsp3));
            deresult.ShouldBeEquivalentTo(model);
        }

        [Fact]
        public void Should_AcceptANumber()
        {
            var model = new BooleanNumberString(1);
            var result = Fixture.SerializeObject(model);

            result.Should().Be("1");

            var deresult = JsonConvert.DeserializeObject<BooleanNumberString>("1", Serializer.CreateSerializerSettings(ClientVersion.Lsp3));
            deresult.ShouldBeEquivalentTo(model);
        }

        [Fact]
        public void Should_AcceptABoolean()
        {
            var model = new BooleanNumberString(true);
            var result = Fixture.SerializeObject(model);

            result.Should().Be("true");

            var deresult = JsonConvert.DeserializeObject<BooleanNumberString>("true", Serializer.CreateSerializerSettings(ClientVersion.Lsp3));
            deresult.ShouldBeEquivalentTo(model);
        }

        [Fact]
        public void Should_AcceptAString()
        {
            var model = new BooleanNumberString("abc");
            var result = Fixture.SerializeObject(model);

            result.Should().Be("\"abc\"");

            var deresult = JsonConvert.DeserializeObject<BooleanNumberString>("\"abc\"", Serializer.CreateSerializerSettings(ClientVersion.Lsp3));
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
