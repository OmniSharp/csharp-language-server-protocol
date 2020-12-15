using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Lsp.Tests.Integration.Fixtures;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.WorkDone;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.WorkDone;
using OmniSharp.Extensions.LanguageServer.Server;
using TestingUtils;
using Xunit;
using Xunit.Abstractions;

namespace Lsp.Tests.Integration
{
    public static class PartialItemsTests
    {
        public class Delegates : LanguageProtocolFixtureTest<DefaultOptions, DefaultClient, Delegates.DelegateServer>
        {
            public Delegates(ITestOutputHelper testOutputHelper, LanguageProtocolFixture<DefaultOptions, DefaultClient, DelegateServer> fixture) : base(testOutputHelper, fixture)
            {
            }

            [RetryFact(10)]
            public async Task Should_Behave_Like_A_Task()
            {
                var result = await Client.TextDocument.RequestCodeLens(
                    new CodeLensParams {
                        TextDocument = new TextDocumentIdentifier(@"c:\test.cs")
                    }, CancellationToken
                );

                result.Should().HaveCount(3);
                result.Select(z => z.Command!.Name).Should().ContainInOrder("CodeLens 1", "CodeLens 2", "CodeLens 3");
            }

            [RetryFact(10, skipOn: SkipOnPlatform.All)]
            public async Task Should_Behave_Like_An_Observable()
            {
                var items = await Client.TextDocument
                                        .RequestCodeLens(
                                             new CodeLensParams {
                                                 TextDocument = new TextDocumentIdentifier(@"c:\test.cs")
                                             }, CancellationToken
                                         )
                                        .Aggregate(
                                             new List<CodeLens>(), (acc, v) => {
                                                 acc.AddRange(v);
                                                 return acc;
                                             }
                                         )
                                        .ToTask(CancellationToken);

                items.Should().HaveCount(3);
                items.Select(z => z.Command!.Name).Should().ContainInOrder("CodeLens 1", "CodeLens 2", "CodeLens 3");
            }

            [RetryFact(10)]
            public async Task Should_Behave_Like_An_Observable_Without_Progress_Support()
            {
                var response = await Client.SendRequest(
                    new CodeLensParams {
                        TextDocument = new TextDocumentIdentifier(@"c:\test.cs")
                    }, CancellationToken
                );

                response.Should().HaveCount(3);
                response.Select(z => z.Command!.Name).Should().ContainInOrder("CodeLens 1", "CodeLens 2", "CodeLens 3");
            }

            public class DelegateServer : IConfigureLanguageServerOptions
            {
                public void Configure(LanguageServerOptions options) =>
                    options.ObserveCodeLens(
                        (@params, observer, capability, cancellationToken) => {
                            observer.OnNext(
                                new[] {
                                    new CodeLens {
                                        Command = new Command {
                                            Name = "CodeLens 1"
                                        }
                                    },
                                }
                            );
                            observer.OnNext(
                                new[] {
                                    new CodeLens {
                                        Command = new Command {
                                            Name = "CodeLens 2"
                                        }
                                    },
                                }
                            );
                            observer.OnNext(
                                new[] {
                                    new CodeLens {
                                        Command = new Command {
                                            Name = "CodeLens 3"
                                        }
                                    },
                                }
                            );
                            observer.OnCompleted();
                        }, (_, _) => new CodeLensRegistrationOptions()
                    );
            }
        }

        public class Handlers : LanguageProtocolFixtureTest<DefaultOptions, DefaultClient, Handlers.HandlersServer>
        {
            public Handlers(ITestOutputHelper testOutputHelper, LanguageProtocolFixture<DefaultOptions, DefaultClient, HandlersServer> fixture) : base(testOutputHelper, fixture)
            {
            }

            [RetryFact(10)]
            public async Task Should_Behave_Like_An_Observable_With_WorkDone()
            {
                var items = new List<CodeLens>();
                var work = new List<WorkDoneProgress>();
                Client.TextDocument
                      .ObserveWorkDone(
                           new CodeLensParams { TextDocument = new TextDocumentIdentifier(@"c:\test.cs") },
                           (client, request) => CodeLensExtensions.RequestCodeLens(client, request, CancellationToken),
                           Observer.Create<WorkDoneProgress>(z => work.Add(z))
                       ).Subscribe(x => items.AddRange(x));


                await work.DelayUntilCount(6, CancellationToken);
                await SettleNext();

                var workResults = work.Select(z => z.Message);

                items.Should().HaveCount(4);
                items.Select(z => z.Command!.Name).Should().ContainInOrder("CodeLens 1", "CodeLens 2", "CodeLens 3", "CodeLens 4");

                workResults.Should().ContainInOrder("Begin", "Report 1", "Report 2", "Report 3", "Report 4", "End");
            }


            public class HandlersServer : IConfigureLanguageServerOptions
            {
                public void Configure(LanguageServerOptions options) => options.AddHandler<InnerCodeLensHandler>();
            }
        }

        private class InnerCodeLensHandler : CodeLensHandlerBase
        {
            private readonly IServerWorkDoneManager _workDoneManager;
            private readonly IProgressManager _progressManager;

            public InnerCodeLensHandler(IServerWorkDoneManager workDoneManager, IProgressManager progressManager)
            {
                _workDoneManager = workDoneManager;
                _progressManager = progressManager;
            }

            public override async Task<CodeLensContainer> Handle(CodeLensParams request, CancellationToken cancellationToken)
            {
                var partial = _progressManager.For(request, cancellationToken);
                var workDone = _workDoneManager.For(
                    request, new WorkDoneProgressBegin {
                        Cancellable = true,
                        Message = "Begin",
                        Percentage = 0,
                        Title = "Work is pending"
                    }, onComplete: () => new WorkDoneProgressEnd {
                        Message = "End"
                    }
                );

                partial.OnNext(
                    new[] {
                        new CodeLens {
                            Command = new Command {
                                Name = "CodeLens 1"
                            }
                        },
                    }
                );
                workDone.OnNext(
                    new WorkDoneProgressReport {
                        Percentage = 10,
                        Message = "Report 1"
                    }
                );

                partial.OnNext(
                    new[] {
                        new CodeLens {
                            Command = new Command {
                                Name = "CodeLens 2"
                            }
                        },
                    }
                );
                workDone.OnNext(
                    new WorkDoneProgressReport {
                        Percentage = 20,
                        Message = "Report 2"
                    }
                );

                partial.OnNext(
                    new[] {
                        new CodeLens {
                            Command = new Command {
                                Name = "CodeLens 3"
                            }
                        },
                    }
                );
                workDone.OnNext(
                    new WorkDoneProgressReport {
                        Percentage = 30,
                        Message = "Report 3"
                    }
                );

                partial.OnNext(
                    new[] {
                        new CodeLens {
                            Command = new Command {
                                Name = "CodeLens 4"
                            }
                        },
                    }
                );
                workDone.OnNext(
                    new WorkDoneProgressReport {
                        Percentage = 40,
                        Message = "Report 4"
                    }
                );

                workDone.OnCompleted();

                await Task.Yield();

                return new CodeLensContainer();
            }

            public override Task<CodeLens> Handle(CodeLens request, CancellationToken cancellationToken) => Task.FromResult(request);
            protected internal override CodeLensRegistrationOptions CreateRegistrationOptions(CodeLensCapability capability, ClientCapabilities clientCapabilities) => new CodeLensRegistrationOptions();
        }
    }
}
