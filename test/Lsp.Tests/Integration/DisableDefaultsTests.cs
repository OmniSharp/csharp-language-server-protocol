using System.Collections.Generic;
using System.Composition;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Testing;
using OmniSharp.Extensions.LanguageProtocol.Testing;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using OmniSharp.Extensions.LanguageServer.Server;
using OmniSharp.Extensions.LanguageServer.Shared;
using Xunit;
using Xunit.Abstractions;

namespace Lsp.Tests.Integration
{
    public class DisableDefaultsTests : LanguageProtocolTestBase
    {
        public DisableDefaultsTests(ITestOutputHelper outputHelper) : base(new JsonRpcTestOptions().ConfigureForXUnit(outputHelper))
        {
        }

        [Theory]
        [InlineData(true), InlineData(false)]
        public async Task Should_Disable_Registration_Manager(bool enabled)
        {
            var (client, _) = await Initialize(
                options => options.UseDefaultRegistrationManager(enabled),
                options => { }
            );

            var clientManager = client.Services.GetRequiredService<SharedHandlerCollection>();
            clientManager.ContainsHandler(typeof(IRegisterCapabilityHandler)).Should().Be(enabled);
            clientManager.ContainsHandler(typeof(IUnregisterCapabilityHandler)).Should().Be(enabled);
        }

        [Theory]
        [InlineData(true), InlineData(false)]
        public async Task Should_Disable_Workspace_Folder_Manager(bool enabled)
        {
            var (client, server) = await Initialize(
                options => options.UseDefaultWorkspaceFolderManager(enabled),
                options => options.UseDefaultWorkspaceFolderManager(enabled)
            );

            var clientManager = client.Services.GetRequiredService<SharedHandlerCollection>();
            clientManager.ContainsHandler(typeof(IWorkspaceFoldersHandler)).Should().Be(enabled);

            var serverManager = server.Services.GetRequiredService<SharedHandlerCollection>();
            serverManager.ContainsHandler(typeof(IDidChangeWorkspaceFoldersHandler)).Should().Be(enabled);
        }

        [Theory]
        [InlineData(true), InlineData(false)]
        public async Task Should_Disable_Configuration(bool enabled)
        {
            var (_, server) = await Initialize(
                options => { },
                options => options.UseDefaultServerConfiguration(enabled)
            );

            var serverManager = server.Services.GetRequiredService<SharedHandlerCollection>();
            serverManager.ContainsHandler(typeof(IDidChangeConfigurationHandler)).Should().Be(enabled);
        }
    }
}
