using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using TestingUtils;
using Xunit;
using Xunit.Abstractions;

namespace Lsp.Tests.Models
{
    public class CodeActionContextTests
    {
        private ILogger _logger;

        public CodeActionContextTests(ITestOutputHelper testOutputHelper)
        {
            var factory = new TestLoggerFactory(testOutputHelper);
            _logger = factory.CreateLogger<CodeActionContextTests>();
        }
        [Theory]
        [JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new CodeActionContext {
                Diagnostics = new[] {
                    new Diagnostic {
                        Code = new DiagnosticCode("abcd"),
                        Message = "message",
                        Range = new Range(new Position(1, 1), new Position(2, 2)),
                        Severity = DiagnosticSeverity.Error,
                        Source = "csharp"
                    }
                }
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new LspSerializer(ClientVersion.Lsp3).DeserializeObject<CodeActionContext>(expected);
            deresult.Should().BeEquivalentTo(model, x => x.UsingStructuralRecordEquality());
        }
    }
}
