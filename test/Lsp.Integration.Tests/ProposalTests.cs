using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using FluentAssertions;
using Lsp.Tests.Integration.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc.Testing;
using OmniSharp.Extensions.LanguageProtocol.Testing;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using TestingUtils;
using Xunit;
using Xunit.Abstractions;

namespace Lsp.Tests.Integration
{
    public class ProposalTests : LanguageProtocolTestBase
    {
        public ProposalTests(ITestOutputHelper testOutputHelper) : base(new JsonRpcTestOptions().ConfigureForXUnit(testOutputHelper))
        {
        }

        [Fact]
        public async Task Server_Should_Deserialize_Capabilities_As_Proposal_Types()
        {
            var (_, server) = await Initialize(
                options => {
                    options.EnableProposals().EnableAllCapabilities();
                }, options => {
                    options.EnableProposals();
                }
            );

            server.ServerSettings.Capabilities.Should().BeOfType<ProposedServerCapabilities>();
            server.ServerSettings.Capabilities.Workspace.Should().BeOfType<ProposedWorkspaceServerCapabilities>();
            server.ClientSettings.Capabilities.Should().BeOfType<ProposedClientCapabilities>();
            server.ClientSettings.Capabilities!.General.Should().BeOfType<ProposedGeneralClientCapabilities>();
            server.ClientSettings.Capabilities!.TextDocument.Should().BeOfType<ProposedTextDocumentClientCapabilities>();
            server.ClientSettings.Capabilities!.Window.Should().BeOfType<ProposedWindowClientCapabilities>();
            server.ClientSettings.Capabilities!.Workspace.Should().BeOfType<ProposedWorkspaceClientCapabilities>();
        }

        [Fact]
        public async Task Client_Should_Deserialize_Capabilities_As_Proposal_Types()
        {
            var (client, _) = await Initialize(
                options => {
                    options.EnableProposals().EnableAllCapabilities();
                }, options => {
                    options.EnableProposals();
                }
            );

            client.ServerSettings.Capabilities.Should().BeOfType<ProposedServerCapabilities>();
            client.ServerSettings.Capabilities.Workspace.Should().BeOfType<ProposedWorkspaceServerCapabilities>();
            client.ClientSettings.Capabilities.Should().BeOfType<ProposedClientCapabilities>();
            client.ClientSettings.Capabilities!.General.Should().BeOfType<ProposedGeneralClientCapabilities>();
            client.ClientSettings.Capabilities!.TextDocument.Should().BeOfType<ProposedTextDocumentClientCapabilities>();
            client.ClientSettings.Capabilities!.Window.Should().BeOfType<ProposedWindowClientCapabilities>();
            client.ClientSettings.Capabilities!.Workspace.Should().BeOfType<ProposedWorkspaceClientCapabilities>();
        }
    }
}
