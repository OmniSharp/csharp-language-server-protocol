using FluentAssertions;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using Xunit;

namespace Lsp.Tests.Capabilities.Client
{
    public class WorkspaceRefreshCapabilityTests
    {
        [Fact]
        public void Should_Serialize_InlayHint_And_Diagnostic_Workspace_Capabilities_To_Their_Own_Keys()
        {
            var model = new ClientCapabilities {
                Workspace = new WorkspaceClientCapabilities {
                    InlayHint = new InlayHintWorkspaceClientCapabilities { RefreshSupport = true },
                    Diagnostics = new DiagnosticWorkspaceClientCapabilities { RefreshSupport = true }
                }
            };

            var result = new LspSerializer(ClientVersion.Lsp3).SerializeObject(model);

            var workspace = JObject.Parse(result)["workspace"]!;
            workspace["inlayHint"]!["refreshSupport"]!.Value<bool>().Should().BeTrue();
            workspace["diagnostics"]!["refreshSupport"]!.Value<bool>().Should().BeTrue();
            workspace["semanticTokens"].Should().BeNull();
            workspace["codeLens"].Should().BeNull();
        }
    }
}
