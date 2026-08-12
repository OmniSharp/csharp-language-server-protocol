using System.Text.Json;
using FluentAssertions;
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

            using var doc = JsonDocument.Parse(result);
            var workspace = doc.RootElement.GetProperty("workspace");
            workspace.GetProperty("inlayHint").GetProperty("refreshSupport").GetBoolean().Should().BeTrue();
            workspace.GetProperty("diagnostics").GetProperty("refreshSupport").GetBoolean().Should().BeTrue();
            workspace.TryGetProperty("semanticTokens", out _).Should().BeFalse();
            workspace.TryGetProperty("codeLens", out _).Should().BeFalse();
        }
    }
}
