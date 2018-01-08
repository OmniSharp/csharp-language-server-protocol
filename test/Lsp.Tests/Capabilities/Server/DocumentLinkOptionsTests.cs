using System;
using FluentAssertions;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using Xunit;

namespace Lsp.Tests.Capabilities.Server
{
    public class DocumentLinkOptionsTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new DocumentLinkOptions() {
                ResolveProvider = true,
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new Serializer(ClientVersion.Lsp3).DeserializeObject<DocumentLinkOptions>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
