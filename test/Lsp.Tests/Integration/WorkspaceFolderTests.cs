using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Reactive.Testing;
using Newtonsoft.Json;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc.Testing;
using OmniSharp.Extensions.LanguageProtocol.Testing;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using OmniSharp.Extensions.LanguageServer.Server;
using Serilog.Events;
using Xunit;
using Xunit.Abstractions;

namespace Lsp.Tests.Integration
{
    public class WorkspaceFolderTests : LanguageProtocolTestBase
    {
        private readonly ITestOutputHelper _outputHelper;

        public WorkspaceFolderTests(ITestOutputHelper outputHelper) : base(new JsonRpcTestOptions().ConfigureForXUnit(outputHelper, LogEventLevel.Verbose))
        {
            _outputHelper = outputHelper;
        }

        [Fact]
        public async Task Should_Add_A_Workspace_Folder()
        {
            var (client, server) = await Initialize(ConfigureClient, ConfigureServer);

            var folders = new List<WorkspaceFolderChange>();
            server.WorkspaceFolderManager.Changed.Subscribe(x => folders.Add(x));

            client.WorkspaceFoldersManager.Add("/abcd/", nameof(Should_Add_A_Workspace_Folder));

            await SettleNext();

            folders.Should().HaveCount(1);
            folders[0].Event.Should().Be(WorkspaceFolderEvent.Add);
            folders[0].Folder.Name.Should().Be(nameof(Should_Add_A_Workspace_Folder));
        }

        [Fact]
        public async Task Should_Have_Workspace_Folder_At_Startup()
        {
            var (client, server) = await Initialize(options => { options.WithWorkspaceFolder("/abcd/", nameof(Should_Have_Workspace_Folder_At_Startup)); }, ConfigureServer);

            var folder = server.WorkspaceFolderManager.CurrentWorkspaceFolders.Should().HaveCount(1).And.Subject.First();
            folder.Name.Should().Be(nameof(Should_Have_Workspace_Folder_At_Startup));
        }

        [Fact]
        public async Task Should_Remove_Workspace_Folder_by_name()
        {
            var (client, server) = await Initialize(options => {
                options.WithWorkspaceFolder("/abcd/", nameof(Should_Remove_Workspace_Folder_by_name));
            }, ConfigureServer);

            var folders = new List<WorkspaceFolderChange>();
            server.WorkspaceFolderManager.Changed.Subscribe(x => folders.Add(x));

            var folder = server.WorkspaceFolderManager.CurrentWorkspaceFolders.Should().HaveCount(1).And.Subject.First();
            folder.Name.Should().Be(nameof(Should_Remove_Workspace_Folder_by_name));

            client.WorkspaceFoldersManager.Remove(nameof(Should_Remove_Workspace_Folder_by_name));

            await SettleNext();

            folders.Should().HaveCount(1);
            folders[0].Event.Should().Be(WorkspaceFolderEvent.Remove);
            folders[0].Folder.Name.Should().Be(nameof(Should_Remove_Workspace_Folder_by_name));
        }

        [Fact]
        public async Task Should_Remove_Workspace_Folder_by_uri()
        {
            var (client, server) = await Initialize(options => {
                options.WithWorkspaceFolder("/abcd/", nameof(Should_Remove_Workspace_Folder_by_uri));
            }, ConfigureServer);

            var folders = new List<WorkspaceFolderChange>();
            server.WorkspaceFolderManager.Changed.Subscribe(x => folders.Add(x));

            var folder = server.WorkspaceFolderManager.CurrentWorkspaceFolders.Should().HaveCount(1).And.Subject.First();
            folder.Name.Should().Be(nameof(Should_Remove_Workspace_Folder_by_uri));

            client.WorkspaceFoldersManager.Remove(DocumentUri.From("/abcd/"));

            await SettleNext();

            folders.Should().HaveCount(1);
            folders[0].Event.Should().Be(WorkspaceFolderEvent.Remove);
            folders[0].Folder.Name.Should().Be(nameof(Should_Remove_Workspace_Folder_by_uri));
        }

        private void ConfigureClient(LanguageClientOptions options)
        {
        }

        private void ConfigureServer(LanguageServerOptions options)
        {
        }
    }
}
