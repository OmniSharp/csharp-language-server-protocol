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
using Xunit;
using Xunit.Abstractions;

namespace Lsp.Integration.Tests
{
    public class FileOperationsTests : LanguageProtocolTestBase
    {
        private readonly Action<DidCreateFilesParams> _didCreateFileHandler = Substitute.For<Action<DidCreateFilesParams>>();

        private readonly Func<WillCreateFilesParams, Task<WorkspaceEdit?>> _willCreateFileHandler =
            Substitute.For<Func<WillCreateFilesParams, Task<WorkspaceEdit?>>>();

        private readonly Action<DidRenameFilesParams> _didRenameFileHandler = Substitute.For<Action<DidRenameFilesParams>>();

        private readonly Func<WillRenameFilesParams, Task<WorkspaceEdit?>> _willRenameFileHandler =
            Substitute.For<Func<WillRenameFilesParams, Task<WorkspaceEdit?>>>();

        private readonly Action<DidDeleteFilesParams> _didDeleteFileHandler = Substitute.For<Action<DidDeleteFilesParams>>();

        private readonly Func<WillDeleteFilesParams, Task<WorkspaceEdit?>> _willDeleteFileHandler =
            Substitute.For<Func<WillDeleteFilesParams, Task<WorkspaceEdit?>>>();

        public FileOperationsTests(ITestOutputHelper outputHelper) : base(new JsonRpcTestOptions().ConfigureForXUnit(outputHelper, LogEventLevel.Verbose))
        {
        }

        [Fact]
        public async Task Should_Handle_FileCreate()
        {
            var (client, server) = await Initialize(Configure, Configure);

            await client.RequestWillCreateFiles(
                new WillCreateFilesParams
                {
                    Files = Container<FileCreate>.From(new FileCreate { Uri = new Uri("file://asdf") })
                }
            );
            client.DidCreateFiles(
                new DidCreateFilesParams
                {
                    Files = Container<FileCreate>.From(new FileCreate { Uri = new Uri("file://asdf") })
                }
            );

            await TestHelper.DelayUntil(() => _didCreateFileHandler.ReceivedCalls().Any(), CancellationToken);

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

        [Fact]
        public async Task Should_Handle_FileRename()
        {
            var (client, server) = await Initialize(Configure, Configure);

            await client.RequestWillRenameFiles(
                new WillRenameFilesParams
                {
                    Files = Container<FileRename>.From(new FileRename {
                        OldUri = new Uri("file://asdf"),
                        NewUri = new Uri("file://zxcv"),
                    })
                }
            );
            client.DidRenameFiles(
                new DidRenameFilesParams
                {
                    Files = Container<FileRename>.From(new FileRename {
                        OldUri = new Uri("file://asdf"),
                        NewUri = new Uri("file://zxcv")
                    })
                }
            );

            await TestHelper.DelayUntil(() => _didRenameFileHandler.ReceivedCalls().Any(), CancellationToken);

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

        [Fact]
        public async Task Should_Handle_FileDelete()
        {
            var (client, server) = await Initialize(Configure, Configure);

            await client.RequestWillDeleteFiles(
                new WillDeleteFilesParams
                {
                    Files = Container<FileDelete>.From(new FileDelete { Uri = new Uri("file://asdf") })
                }
            );
            client.DidDeleteFiles(
                new DidDeleteFilesParams
                {
                    Files = Container<FileDelete>.From(new FileDelete { Uri = new Uri("file://asdf") })
                }
            );

            await TestHelper.DelayUntil(() => _didDeleteFileHandler.ReceivedCalls().Any(), CancellationToken);

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
                new FileOperationsWorkspaceClientCapabilities
                {
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
                new FileOperationFilter
                {
                    Scheme = "file", Pattern = new FileOperationPattern
                    {
                        Glob = "**/*.cs",
                        Matches = FileOperationPatternKind.File,
                        Options = new FileOperationPatternOptions
                        {
                            IgnoreCase = true
                        }
                    }
                }
            );
            options.OnDidCreateFiles(_didCreateFileHandler, (capability, capabilities) => new() { Filters = filters });
            options.OnWillCreateFiles(_willCreateFileHandler, (capability, capabilities) => new() { Filters = filters });
            options.OnDidRenameFiles(_didRenameFileHandler, (capability, capabilities) => new() { Filters = filters });
            options.OnWillRenameFiles(_willRenameFileHandler, (capability, capabilities) => new() { Filters = filters });
            options.OnDidDeleteFiles(_didDeleteFileHandler, (capability, capabilities) => new() { Filters = filters });
            options.OnWillDeleteFiles(_willDeleteFileHandler, (capability, capabilities) => new() { Filters = filters });
        }
    }
}
