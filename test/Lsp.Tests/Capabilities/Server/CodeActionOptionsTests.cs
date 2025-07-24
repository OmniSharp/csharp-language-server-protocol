using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using TestingUtils;

namespace Lsp.Tests.Capabilities.Server
{
    public class CodeActionOptionsTests
    {
        [Theory]
        [JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new CodeActionRegistrationOptions.StaticOptions {
                ResolveProvider = true,
                CodeActionKinds = new[] { CodeActionKind.QuickFix, CodeActionKind.Refactor }
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new LspSerializer(ClientVersion.Lsp3).DeserializeObject<CodeActionRegistrationOptions.StaticOptions>(expected);
            deresult.Should().BeEquivalentTo(model, x => x.UsingStructuralRecordEquality());
        }
    }
}
