using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc.Server;
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
using ISerializer = OmniSharp.Extensions.LanguageServer.Protocol.Serialization.ISerializer;

namespace Lsp.Tests.Integration
{

    public class RequestCancellation : LanguageProtocolTestBase
    {
        public RequestCancellation(ITestOutputHelper outputHelper) : base(new JsonRpcTestOptions()
            .ConfigureForXUnit(outputHelper)
            .WithSettleTimeSpan(TimeSpan.FromMilliseconds(200))
        )
        {
        }

        [Fact]
        public async Task Should_Cancel_Pending_Requests()
        {
            var (client, server) = await Initialize(ConfigureClient, ConfigureServer);

            var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(10));
            CancellationToken.Register(cts.Cancel);
            Func<Task<CompletionList>> action = () => client.TextDocument.RequestCompletion(new CompletionParams() {
                TextDocument = "/a/file.cs"
            }, cts.Token).AsTask();
            action.Should().Throw<TaskCanceledException>();

        }

        [Fact]
        public async Task Should_Abandon_Pending_Requests_For_Text_Changes()
        {
            var (client, server) = await Initialize(ConfigureClient, ConfigureServer);

            var request1 = client.TextDocument.RequestCompletion(new CompletionParams() {
                TextDocument = "/a/file.cs"
            }, CancellationToken).AsTask();

            client.TextDocument.DidChangeTextDocument(new DidChangeTextDocumentParams() {
                TextDocument = new VersionedTextDocumentIdentifier() {
                    Uri = "/a/file.cs",
                    Version = 123,
                },
                ContentChanges = new Container<TextDocumentContentChangeEvent>()
            });

            await SettleNext();

            Func<Task> action = () => request1;
            action.Should().Throw<ContentModifiedException>();
        }

        private void ConfigureClient(LanguageClientOptions options)
        {

        }

        private void ConfigureServer(LanguageServerOptions options)
        {
            options.WithContentModifiedSupport(true);
            options.OnCompletion(async (x, ct) => {
                await Task.Delay(5000, ct);
                return new CompletionList();
            }, new CompletionRegistrationOptions());
            options.OnDidChangeTextDocument(async x => {
                await Task.Delay(20);
            }, new TextDocumentChangeRegistrationOptions());
        }
    }

    public class PartialItemTests : LanguageProtocolTestBase
    {
        public PartialItemTests(ITestOutputHelper outputHelper) : base(new JsonRpcTestOptions()
            .ConfigureForXUnit(outputHelper)
            .WithSettleTimeSpan(TimeSpan.FromMilliseconds(200))
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
                    (client, request) => client.RequestCodeLens(request, CancellationToken),
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

    public class ProgressTests : LanguageProtocolTestBase
    {
        public ProgressTests(ITestOutputHelper outputHelper) : base(new JsonRpcTestOptions()
            .ConfigureForXUnit(outputHelper)
            .WithSettleTimeSpan(TimeSpan.FromMilliseconds(200))
        )
        {
        }

        class Data
        {
            public string Value { get; set; } = "Value";
        }

        [Fact]
        public async Task Should_Send_Progress_From_Server_To_Client()
        {
            var (client, server) = await Initialize(ConfigureClient, ConfigureServer);
            var token = new ProgressToken(Guid.NewGuid().ToString());

            var data = new List<string>();

            var observer = client.ProgressManager.For<Data>(token, CancellationToken);
            server.ProgressManager.Monitor(token, x => x.ToObject<Data>(server.GetRequiredService<ISerializer>().JsonSerializer)).Subscribe(x => data.Add(x.Value));

            observer.OnNext(new Data() {
                Value = "1"
            });
            observer.OnNext(new Data() {
                Value = "3"
            });
            observer.OnNext(new Data() {
                Value = "2"
            });
            observer.OnNext(new Data() {
                Value = "4"
            });
            observer.OnNext(new Data() {
                Value = "5"
            });

            await SettleNext();
            observer.OnCompleted();

            data.Should().ContainInOrder("1", "3", "2", "4", "5");
        }

        [Fact]
        public async Task Should_Send_Progress_From_Client_To_Server()
        {
            var (client, server) = await Initialize(ConfigureClient, ConfigureServer);
            var token = new ProgressToken(Guid.NewGuid().ToString());

            var data = new List<string>();

            using var observer = server.ProgressManager.For<Data>(token, CancellationToken);
            client.ProgressManager.Monitor(token, x => x.ToObject<Data>(client.GetRequiredService<ISerializer>().JsonSerializer)).Subscribe(x => data.Add(x.Value));

            observer.OnNext(new Data() {
                Value = "1"
            });
            observer.OnNext(new Data() {
                Value = "3"
            });
            observer.OnNext(new Data() {
                Value = "2"
            });
            observer.OnNext(new Data() {
                Value = "4"
            });
            observer.OnNext(new Data() {
                Value = "5"
            });

            await SettleNext();
            observer.OnCompleted();

            data.Should().ContainInOrder("1", "3", "2", "4", "5");
        }

        [Fact]
        public async Task WorkDone_Should_Be_Supported()
        {
            var (client, server) = await Initialize(ConfigureClient, ConfigureServer);
            server.WorkDoneManager.IsSupported.Should().BeTrue();
            client.WorkDoneManager.IsSupported.Should().BeTrue();
        }

        [Fact]
        public async Task Should_Support_Creating_Work_Done_From_Sever_To_Client()
        {
            var (client, server) = await Initialize(ConfigureClient, ConfigureServer);
            var token = new ProgressToken(Guid.NewGuid().ToString());

            var data = new List<WorkDoneProgress>();
            using var workDoneObservable = client.WorkDoneManager.Monitor(token);
            workDoneObservable.Subscribe(x => data.Add(x));

            using var workDoneObserver = await server.WorkDoneManager.Create(token, new WorkDoneProgressBegin() {
                Cancellable = true,
                Message = "Begin",
                Percentage = 0,
                Title = "Work is pending"
            }, onComplete: () => new WorkDoneProgressEnd() {
                Message = "End"
            });

            workDoneObserver.OnNext(new WorkDoneProgressReport() {
                Percentage = 10,
                Message = "Report 1"
            });

            workDoneObserver.OnNext(new WorkDoneProgressReport() {
                Percentage = 20,
                Message = "Report 2"
            });

            workDoneObserver.OnNext(new WorkDoneProgressReport() {
                Percentage = 30,
                Message = "Report 3"
            });

            workDoneObserver.OnNext(new WorkDoneProgressReport() {
                Percentage = 40,
                Message = "Report 4"
            });

            workDoneObserver.OnCompleted();

            await SettleNext();

            var results = data.Select(z => z switch {
                WorkDoneProgressBegin begin => begin.Message,
                WorkDoneProgressReport begin => begin.Message,
                WorkDoneProgressEnd begin => begin.Message,
            });

            results.Should().ContainInOrder("Begin", "Report 1", "Report 2", "Report 3", "Report 4", "End");
        }

        [Fact]
        public async Task Should_Support_Observing_Work_Done_From_Client_To_Server_Request()
        {
            var (client, server) = await Initialize(ConfigureClient, ConfigureServer);
            var token = new ProgressToken(Guid.NewGuid().ToString());

            var data = new List<WorkDoneProgress>();
            using var workDoneObservable = client.WorkDoneManager.Monitor(token);
            workDoneObservable.Subscribe(x => data.Add(x));

            using var workDoneObserver = await server.WorkDoneManager.Create(token, new WorkDoneProgressBegin() {
                Cancellable = true,
                Message = "Begin",
                Percentage = 0,
                Title = "Work is pending"
            }, onComplete: () => new WorkDoneProgressEnd() {
                Message = "End"
            });

            workDoneObserver.OnNext(new WorkDoneProgressReport() {
                Percentage = 10,
                Message = "Report 1"
            });

            workDoneObserver.OnNext(new WorkDoneProgressReport() {
                Percentage = 20,
                Message = "Report 2"
            });

            workDoneObserver.OnNext(new WorkDoneProgressReport() {
                Percentage = 30,
                Message = "Report 3"
            });

            workDoneObserver.OnNext(new WorkDoneProgressReport() {
                Percentage = 40,
                Message = "Report 4"
            });

            workDoneObserver.OnCompleted();

            await SettleNext();

            var results = data.Select(z => z switch {
                WorkDoneProgressBegin begin => begin.Message,
                WorkDoneProgressReport begin => begin.Message,
                WorkDoneProgressEnd begin => begin.Message,
            });

            results.Should().ContainInOrder("Begin", "Report 1", "Report 2", "Report 3", "Report 4", "End");
        }

        private void ConfigureClient(LanguageClientOptions options)
        {

        }

        private void ConfigureServer(LanguageServerOptions options)
        {
            // options.OnCodeLens()
        }
    }
}
