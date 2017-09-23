using System;
using FluentAssertions;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServerProtocol.Models;
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

            var deresult = JsonConvert.DeserializeObject<CodeLensRegistrationOptions>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
