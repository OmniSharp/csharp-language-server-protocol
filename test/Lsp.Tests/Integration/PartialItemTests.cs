using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc.Testing;
using OmniSharp.Extensions.LanguageProtocol.Testing;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.WorkDone;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.WorkDone;
using OmniSharp.Extensions.LanguageServer.Server;
using Xunit;
using Xunit.Abstractions;

namespace Lsp.Tests.Integration
{
    public class PartialItemTests : LanguageProtocolTestBase
    {
        public PartialItemTests(ITestOutputHelper outputHelper) : base(new JsonRpcTestOptions()
            .ConfigureForXUnit(outputHelper)
            .WithSettleTimeSpan(TimeSpan.FromMilliseconds(500))
        )
        {
        }

        [Fact]
        public async Task Should_Behave_Like_A_Task()
        {
            var (client, server) = await Initialize(ConfigureClient, ConfigureServerWithDelegateCodeLens);
            var result = await client.TextDocument.RequestCodeLens(new CodeLensParams() {
                TextDocument = new TextDocumentIdentifier(@"c:\test.cs")
            }, CancellationToken);

            result.Should().HaveCount(3);
            result.Select(z => z.Command.Name).Should().ContainInOrder("CodeLens 1", "CodeLens 2", "CodeLens 3");
        }

        [Fact]
        public async Task Should_Behave_Like_An_Observable()
        {
            var (client, server) = await Initialize(ConfigureClient, ConfigureServerWithDelegateCodeLens);

            var items = new List<CodeLens>();
            client.TextDocument.RequestCodeLens(new CodeLensParams() {
                TextDocument = new TextDocumentIdentifier(@"c:\test.cs")
            }, CancellationToken).Subscribe(x => items.AddRange(x));

            await SettleNext();

            items.Should().HaveCount(3);
            items.Select(z => z.Command.Name).Should().ContainInOrder("CodeLens 1", "CodeLens 2", "CodeLens 3");
        }

        [Fact]
        public async Task Should_Behave_Like_An_Observable_With_WorkDone()
        {
            var (client, server) = await Initialize(ConfigureClient, ConfigureServerWithClassCodeLens);

            var items = new List<CodeLens>();
            var work = new List<WorkDoneProgress>();
            client.TextDocument
                .ObserveWorkDone(
                    new CodeLensParams() {TextDocument = new TextDocumentIdentifier(@"c:\test.cs")},
                    (client, request) => CodeLensExtensions.RequestCodeLens(client, request, CancellationToken),
                    Observer.Create<WorkDoneProgress>(z => work.Add(z))
                ).Subscribe(x => items.AddRange(x));

            await SettleNext();

            var workResults = work.Select(z => z.Message);

            items.Should().HaveCount(4);
            items.Select(z => z.Command.Name).Should().ContainInOrder("CodeLens 1", "CodeLens 2", "CodeLens 3", "CodeLens 4");

            workResults.Should().ContainInOrder("Begin", "Report 1", "Report 2", "Report 3", "Report 4", "End");
        }

        private void ConfigureClient(LanguageClientOptions options)
        {

        }

        private void ConfigureServerWithDelegateCodeLens(LanguageServerOptions options)
        {
            options.OnCodeLens((@params, observer, capability, cancellationToken) => {
                observer.OnNext(new [] {
                    new CodeLens() {
                        Command = new Command() {
                            Name = "CodeLens 1"
                        }
                    },
                });
                observer.OnNext(new [] {
                    new CodeLens() {
                        Command = new Command() {
                            Name = "CodeLens 2"
                        }
                    },
                });
                observer.OnNext(new [] {
                    new CodeLens() {
                        Command = new Command() {
                            Name = "CodeLens 3"
                        }
                    },
                });
                observer.OnCompleted();
            }, new CodeLensRegistrationOptions() {
                // DocumentSelector = DocumentSelector.ForPattern("**/*.cs")
            });
        }

        private void ConfigureServerWithClassCodeLens(LanguageServerOptions options)
        {
            options.AddHandler<InnerCodeLensHandler>();
        }

        class InnerCodeLensHandler : CodeLensHandler
        {
            private readonly IServerWorkDoneManager _workDoneManager;
            private readonly IProgressManager _progressManager;

            public InnerCodeLensHandler(IServerWorkDoneManager workDoneManager, IProgressManager progressManager) : base(new CodeLensRegistrationOptions())
            {
                _workDoneManager = workDoneManager;
                _progressManager = progressManager;
            }

            public override async Task<CodeLensContainer> Handle(CodeLensParams request, CancellationToken cancellationToken)
            {
                var partial = _progressManager.For(request, cancellationToken);
                var workDone = _workDoneManager.For(request, new WorkDoneProgressBegin() {

                    Cancellable = true,
                    Message = "Begin",
                    Percentage = 0,
                    Title = "Work is pending"
                }, onComplete: () => new WorkDoneProgressEnd() {
                    Message = "End"
                });

                partial.OnNext(new [] {
                    new CodeLens() {
                        Command = new Command() {
                            Name = "CodeLens 1"
                        }
                    },
                });
                workDone.OnNext(new WorkDoneProgressReport() {
                    Percentage = 10,
                    Message = "Report 1"
                });

                partial.OnNext(new [] {
                    new CodeLens() {
                        Command = new Command() {
                            Name = "CodeLens 2"
                        }
                    },
                });
                workDone.OnNext(new WorkDoneProgressReport() {
                    Percentage = 20,
                    Message = "Report 2"
                });

                partial.OnNext(new [] {
                    new CodeLens() {
                        Command = new Command() {
                            Name = "CodeLens 3"
                        }
                    },
                });
                workDone.OnNext(new WorkDoneProgressReport() {
                    Percentage = 30,
                    Message = "Report 3"
                });

                partial.OnNext(new [] {
                    new CodeLens() {
                        Command = new Command() {
                            Name = "CodeLens 4"
                        }
                    },
                });
                workDone.OnNext(new WorkDoneProgressReport() {
                    Percentage = 40,
                    Message = "Report 4"
                });

                workDone.OnCompleted();

                await Task.Yield();

                return new CodeLensContainer();
            }

            public override Task<CodeLens> Handle(CodeLens request, CancellationToken cancellationToken) => Task.FromResult(request);

            public override bool CanResolve(CodeLens value) => true;
        }

    }
}
