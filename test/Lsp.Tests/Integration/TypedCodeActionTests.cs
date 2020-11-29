using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using FluentAssertions;
using Lsp.Tests.Integration.Fixtures;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc.Testing;
using OmniSharp.Extensions.LanguageProtocol.Testing;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Serilog.Events;
using TestingUtils;
using Xunit;
using Xunit.Abstractions;
using Nested = Lsp.Tests.Integration.Fixtures.Nested;

namespace Lsp.Tests.Integration
{
    public class TypedCodeActionTests : LanguageProtocolTestBase
    {
        public TypedCodeActionTests(ITestOutputHelper outputHelper) : base(new JsonRpcTestOptions().ConfigureForXUnit(outputHelper, LogEventLevel.Verbose))
        {
        }

        [Fact]
        public async Task Should_Aggregate_With_All_Related_Handlers()
        {
            var (client, _) = await Initialize(
                options => { }, options => {
                    var identifier = Substitute.For<ITextDocumentIdentifier>();
                    identifier.GetTextDocumentAttributes(Arg.Any<DocumentUri>()).Returns(
                        call => new TextDocumentAttributes(call.ArgAt<DocumentUri>(0), "file", "csharp")
                    );
                    options.AddTextDocumentIdentifier(identifier);

                    options.OnCodeAction(
                        codeActionParams => {
                            return Task.FromResult(
                                new CodeActionContainer<Fixtures.Data>(
                                    new CodeAction<Fixtures.Data> {
                                        Title = "data-a",
                                        Kind = CodeActionKind.QuickFix,
                                        Command = new Command {
                                            Name = "data-a",
                                            Arguments = JArray.FromObject(new object[] { 1, "2", false })
                                        },
                                        Data = new Fixtures.Data {
                                            Child = new Nested {
                                                Date = DateTimeOffset.MinValue
                                            },
                                            Id = Guid.NewGuid(),
                                            Name = "name"
                                        }
                                    }
                                )
                            );
                        },
                        action => {
                            return Task.FromResult(action with { Command = action.Command with { Name = "resolved-a" } });
                        },
                        (_, _) => new CodeActionRegistrationOptions {
                            DocumentSelector = DocumentSelector.ForPattern("**/*.cs")
                        }
                    );

                    options.OnCodeAction(
                        codeActionParams => {
                            return Task.FromResult(
                                new CodeActionContainer<Nested>(
                                    new CodeAction<Nested> {
                                        Title = "nested-b",
                                        Kind = CodeActionKind.QuickFix,
                                        Command = new Command {
                                            Name = "nested-b",
                                            Arguments = JArray.FromObject(new object[] { 1, "2", false })
                                        },
                                        Data = new Nested {
                                            Date = DateTimeOffset.Now
                                        }
                                    }
                                )
                            );
                        },
                        action => {
                            return Task.FromResult(action with { Command = action.Command with { Name = "resolved-b" } });
                        },
                        (_, _) => new CodeActionRegistrationOptions {
                            DocumentSelector = DocumentSelector.ForPattern("**/*.cs")
                        }
                    );

                    options.OnCodeAction(
                        codeActionParams => {
                            return Task.FromResult(
                                new CommandOrCodeActionContainer(
                                    new CodeAction {
                                        Title = "no-data-c",
                                        Kind = CodeActionKind.QuickFix,
                                        Command = new Command {
                                            Name = "no-data-c",
                                            Arguments = JArray.FromObject(new object[] { 1, "2", false })
                                        }
                                    }
                                )
                            );
                        },
                        action => {
                            return Task.FromResult(action with { Command = action.Command with { Name = "resolved-c" } });
                        },
                        (_, _) => new CodeActionRegistrationOptions {
                            DocumentSelector = DocumentSelector.ForPattern("**/*.cs")
                        }
                    );

                    options.OnCodeAction(
                        codeActionParams => {
                            return Task.FromResult(
                                new CommandOrCodeActionContainer(
                                    new CodeAction {
                                        Title = "not-included",
                                        Kind = CodeActionKind.QuickFix,
                                        Command = new Command {
                                            Name = "not-included",
                                            Arguments = JArray.FromObject(new object[] { 1, "2", false })
                                        }
                                    }
                                )
                            );
                        },
                        action => {
                            return Task.FromResult(action with { Command = action.Command with { Name = "resolved-d" } });
                        },
                        (_, _) => new CodeActionRegistrationOptions {
                            DocumentSelector = DocumentSelector.ForLanguage("vb")
                        }
                    );
                }
            );

            var codeAction = await client.RequestCodeAction(
                new CodeActionParams {
                    TextDocument = new TextDocumentIdentifier("/some/path/file.cs"),
                }
            );

            var actions = codeAction.GetCodeActions().ToArray();

            var responses = await Task.WhenAll(actions.Select(z => client.ResolveCodeAction(z)));
            responses.Select(z => z.Command!.Name).Should().Contain(new[] { "resolved-a", "resolved-b", "resolved-c" });
            responses.Select(z => z.Command!.Name).Should().NotContain("resolved-d");
            actions.Length.Should().Be(3);
        }

        [Fact]
        public async Task Should_Resolve_With_Data_Capability()
        {
            var (client, _) = await Initialize(
                options => { }, options => {
                    options.OnCodeAction(
                        (codeActionParams, capability, token) => {
                            return Task.FromResult(
                                new CodeActionContainer<Fixtures.Data>(
                                    new CodeAction<Fixtures.Data> {
                                        Title = "name",
                                        Kind = CodeActionKind.QuickFix,
                                        Command = new Command {
                                            Name = "execute-a",
                                            Arguments = JArray.FromObject(new object[] { 1, "2", false })
                                        },
                                        Data = new Fixtures.Data {
                                            Child = new Nested {
                                                Date = DateTimeOffset.MinValue
                                            },
                                            Id = Guid.NewGuid(),
                                            Name = "name"
                                        }
                                    }
                                )
                            );
                        },
                        (codeAction, capability, token) => {
                            codeAction.Data.Id.Should().NotBeEmpty();
                            codeAction.Data.Child.Should().NotBeNull();
                            codeAction.Data.Name.Should().Be("name");
                            return Task.FromResult(codeAction with { Command = codeAction.Command with { Name = "resolved" } });
                        },
                        (_, _) => new CodeActionRegistrationOptions()
                    );
                }
            );

            var items = await client.RequestCodeAction(new CodeActionParams());

            var item = items.Single();

            item = await client.ResolveCodeAction(item.CodeAction!);
            item.CodeAction!.Command!.Name.Should().Be("resolved");
        }

        [RetryFact]
        public async Task Should_Resolve_With_Partial_Data_Capability()
        {
            var (client, _) = await Initialize(
                options => { }, options => {
                    options.ObserveCodeAction<Fixtures.Data>(
                        (codeActionParams, observer, capability, token) => {
                            var a = new CodeActionContainer<Fixtures.Data>(
                                new CodeAction<Fixtures.Data> {
                                    Title = "name",
                                    Kind = CodeActionKind.QuickFix,
                                    Command = new Command {
                                        Name = "execute-a",
                                        Arguments = JArray.FromObject(new object[] { 1, "2", false })
                                    },
                                    Data = new Fixtures.Data {
                                        Child = new Nested {
                                            Date = DateTimeOffset.MinValue
                                        },
                                        Id = Guid.NewGuid(),
                                        Name = "name"
                                    }
                                }
                            );

                            observer.OnNext(a);
                            observer.OnCompleted();
                        },
                        (codeAction, capability, token) => {
                            codeAction.Data.Id.Should().NotBeEmpty();
                            codeAction.Data.Child.Should().NotBeNull();
                            codeAction.Data.Name.Should().Be("name");
                            return Task.FromResult(codeAction with { Command = codeAction.Command with { Name = "resolved" } });
                        },
                        (_, _) => new CodeActionRegistrationOptions()
                    );
                }
            );

            var item = await client.RequestCodeAction(new CodeActionParams()).SelectMany(z => z).Take(1).ToTask(CancellationToken);

            item = await client.ResolveCodeAction(item.CodeAction!);
            item.CodeAction!.Command!.Name.Should().Be("resolved");
        }

        [Fact]
        public async Task Should_Resolve_With_Data_CancellationToken()
        {
            var (client, _) = await Initialize(
                options => { }, options => {
                    options.OnCodeAction(
                        (codeActionParams, token) => {
                            return Task.FromResult(
                                new CodeActionContainer<Fixtures.Data>(
                                    new CodeAction<Fixtures.Data> {
                                        Title = "execute-a",
                                        Kind = CodeActionKind.QuickFix,
                                        Command = new Command {
                                            Name = "execute-a",
                                            Arguments = JArray.FromObject(new object[] { 1, "2", false })
                                        },
                                        Data = new Fixtures.Data {
                                            Child = new Nested {
                                                Date = DateTimeOffset.MinValue
                                            },
                                            Id = Guid.NewGuid(),
                                            Name = "name"
                                        }
                                    }
                                )
                            );
                        },
                        (codeAction, token) => {
                            codeAction.Data.Id.Should().NotBeEmpty();
                            codeAction.Data.Child.Should().NotBeNull();
                            codeAction.Data.Name.Should().Be("name");
                            return Task.FromResult(codeAction with { Command = codeAction.Command with { Name = "resolved" } });
                        },
                        (_, _) => new CodeActionRegistrationOptions()
                    );
                }
            );

            var items = await client.RequestCodeAction(new CodeActionParams());

            var item = items.Single();

            item = await client.ResolveCodeAction(item.CodeAction!);
            item.CodeAction!.Command!.Name.Should().Be("resolved");
        }

        [RetryFact]
        public async Task Should_Resolve_With_Partial_Data_CancellationToken()
        {
            var (client, _) = await Initialize(
                options => { }, options => {
                    options.ObserveCodeAction<Fixtures.Data>(
                        (codeActionParams, observer, token) => {
                            var a = new CodeActionContainer<Fixtures.Data>(
                                new CodeAction<Fixtures.Data> {
                                    Title = "execute-a",
                                    Kind = CodeActionKind.QuickFix,
                                    Command = new Command {
                                        Name = "execute-a",
                                        Arguments = JArray.FromObject(new object[] { 1, "2", false })
                                    },
                                    Data = new Fixtures.Data {
                                        Child = new Nested {
                                            Date = DateTimeOffset.MinValue
                                        },
                                        Id = Guid.NewGuid(),
                                        Name = "name"
                                    }
                                }
                            );

                            observer.OnNext(a);
                            observer.OnCompleted();
                        },
                        (codeAction, token) => {
                            codeAction.Data.Id.Should().NotBeEmpty();
                            codeAction.Data.Child.Should().NotBeNull();
                            codeAction.Data.Name.Should().Be("name");
                            return Task.FromResult(codeAction with { Command = codeAction.Command with { Name = "resolved" } });
                        },
                        (_, _) => new CodeActionRegistrationOptions()
                    );
                }
            );

            var item = await client.RequestCodeAction(new CodeActionParams()).SelectMany(z => z).Take(1).ToTask(CancellationToken);

            item = await client.ResolveCodeAction(item.CodeAction!);
            item.CodeAction!.Command!.Name.Should().Be("resolved");
        }

        [Fact]
        public async Task Should_Resolve_With_Data()
        {
            var (client, _) = await Initialize(
                options => { }, options => {
                    options.OnCodeAction(
                        codeActionParams => {
                            return Task.FromResult(
                                new CodeActionContainer<Fixtures.Data>(
                                    new CodeAction<Data> {
                                        Title = "execute-a",
                                        Kind = CodeActionKind.QuickFix,
                                        Command = new Command {
                                            Name = "execute-a",
                                            Arguments = JArray.FromObject(new object[] { 1, "2", false })
                                        },
                                        Data = new Data {
                                            Child = new Nested {
                                                Date = DateTimeOffset.MinValue
                                            },
                                            Id = Guid.NewGuid(),
                                            Name = "name"
                                        }
                                    }
                                )
                            );
                        },
                        codeAction => {
                            codeAction.Data.Id.Should().NotBeEmpty();
                            codeAction.Data.Child.Should().NotBeNull();
                            codeAction.Data.Name.Should().Be("name");
                            return Task.FromResult(codeAction with { Command = codeAction.Command with { Name = "resolved" } });
                        },
                        (_, _) => new CodeActionRegistrationOptions()
                    );
                }
            );

            var items = await client.RequestCodeAction(new CodeActionParams());

            var item = items.Single();

            item = await client.ResolveCodeAction(item.CodeAction!);
            item.CodeAction!.Command!.Name.Should().Be("resolved");
        }

        [RetryFact]
        public async Task Should_Resolve_With_Partial_Data()
        {
            var (client, _) = await Initialize(
                options => { }, options => {
                    options.ObserveCodeAction<Data>(
                        (codeActionParams, observer) => {
                            var a = new CodeActionContainer<Data>(
                                new CodeAction<Data> {
                                    Title = "execute-a",
                                    Kind = CodeActionKind.QuickFix,
                                    Command = new Command {
                                        Name = "execute-a",
                                        Arguments = JArray.FromObject(new object[] { 1, "2", false })
                                    },
                                    Data = new Data {
                                        Child = new Nested {
                                            Date = DateTimeOffset.MinValue
                                        },
                                        Id = Guid.NewGuid(),
                                        Name = "name"
                                    }
                                }
                            );

                            observer.OnNext(a);
                            observer.OnCompleted();
                        },
                        codeAction => {
                            codeAction.Data.Id.Should().NotBeEmpty();
                            codeAction.Data.Child.Should().NotBeNull();
                            codeAction.Data.Name.Should().Be("name");
                            return Task.FromResult(codeAction with { Command = codeAction.Command with { Name = "resolved" } });
                        },
                        (_, _) => new CodeActionRegistrationOptions()
                    );
                }
            );

            var item = await client.RequestCodeAction(new CodeActionParams()).SelectMany(z => z).Take(1).ToTask(CancellationToken);

            item = await client.ResolveCodeAction(item.CodeAction!);
            item.CodeAction!.Command!.Name.Should().Be("resolved");
        }


        [Fact]
        public async Task Should_Resolve_Capability()
        {
            var (client, _) = await Initialize(
                options => { }, options => {
                    options.OnCodeAction(
                        (codeActionParams, capability, token) => {
                            return Task.FromResult(
                                new CommandOrCodeActionContainer(
                                    new CodeAction {
                                        Title = "execute-a",
                                        Kind = CodeActionKind.QuickFix,
                                        Command = new Command {
                                            Name = "execute-a",
                                            Arguments = JArray.FromObject(new object[] { 1, "2", false })
                                        }
                                    }
                                )
                            );
                        },
                        (codeAction, capability, token) => {
                            return Task.FromResult(codeAction with { Command = codeAction.Command with { Name = "resolved" } });
                        },
                        (_, _) => new CodeActionRegistrationOptions()
                    );
                }
            );

            var items = await client.RequestCodeAction(new CodeActionParams());

            var item = items.Single();

            item = await client.ResolveCodeAction(item.CodeAction!);
            item.CodeAction!.Command!.Name.Should().Be("resolved");
        }

        [RetryFact]
        public async Task Should_Resolve_Partial_Capability()
        {
            var (client, _) = await Initialize(
                options => { }, options => {
                    options.ObserveCodeAction(
                        (codeActionParams, observer, capability, token) => {
                            var a = new CommandOrCodeActionContainer(
                                new CodeAction {
                                    Title = "execute-a",
                                    Kind = CodeActionKind.QuickFix,
                                    Command = new Command {
                                        Name = "execute-a",
                                        Arguments = JArray.FromObject(new object[] { 1, "2", false })
                                    },
                                }
                            );

                            observer.OnNext(a);
                            observer.OnCompleted();
                        },
                        (codeAction, capability, token) => {
                            return Task.FromResult(codeAction with { Command = codeAction.Command with { Name = "resolved" } });
                        },
                        (_, _) => new CodeActionRegistrationOptions()
                    );
                }
            );

            var item = await client.RequestCodeAction(new CodeActionParams()).SelectMany(z => z).Take(1).ToTask(CancellationToken);

            item = await client.ResolveCodeAction(item.CodeAction!);
            item.CodeAction!.Command!.Name.Should().Be("resolved");
        }

        [Fact]
        public async Task Should_Resolve_CancellationToken()
        {
            var (client, _) = await Initialize(
                options => { }, options => {
                    options.OnCodeAction(
                        (codeActionParams, token) => {
                            return Task.FromResult(
                                new CommandOrCodeActionContainer(
                                    new CodeAction {
                                        Title = "execute-a",
                                        Kind = CodeActionKind.QuickFix,
                                        Command = new Command {
                                            Name = "execute-a",
                                            Arguments = JArray.FromObject(new object[] { 1, "2", false })
                                        },
                                    }
                                )
                            );
                        },
                        (codeAction, token) => {
                            return Task.FromResult(codeAction with { Command = codeAction.Command with { Name = "resolved" } });
                        },
                        (_, _) => new CodeActionRegistrationOptions()
                    );
                }
            );

            var items = await client.RequestCodeAction(new CodeActionParams());

            var item = items.Single();

            item = await client.ResolveCodeAction(item.CodeAction!);
            item.CodeAction!.Command!.Name.Should().Be("resolved");
        }

        [RetryFact]
        public async Task Should_Resolve_Partial_CancellationToken()
        {
            var (client, _) = await Initialize(
                options => { }, options => {
                    options.ObserveCodeAction(
                        (codeActionParams, observer, token) => {
                            var a = new CommandOrCodeActionContainer(
                                new CodeAction {
                                    Title = "execute-a",
                                    Kind = CodeActionKind.QuickFix,
                                    Command = new Command {
                                        Name = "execute-a",
                                        Arguments = JArray.FromObject(new object[] { 1, "2", false })
                                    },
                                }
                            );

                            observer.OnNext(a);
                            observer.OnCompleted();
                        },
                        (codeAction, token) => {
                            return Task.FromResult(codeAction with { Command = codeAction.Command with { Name = "resolved" } });
                        },
                        (_, _) => new CodeActionRegistrationOptions()
                    );
                }
            );

            var item = await client.RequestCodeAction(new CodeActionParams()).SelectMany(z => z).Take(1).ToTask(CancellationToken);

            item = await client.ResolveCodeAction(item.CodeAction!);
            item.CodeAction!.Command!.Name.Should().Be("resolved");
        }

        [Fact]
        public async Task Should_Resolve()
        {
            var (client, _) = await Initialize(
                options => { }, options => {
                    options.OnCodeAction(
                        codeActionParams => {
                            return Task.FromResult(
                                new CommandOrCodeActionContainer(
                                    new CodeAction {
                                        Title = "execute-a",
                                        Kind = CodeActionKind.QuickFix,
                                        Command = new Command {
                                            Name = "execute-a",
                                            Arguments = JArray.FromObject(new object[] { 1, "2", false })
                                        },
                                    }
                                )
                            );
                        },
                        codeAction => {
                            return Task.FromResult(codeAction with { Command = codeAction.Command with { Name = "resolved" } });
                        },
                        (_, _) => new CodeActionRegistrationOptions()
                    );
                }
            );

            var items = await client.RequestCodeAction(new CodeActionParams());

            var item = items.Single();

            item = await client.ResolveCodeAction(item.CodeAction!);
            item.CodeAction!.Command!.Name.Should().Be("resolved");
        }

        [RetryFact]
        public async Task Should_Resolve_Partial()
        {
            var (client, _) = await Initialize(
                options => { }, options => {
                    options.ObserveCodeAction(
                        (codeActionParams, observer) => {
                            var a = new CommandOrCodeActionContainer(
                                new CodeAction {
                                    Title = "execute-a",
                                    Kind = CodeActionKind.QuickFix,
                                    Command = new Command {
                                        Name = "execute-a",
                                        Arguments = JArray.FromObject(new object[] { 1, "2", false })
                                    },
                                }
                            );

                            observer.OnNext(a);
                            observer.OnCompleted();
                        },
                        codeAction => {
                            return Task.FromResult(codeAction with { Command = codeAction.Command! with { Name = "resolved"}});
                        },
                        (_, _) => new CodeActionRegistrationOptions()
                    );
                }
            );

            var item = await client.RequestCodeAction(new CodeActionParams()).SelectMany(z => z).Take(1).ToTask(CancellationToken);

            item = await client.ResolveCodeAction(item.CodeAction!);
            item.CodeAction!.Command!.Name.Should().Be("resolved");
        }
    }
}
