using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc.Testing;
using OmniSharp.Extensions.LanguageProtocol.Testing;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using OmniSharp.Extensions.LanguageServer.Server;
using Serilog.Events;
using TestingUtils;
using Xunit.Abstractions;

namespace Lsp.Tests.Integration
{
    public class FileOperationTests : LanguageProtocolTestBase
    {
        private readonly Action<DidCreateFileParams> _didCreateFileHandler = Substitute.For<Action<DidCreateFileParams>>();
        private readonly Func<WillCreateFileParams, Task<WorkspaceEdit?>> _willCreateFileHandler = Substitute.For<Func<WillCreateFileParams, Task<WorkspaceEdit?>>>();
        private readonly Action<DidRenameFileParams> _didRenameFileHandler = Substitute.For<Action<DidRenameFileParams>>();
        private readonly Func<WillRenameFileParams, Task<WorkspaceEdit?>> _willRenameFileHandler = Substitute.For<Func<WillRenameFileParams, Task<WorkspaceEdit?>>>();
        private readonly Action<DidDeleteFileParams> _didDeleteFileHandler = Substitute.For<Action<DidDeleteFileParams>>();
        private readonly Func<WillDeleteFileParams, Task<WorkspaceEdit?>> _willDeleteFileHandler = Substitute.For<Func<WillDeleteFileParams, Task<WorkspaceEdit?>>>();

        public FileOperationTests(ITestOutputHelper outputHelper) : base(new JsonRpcTestOptions().ConfigureForXUnit(outputHelper, LogEventLevel.Verbose))
        {
        }

        [RetryFact]
        public async Task Should_Handle_FileCreate()
        {
            var (client, server) = await Initialize(Configure, Configure);

            await client.RequestWillCreateFile(
                new WillCreateFileParams() {
                    Files = Container<FileCreate>.From(new FileCreate() { Uri = new Uri("file://asdf") })
                }
            );
            client.DidCreateFile(
                new DidCreateFileParams() {
                    Files = Container<FileCreate>.From(new FileCreate() { Uri = new Uri("file://asdf") })
                }
            );

            await SettleNext();

            _didCreateFileHandler.ReceivedCalls().Should().HaveCount(1);
            _willCreateFileHandler.ReceivedCalls().Should().HaveCount(1);

            VerifyServerSettings(server.ServerSettings);
            VerifyServerSettings(client.ServerSettings);

            VerifyClientSettings(server.ClientSettings);
            VerifyClientSettings(client.ClientSettings);

            static void VerifyServerSettings(InitializeResult result)
            {
                result.Capabilities.Workspace.FileOperations.DidCreate.Should().NotBeNull();
                result.Capabilities.Workspace.FileOperations.WillCreate.Should().NotBeNull();
                var s = result.Capabilities.Workspace.FileOperations.DidCreate.Filters.Should().HaveCount(1).And.Subject.FirstOrDefault();
                s.Scheme.Should().Be("file");
                s.Pattern.Glob.Should().Be("**/*.cs");
                s.Pattern.Matches.Should().Be(FileOperationPatternKind.File);
                s.Pattern.Options.IgnoreCase.Should().BeTrue();
            }

            static void VerifyClientSettings(InitializeParams result)
            {
                result.Capabilities.Workspace.FileOperations.Value.Should().BeOfType<FileOperationsWorkspaceClientCapabilities>();
                result.Capabilities.Workspace.FileOperations.Value.DidCreate.Should().BeTrue();
                result.Capabilities.Workspace.FileOperations.Value.WillCreate.Should().BeTrue();
            }
        }

        [RetryFact]
        public async Task Should_Handle_FileRename()
        {
            var (client, server) = await Initialize(Configure, Configure);

            await client.RequestWillRenameFile(
                new WillRenameFileParams() {
                    Files = Container<FileRename>.From(new FileRename() { Uri = new Uri("file://asdf") })
                }
            );
            client.DidRenameFile(
                new DidRenameFileParams() {
                    Files = Container<FileRename>.From(new FileRename() { Uri = new Uri("file://asdf") })
                }
            );

            await SettleNext();

            _didRenameFileHandler.ReceivedCalls().Should().HaveCount(1);
            _willRenameFileHandler.ReceivedCalls().Should().HaveCount(1);

            VerifyServerSettings(server.ServerSettings);
            VerifyServerSettings(client.ServerSettings);

            VerifyClientSettings(server.ClientSettings);
            VerifyClientSettings(client.ClientSettings);

            static void VerifyServerSettings(InitializeResult result)
            {
                result.Capabilities.Workspace.FileOperations.DidRename.Should().NotBeNull();
                result.Capabilities.Workspace.FileOperations.WillRename.Should().NotBeNull();
                var s = result.Capabilities.Workspace.FileOperations.DidRename.Filters.Should().HaveCount(1).And.Subject.FirstOrDefault();
                s.Scheme.Should().Be("file");
                s.Pattern.Glob.Should().Be("**/*.cs");
                s.Pattern.Matches.Should().Be(FileOperationPatternKind.File);
                s.Pattern.Options.IgnoreCase.Should().BeTrue();
            }

            static void VerifyClientSettings(InitializeParams result)
            {
                result.Capabilities.Workspace.FileOperations.Value.Should().BeOfType<FileOperationsWorkspaceClientCapabilities>();
                result.Capabilities.Workspace.FileOperations.Value.DidRename.Should().BeTrue();
                result.Capabilities.Workspace.FileOperations.Value.WillRename.Should().BeTrue();
            }
        }

        [RetryFact]
        public async Task Should_Handle_FileDelete()
        {
            var (client, server) = await Initialize(Configure, Configure);

            await client.RequestWillDeleteFile(
                new WillDeleteFileParams() {
                    Files = Container<FileDelete>.From(new FileDelete() { Uri = new Uri("file://asdf") })
                }
            );
            client.DidDeleteFile(
                new DidDeleteFileParams() {
                    Files = Container<FileDelete>.From(new FileDelete() { Uri = new Uri("file://asdf") })
                }
            );

            await SettleNext();

            _didDeleteFileHandler.ReceivedCalls().Should().HaveCount(1);
            _willDeleteFileHandler.ReceivedCalls().Should().HaveCount(1);

            VerifyServerSettings(server.ServerSettings);
            VerifyServerSettings(client.ServerSettings);

            VerifyClientSettings(server.ClientSettings);
            VerifyClientSettings(client.ClientSettings);

            static void VerifyServerSettings(InitializeResult result)
            {
                result.Capabilities.Workspace.FileOperations.DidDelete.Should().NotBeNull();
                result.Capabilities.Workspace.FileOperations.WillDelete.Should().NotBeNull();
                var s = result.Capabilities.Workspace.FileOperations.DidDelete.Filters.Should().HaveCount(1).And.Subject.FirstOrDefault();
                s.Scheme.Should().Be("file");
                s.Pattern.Glob.Should().Be("**/*.cs");
                s.Pattern.Matches.Should().Be(FileOperationPatternKind.File);
                s.Pattern.Options.IgnoreCase.Should().BeTrue();
            }

            static void VerifyClientSettings(InitializeParams result)
            {
                result.Capabilities.Workspace.FileOperations.Value.Should().BeOfType<FileOperationsWorkspaceClientCapabilities>();
                result.Capabilities.Workspace.FileOperations.Value.DidDelete.Should().BeTrue();
                result.Capabilities.Workspace.FileOperations.Value.WillDelete.Should().BeTrue();
            }
        }

        private void Configure(LanguageClientOptions options)
        {
            options.WithCapability(
                new FileOperationsWorkspaceClientCapabilities() {
                    DidCreate = true,
                    DidRename = true,
                    DidDelete = true,
                    WillCreate = true,
                    WillRename = true,
                    WillDelete = true
                }
            );
        }

        private void Configure(LanguageServerOptions options)
        {
            var filters = Container<FileOperationFilter>.From(
                new FileOperationFilter() {
                    Scheme = "file", Pattern = new FileOperationPattern() {
                        Glob = "**/*.cs",
                        Matches = FileOperationPatternKind.File,
                        Options = new FileOperationPatternOptions() {
                            IgnoreCase = true
                        }
                    }
                }
            );
            options.OnDidCreateFile(_didCreateFileHandler, (capability, capabilities) => new() { Filters = filters });
            options.OnWillCreateFile(_willCreateFileHandler, (capability, capabilities) => new() { Filters = filters });
            options.OnDidRenameFile(_didRenameFileHandler, (capability, capabilities) => new() { Filters = filters });
            options.OnWillRenameFile(_willRenameFileHandler, (capability, capabilities) => new() { Filters = filters });
            options.OnDidDeleteFile(_didDeleteFileHandler, (capability, capabilities) => new() { Filters = filters });
            options.OnWillDeleteFile(_willDeleteFileHandler, (capability, capabilities) => new() { Filters = filters });
        }
    }
}
