using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using Xunit;

namespace Lsp.Tests.Models
{
    public class CodeLensRegistrationOptionsTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new CodeLensRegistrationOptions() {
                DocumentSelector = new DocumentSelector(new[] { new DocumentFilter(){
                    Language = "csharp",
                    Pattern = "pattern",
                    Scheme = "scheme"
                }, new DocumentFilter(){
                    Language = "vb",
                    Pattern = "pattern",
                    Scheme = "scheme"
                } }),
                ResolveProvider = true
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new Serializer(ClientVersion.Lsp3).DeserializeObject<CodeLensRegistrationOptions>(expected);
            deresult.Should().BeEquivalentTo(model);
        }
    }
}
