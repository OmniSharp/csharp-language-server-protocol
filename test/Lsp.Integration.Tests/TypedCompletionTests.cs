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
using IHandlerIdentity = OmniSharp.Extensions.LanguageServer.Protocol.Models.IHandlerIdentity;
using Nested = Lsp.Tests.Integration.Fixtures.Nested;

namespace Lsp.Tests.Integration
{
    public class TypedCompletionTests : LanguageProtocolTestBase
    {
        public TypedCompletionTests(ITestOutputHelper outputHelper) : base(new JsonRpcTestOptions().ConfigureForXUnit(outputHelper, LogEventLevel.Verbose))
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

                    options.OnCompletion(
                        codeLensParams => {
                            return Task.FromResult(
                                new CompletionList<Fixtures.Data>(
                                    new CompletionItem<Fixtures.Data> {
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
                        completionItem => { return Task.FromResult(completionItem with { Command = completionItem.Command with { Name = "resolved-a" } }); },
                        (_, _) => new CompletionRegistrationOptions {
                            DocumentSelector = DocumentSelector.ForPattern("**/*.cs")
                        }
                    );

                    options.OnCompletion(
                        codeLensParams => {
                            return Task.FromResult(
                                new CompletionList<Nested>(
                                    new CompletionItem<Nested> {
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
                        completionItem => { return Task.FromResult(completionItem with { Command = completionItem.Command with { Name = "resolved-b" } }); },
                        (_, _) => new CompletionRegistrationOptions {
                            DocumentSelector = DocumentSelector.ForPattern("**/*.cs")
                        }
                    );

                    options.OnCompletion(
                        codeLensParams => {
                            return Task.FromResult(
                                new CompletionList(
                                    new CompletionItem {
                                        Command = new Command {
                                            Name = "no-data-c",
                                            Arguments = JArray.FromObject(new object[] { 1, "2", false })
                                        }
                                    }
                                )
                            );
                        },
                        completionItem => { return Task.FromResult(completionItem with { Command = completionItem.Command with { Name = "resolved-c" } }); },
                        (_, _) => new CompletionRegistrationOptions {
                            DocumentSelector = DocumentSelector.ForPattern("**/*.cs")
                        }
                    );

                    options.OnCompletion(
                        codeLensParams => {
                            return Task.FromResult(
                                new CompletionList(
                                    new CompletionItem {
                                        Command = new Command {
                                            Name = "not-included",
                                            Arguments = JArray.FromObject(new object[] { 1, "2", false })
                                        }
                                    }
                                )
                            );
                        },
                        completionItem => { return Task.FromResult(completionItem with { Command = completionItem.Command with { Name = "resolved-d" }}); },
                        (_, _) => new CompletionRegistrationOptions {
                            DocumentSelector = DocumentSelector.ForLanguage("vb")
                        }
                    );
                }
            );

            var items = await client.RequestCompletion(
                new CompletionParams {
                    TextDocument = new TextDocumentIdentifier("/some/path/file.cs"),
                }
            );

            var lens = items.ToArray();

            var responses = await Task.WhenAll(lens.Select(z => client.ResolveCompletion(z)));
            responses.Select(z => z.Command!.Name).Should().Contain(new[] { "resolved-a", "resolved-b", "resolved-c" });
            responses.Select(z => z.Command!.Name).Should().NotContain("resolved-d");
            lens.Length.Should().Be(3);
        }

        [Fact]
        public async Task Should_Resolve_With_Data_Capability()
        {
            var (client, _) = await Initialize(
                options => { }, options => {
                    options.OnCompletion(
                        (completionParams, capability, token) => {
                            return Task.FromResult(
                                new CompletionList<Fixtures.Data>(
                                    new CompletionItem<Fixtures.Data> {
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
                        (completionItem, capability, token) => {
                            completionItem.Data!.Id.Should().NotBeEmpty();
                            completionItem.Data!.Child.Should().NotBeNull();
                            completionItem.Data!.Name.Should().Be("name");
                            return Task.FromResult(completionItem with { Detail = "resolved" });
                            return Task.FromResult(
                                completionItem with {
                                    Detail = "resolved"
                                    }
                            );
                        },
                        (_, _) => new CompletionRegistrationOptions()
                    );
                }
            );

            var items = await client.RequestCompletion(new CompletionParams());

            var item = items.Items.Single();

            item = await client.ResolveCompletion(item);
            item.Detail.Should().Be("resolved");
        }

        [Fact]
        public async Task Should_Resolve_With_Partial_Data_Capability()
        {
            var (client, _) = await Initialize(
                options => { }, options => {
                    options.ObserveCompletion<Fixtures.Data>(
                        (completionParams, observer, capability, token) => {
                            var a = new CompletionList<Fixtures.Data>(
                                new CompletionItem<Fixtures.Data> {
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
                        (completionItem, capability, token) => {
                            completionItem.Data!.Id.Should().NotBeEmpty();
                            completionItem.Data!.Child.Should().NotBeNull();
                            completionItem.Data!.Name.Should().Be("name");
                            return Task.FromResult(completionItem with { Detail = "resolved" });
                },
                        (_, _) => new CompletionRegistrationOptions()
                    );
                }
            );

            var completionList = await client.RequestCompletion(new CompletionParams());
            var item = completionList.First();

            item = await client.ResolveCompletion(item);
            item.Detail.Should().Be("resolved");
        }

        [Fact]
        public async Task Should_Resolve_With_Data_CancellationToken()
        {
            var (client, _) = await Initialize(
                options => { }, options => {
                    options.OnCompletion(
                        (completionParams, token) => {
                            return Task.FromResult(
                                new CompletionList<Fixtures.Data>(
                                    new CompletionItem<Fixtures.Data> {
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
                        (completionItem, token) => {
                            completionItem.Data!.Id.Should().NotBeEmpty();
                            completionItem.Data!.Child.Should().NotBeNull();
                            completionItem.Data!.Name.Should().Be("name");
                            return Task.FromResult(completionItem with { Detail = "resolved" });
                        },
                        (_, _) => new CompletionRegistrationOptions()
                    );
                }
            );

            var items = await client.RequestCompletion(new CompletionParams());

            var item = items.Items.Single();

            item = await client.ResolveCompletion(item);
            item.Detail.Should().Be("resolved");
        }

        [Fact]
        public async Task Should_Resolve_With_Partial_Data_CancellationToken()
        {
            var (client, _) = await Initialize(
                options => { }, options => {
                    options.ObserveCompletion<Fixtures.Data>(
                        (completionParams, observer, token) => {
                            var a = new CompletionList<Fixtures.Data>(
                                new CompletionItem<Fixtures.Data> {
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
                        (completionItem, token) => {
                            completionItem.Data!.Id.Should().NotBeEmpty();
                            completionItem.Data!.Child.Should().NotBeNull();
                            completionItem.Data!.Name.Should().Be("name");
                            return Task.FromResult(completionItem with { Detail = "resolved" });
                        },
                        (_, _) => new CompletionRegistrationOptions()
                    );
                }
            );

            var item = await client.RequestCompletion(new CompletionParams()).SelectMany(z => z).Take(1).ToTask(CancellationToken);

            item = await client.ResolveCompletion(item);
            item.Detail.Should().Be("resolved");
        }

        [Fact]
        public async Task Should_Resolve_With_Data()
        {
            var (client, _) = await Initialize(
                options => { }, options => {
                    options.OnCompletion(
                        completionParams => {
                            return Task.FromResult(
                                new CompletionList<Fixtures.Data>(
                                    new CompletionItem<Data> {
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
                        completionItem => {
                            completionItem.Data!.Id.Should().NotBeEmpty();
                            completionItem.Data!.Child.Should().NotBeNull();
                            completionItem.Data!.Name.Should().Be("name");
                            return Task.FromResult(completionItem with { Detail = "resolved" });
                        },
                        (_, _) => new CompletionRegistrationOptions()
                    );
                }
            );

            var items = await client.RequestCompletion(new CompletionParams());

            var item = items.Items.Single();

            item = await client.ResolveCompletion(item);
            item.Detail.Should().Be("resolved");
        }

        [Fact]
        public async Task Should_Resolve_With_Partial_Data()
        {
            var (client, _) = await Initialize(
                options => { }, options => {
                    options.ObserveCompletion<Data>(
                        (completionParams, observer) => {
                            var a = new CompletionList<Data>(
                                new CompletionItem<Data> {
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
                        completionItem => {
                            completionItem.Data!.Id.Should().NotBeEmpty();
                            completionItem.Data!.Child.Should().NotBeNull();
                            completionItem.Data!.Name.Should().Be("name");
                            return Task.FromResult(completionItem with { Detail = "resolved" });
                        },
                        (_, _) => new CompletionRegistrationOptions()
                    );
                }
            );

            var item = await client.RequestCompletion(new CompletionParams()).SelectMany(z => z).Take(1).ToTask(CancellationToken);

            item = await client.ResolveCompletion(item);
            item.Detail.Should().Be("resolved");
        }


        [Fact]
        public async Task Should_Resolve_Capability()
        {
            var (client, _) = await Initialize(
                options => { }, options => {
                    options.OnCompletion(
                        (completionParams, capability, token) => {
                            return Task.FromResult(
                                new CompletionList(
                                    new CompletionItem {
                                        Command = new Command {
                                            Name = "execute-a",
                                            Arguments = JArray.FromObject(new object[] { 1, "2", false })
                                        }
                                    }
                                )
                            );
                        },
                        (completionItem, capability, token) => { return Task.FromResult(completionItem with { Detail = "resolved" }); },
                        (_, _) => new CompletionRegistrationOptions()
                    );
                }
            );

            var items = await client.RequestCompletion(new CompletionParams());

            var item = items.Items.Single();

            item = await client.ResolveCompletion(item);
            item.Detail.Should().Be("resolved");
        }

        [Fact]
        public async Task Should_Resolve_Partial_Capability()
        {
            var (client, _) = await Initialize(
                options => { }, options => {
                    options.ObserveCompletion(
                        (completionParams, observer, capability, token) => {
                            var a = new CompletionList(
                                new CompletionItem {
                                    Command = new Command {
                                        Name = "execute-a",
                                        Arguments = JArray.FromObject(new object[] { 1, "2", false })
                                    },
                                }
                            );

                            observer.OnNext(a);
                            observer.OnCompleted();
                        },
                        (completionItem, capability, token) => { return Task.FromResult(completionItem with { Detail = "resolved" }); },
                        (_, _) => new CompletionRegistrationOptions()
                    );
                }
            );

            var item = await client.RequestCompletion(new CompletionParams()).SelectMany(z => z).Take(1).ToTask(CancellationToken);

            item = await client.ResolveCompletion(item);
            item.Detail.Should().Be("resolved");
        }

        [Fact]
        public async Task Should_Resolve_CancellationToken()
        {
            var (client, _) = await Initialize(
                options => { }, options => {
                    options.OnCompletion(
                        (completionParams, token) => {
                            return Task.FromResult(
                                new CompletionList(
                                    new CompletionItem {
                                        Command = new Command {
                                            Name = "execute-a",
                                            Arguments = JArray.FromObject(new object[] { 1, "2", false })
                                        },
                                    }
                                )
                            );
                        },
                        (completionItem, token) => { return Task.FromResult(completionItem with { Detail = "resolved" }); },
                        (_, _) => new CompletionRegistrationOptions()
                    );
                }
            );

            var items = await client.RequestCompletion(new CompletionParams());

            var item = items.Items.Single();

            item = await client.ResolveCompletion(item);
            item.Detail.Should().Be("resolved");
        }

        [Fact]
        public async Task Should_Resolve_Partial_CancellationToken()
        {
            var (client, _) = await Initialize(
                options => { }, options => {
                    options.ObserveCompletion(
                        (completionParams, observer, token) => {
                            var a = new CompletionList(
                                new CompletionItem {
                                    Command = new Command {
                                        Name = "execute-a",
                                        Arguments = JArray.FromObject(new object[] { 1, "2", false })
                                    },
                                }
                            );

                            observer.OnNext(a);
                            observer.OnCompleted();
                        },
                        (completionItem, token) => { return Task.FromResult(completionItem with { Detail = "resolved" }); },
                        (_, _) => new CompletionRegistrationOptions()
                    );
                }
            );

            var item = await client.RequestCompletion(new CompletionParams()).SelectMany(z => z).Take(1).ToTask(CancellationToken);

            item = await client.ResolveCompletion(item);
            item.Detail.Should().Be("resolved");
        }

        [Fact]
        public async Task Should_Resolve()
        {
            var (client, _) = await Initialize(
                options => { }, options => {
                    options.OnCompletion(
                        completionParams => {
                            return Task.FromResult(
                                new CompletionList(
                                    new CompletionItem {
                                        Command = new Command {
                                            Name = "execute-a",
                                            Arguments = JArray.FromObject(new object[] { 1, "2", false })
                                        },
                                    }
                                )
                            );
                        },
                        completionItem => { return Task.FromResult(completionItem with { Detail = "resolved" }); },
                        (_, _) => new CompletionRegistrationOptions()
                    );
                }
            );

            var items = await client.RequestCompletion(new CompletionParams());

            var item = items.Items.Single();

            item = await client.ResolveCompletion(item);
            item.Detail.Should().Be("resolved");
        }

        [Fact]
        public async Task Should_Resolve_Partial()
        {
            var (client, _) = await Initialize(
                options => { }, options => {
                    options.ObserveCompletion(
                        (completionParams, observer) => {
                            var a = new CompletionList(
                                new CompletionItem {
                                    Command = new Command {
                                        Name = "execute-a",
                                        Arguments = JArray.FromObject(new object[] { 1, "2", false })
                                    },
                                }
                            );

                            observer.OnNext(a);
                            observer.OnCompleted();
                        },
                        completionItem => { return Task.FromResult(completionItem with { Detail = "resolved" }); },
                        (_, _) => new CompletionRegistrationOptions()
                    );
                }
            );

            var item = await client.RequestCompletion(new CompletionParams()).SelectMany(z => z).Take(1).ToTask(CancellationToken);

            item = await client.ResolveCompletion(item);
            item.Detail.Should().Be("resolved");
        }
    }
}
