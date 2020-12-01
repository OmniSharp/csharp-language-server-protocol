using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc.Testing;
using OmniSharp.Extensions.LanguageProtocol.Testing;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using OmniSharp.Extensions.LanguageServer.Server;
using Serilog.Events;
using Xunit;
using Xunit.Abstractions;

namespace Lsp.Tests.Integration
{
    public class WorkspaceFolderTests : LanguageProtocolTestBase
    {
        public WorkspaceFolderTests(ITestOutputHelper outputHelper) : base(
            new JsonRpcTestOptions().ConfigureForXUnit(outputHelper, LogEventLevel.Verbose)
                                    .WithTimeout(TimeSpan.FromMilliseconds(1200))
                                    .WithWaitTime(TimeSpan.FromMilliseconds(600))
        )
        {
        }

        [Fact]
        public async Task Should_Disable_If_Not_Supported()
        {
            var (_, server) = await Initialize(
                options => {
                    options.DisableAllCapabilities();
                    options.OnInitialize(async (languageClient, request, token) => { request.Capabilities!.Workspace!.WorkspaceFolders = false; });
                }, ConfigureServer
            );
            server.WorkspaceFolderManager.IsSupported.Should().Be(false);
            var folders = await server.WorkspaceFolderManager.Refresh().ToArray().ToTask();
            folders.Should().BeEmpty();
        }

        [Fact]
        public async Task Should_Enable_If_Supported()
        {
            var (_, server) = await Initialize(ConfigureClient, ConfigureServer);
            server.WorkspaceFolderManager.IsSupported.Should().Be(true);
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
        public async Task Should_Allow_Null_Response()
        {
            var (client, server) = await Initialize(
                options => {
                    ConfigureClient(options);
                    options.OnWorkspaceFolders(@params => Task.FromResult<Container<WorkspaceFolder>?>(null));
                }, ConfigureServer);

            Func<Task> a = () => server.WorkspaceFolderManager.Refresh().LastOrDefaultAsync().ToTask();
            a.Should().NotThrow();
        }

        [Fact]
        public async Task Should_Have_Workspace_Folder_At_Startup()
        {
            var (_, server) = await Initialize(options => { options.WithWorkspaceFolder("/abcd/", nameof(Should_Have_Workspace_Folder_At_Startup)); }, ConfigureServer);

            var folder = server.WorkspaceFolderManager.CurrentWorkspaceFolders.Should().HaveCount(1).And.Subject.First();
            folder.Name.Should().Be(nameof(Should_Have_Workspace_Folder_At_Startup));
        }

        [Fact]
        public async Task Should_Remove_Workspace_Folder_by_name()
        {
            var (client, server) = await Initialize(options => { options.WithWorkspaceFolder("/abcd/", nameof(Should_Remove_Workspace_Folder_by_name)); }, ConfigureServer);

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
            var (client, server) = await Initialize(options => { options.WithWorkspaceFolder("/abcd/", nameof(Should_Remove_Workspace_Folder_by_uri)); }, ConfigureServer);

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

        [Fact]
        public async Task Should_Handle_Null_Workspace_Folders()
        {
            var workspaceLanguageServer = Substitute.For<IWorkspaceLanguageServer>();
            var languageServer = Substitute.For<ILanguageServer>();
            languageServer.ClientSettings.Returns(
                new InitializeParams() {
                    Capabilities = new ClientCapabilities() {
                        Workspace = new WorkspaceClientCapabilities() {
                            WorkspaceFolders = true
                        }
                    },
                    WorkspaceFolders = null
                }
            );
            var workspaceFolders = new LanguageServerWorkspaceFolderManager(workspaceLanguageServer);
            var started = (IOnLanguageServerStarted) workspaceFolders;
            await started.OnStarted(languageServer, CancellationToken);
        }

        [Fact]
        public async Task Should_Handle_Null_Workspace_Folders_On_Refresh()
        {
            var workspaceLanguageServer = Substitute.For<IWorkspaceLanguageServer>();
            var languageServer = Substitute.For<ILanguageServer>();
            languageServer.ClientSettings.Returns(
                new InitializeParams() {
                    Capabilities = new ClientCapabilities() {
                        Workspace = new WorkspaceClientCapabilities() {
                            WorkspaceFolders = true
                        }
                    },
                    WorkspaceFolders = null
                }
            );
            languageServer.SendRequest(Arg.Any<WorkspaceFolderParams>(), Arg.Any<CancellationToken>()).Returns((Container<WorkspaceFolder>? ) null);
            var workspaceFolders = new LanguageServerWorkspaceFolderManager(workspaceLanguageServer);
            var started = (IOnLanguageServerStarted) workspaceFolders;
            await started.OnStarted(languageServer, CancellationToken);

            var result = await workspaceFolders.Refresh().ToArray();

            result.Should().BeEmpty();
        }

        private void ConfigureClient(LanguageClientOptions options) =>
            options.WithClientCapabilities(
                new ClientCapabilities {
                    Workspace = new WorkspaceClientCapabilities {
                        WorkspaceFolders = true
                    }
                }
            );

        private void ConfigureServer(LanguageServerOptions options)
        {
        }
    }
}
