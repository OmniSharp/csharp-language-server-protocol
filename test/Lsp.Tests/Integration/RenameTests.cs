using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NSubstitute.Callbacks;
using OmniSharp.Extensions.JsonRpc.Testing;
using OmniSharp.Extensions.LanguageProtocol.Testing;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Server;
using Serilog.Events;
using TestingUtils;
using Xunit;
using Xunit.Abstractions;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Lsp.Tests.Integration
{
    public class RenameTests : LanguageProtocolTestBase
    {
        private readonly Func<PrepareRenameParams, RenameCapability, CancellationToken, Task<RangeOrPlaceholderRange?>> _prepareRename;
        private readonly Func<RenameParams, RenameCapability, CancellationToken, Task<WorkspaceEdit?>> _rename;

        public RenameTests(ITestOutputHelper outputHelper) : base(new JsonRpcTestOptions().ConfigureForXUnit(outputHelper, LogEventLevel.Verbose))
        {
            _prepareRename = Substitute.For<Func<PrepareRenameParams, RenameCapability, CancellationToken, Task<RangeOrPlaceholderRange?>>>();
            _rename = Substitute.For<Func<RenameParams, RenameCapability, CancellationToken, Task<WorkspaceEdit?>>>();
        }

        [Fact]
        public async Task Should_Handle_Rename_With_No_Value()
        {
            _prepareRename.Invoke(Arg.Any<PrepareRenameParams>(), Arg.Any<RenameCapability>(), Arg.Any<CancellationToken>())
                          .Returns(
                               call => {
                                   var pos = call.Arg<PrepareRenameParams>().Position;
                                   return new RangeOrPlaceholderRange(
                                       new Range(
                                           pos,
                                           ( pos.Line, pos.Character + 2 )
                                       )
                                   );
                               }
                           );

            _rename.Invoke(Arg.Any<RenameParams>(), Arg.Any<RenameCapability>(), Arg.Any<CancellationToken>())
                   .Returns(
                        new WorkspaceEdit() {
                            DocumentChanges = new Container<WorkspaceEditDocumentChange>(new WorkspaceEditDocumentChange(new CreateFile() {
                                Uri = DocumentUri.FromFileSystemPath("/abcd/create.cs")
                            }))
                        }
                    );

            var (client, _) = await Initialize(ClientOptionsAction, ServerOptionsAction);

            var result = await client.PrepareRename(
                new PrepareRenameParams() {
                    Position = ( 1, 1 ),
                    TextDocument = DocumentUri.FromFileSystemPath("/abcd/file.cs")
                },
                CancellationToken
            );

            result.Should().NotBeNull();

            var renameResponse = await client.RequestRename(
                new RenameParams() {
                    Position = result!.Range!.Start,
                    NewName = "newname",
                    TextDocument = DocumentUri.FromFileSystemPath("/abcd/file.cs")
                }
            );

            renameResponse!.DocumentChanges.Should().HaveCount(1);
            renameResponse.DocumentChanges.Should().Match(z => z.Any(x => x.IsCreateFile));

            // Ensure capability was provided to both sides as needed
            _prepareRename.Received(1).Invoke(Arg.Any<PrepareRenameParams>(), Arg.Is<RenameCapability>(z => z.PrepareSupport), Arg.Any<CancellationToken>());
            _rename.Received(1).Invoke(Arg.Any<RenameParams>(), Arg.Is<RenameCapability>(z => z.PrepareSupport), Arg.Any<CancellationToken>());
            var capability1 = _prepareRename.ReceivedCalls().Select(z => z.GetArguments()[1] as RenameCapability).FirstOrDefault();
            var capability2 = _rename.ReceivedCalls().Select(z => z.GetArguments()[1] as RenameCapability).FirstOrDefault();
            capability1.Should().BeSameAs(capability2);
        }

        [Fact]
        public async Task Should_Handle_Prepare_Rename_With_No_Value()
        {
            _prepareRename.Invoke(Arg.Any<PrepareRenameParams>(), Arg.Any<RenameCapability>(), Arg.Any<CancellationToken>())
                          .Returns(Task.FromResult<RangeOrPlaceholderRange?>(null)!);
            var (client, _) = await Initialize(ClientOptionsAction, ServerOptionsAction);

            var result = await client.PrepareRename(
                new PrepareRenameParams() {
                    Position = ( 1, 1 ),
                    TextDocument = DocumentUri.FromFileSystemPath("/abcd/file.cs")
                },
                CancellationToken
            );

            result.Should().BeNull();
        }

        [Fact]
        public async Task Should_Handle_Prepare_Rename_With_Range()
        {
            _prepareRename.Invoke(Arg.Any<PrepareRenameParams>(), Arg.Any<RenameCapability>(), Arg.Any<CancellationToken>())
                          .Returns(
                               call => {
                                   var pos = call.Arg<PrepareRenameParams>().Position;
                                   return new RangeOrPlaceholderRange(
                                       new Range(
                                           pos,
                                           ( pos.Line, pos.Character + 2 )
                                       )
                                   );
                               }
                           );

            var (client, _) = await Initialize(ClientOptionsAction, ServerOptionsAction);

            var result = await client.PrepareRename(
                new PrepareRenameParams() {
                    Position = ( 1, 1 ),
                    TextDocument = DocumentUri.FromFileSystemPath("/abcd/file.cs")
                },
                CancellationToken
            );

            result!.IsRange.Should().BeTrue();
        }

        [Fact]
        public async Task Should_Handle_Prepare_Rename_With_PlaceholderRange()
        {
            _prepareRename.Invoke(Arg.Any<PrepareRenameParams>(), Arg.Any<RenameCapability>(), Arg.Any<CancellationToken>())
                          .Returns(
                               call => {
                                   var pos = call.Arg<PrepareRenameParams>().Position;
                                   return new RangeOrPlaceholderRange(
                                       new PlaceholderRange() {
                                           Range =
                                               new Range(
                                                   pos,
                                                   ( pos.Line, pos.Character + 2 )
                                               ),
                                           Placeholder = "Bogus"
                                       }
                                   );
                               }
                           );

            var (client, _) = await Initialize(ClientOptionsAction, ServerOptionsAction);

            var result = await client.PrepareRename(
                new PrepareRenameParams() {
                    Position = ( 1, 1 ),
                    TextDocument = DocumentUri.FromFileSystemPath("/abcd/file.cs")
                },
                CancellationToken
            );

            result!.IsPlaceholderRange.Should().BeTrue();
        }

        [Fact]
        public async Task Should_Handle_Prepare_Rename_With_DefaultBehavior()
        {
            _prepareRename.Invoke(Arg.Any<PrepareRenameParams>(), Arg.Any<RenameCapability>(), Arg.Any<CancellationToken>())
                          .Returns(
                               call => new RangeOrPlaceholderRange(
                                   new RenameDefaultBehavior() {
                                       DefaultBehavior = true
                                   }
                               )
                           );

            var (client, _) = await Initialize(ClientOptionsAction, ServerOptionsAction);

            var result = await client.PrepareRename(
                new PrepareRenameParams() {
                    Position = ( 1, 1 ),
                    TextDocument = DocumentUri.FromFileSystemPath("/abcd/file.cs")
                },
                CancellationToken
            );

            result!.IsDefaultBehavior.Should().BeTrue();
        }

        [Fact]
        public async Task Should_Not_Register_Prepare_Rename()
        {
            var (client, _) = await Initialize(ClientOptionsAction, ServerOptionsAction);
            await client.DelayUntil(
                c => client.RegistrationManager.CurrentRegistrations.Any(z => z.Method == TextDocumentNames.Rename),
                CancellationToken
            );

            await SettleNext();

            client.RegistrationManager.CurrentRegistrations.Should().NotContain(z => z.Method == TextDocumentNames.PrepareRename);
        }

        private void ServerOptionsAction(LanguageServerOptions obj)
        {
            obj.OnPrepareRename(
                _prepareRename, new TextDocumentRegistrationOptions() {
                    DocumentSelector = DocumentSelector.ForLanguage("csharp")
                }
            );
            obj.OnRename(
                _rename, new RenameRegistrationOptions() {
                    DocumentSelector = DocumentSelector.ForLanguage("csharp"),
                    PrepareProvider = true,
                }
            );
        }

        private void ClientOptionsAction(LanguageClientOptions obj)
        {
            obj.WithCapability(
                new RenameCapability() {
                    PrepareSupport = true,
                    PrepareSupportDefaultBehavior = true
                }
            );
        }
    }
}
