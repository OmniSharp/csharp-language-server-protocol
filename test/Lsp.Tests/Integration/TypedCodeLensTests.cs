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
    public class TypedCodeLensTests : LanguageProtocolTestBase
    {
        public TypedCodeLensTests(ITestOutputHelper outputHelper) : base(new JsonRpcTestOptions().ConfigureForXUnit(outputHelper, LogEventLevel.Verbose))
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

                    options.OnCodeLens(
                        codeLensParams => {
                            return Task.FromResult(
                                new CodeLensContainer<Fixtures.Data>(
                                    new CodeLens<Fixtures.Data> {
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
                        l => { return Task.FromResult(l with { Command = l.Command with { Name = "resolved-a" } }); },
                        (_, _) => new CodeLensRegistrationOptions {
                            DocumentSelector = DocumentSelector.ForPattern("**/*.cs")
                        }
                    );

                    options.OnCodeLens(
                        codeLensParams => {
                            return Task.FromResult(
                                new CodeLensContainer<Nested>(
                                    new CodeLens<Nested> {
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
                        l => { return Task.FromResult(l with { Command = l.Command with { Name = "resolved-b" } }); },
                        (_, _) => new CodeLensRegistrationOptions {
                            DocumentSelector = DocumentSelector.ForPattern("**/*.cs")
                        }
                    );

                    options.OnCodeLens(
                        codeLensParams => {
                            return Task.FromResult(
                                new CodeLensContainer(
                                    new CodeLens {
                                        Command = new Command {
                                            Name = "no-data-c",
                                            Arguments = JArray.FromObject(new object[] { 1, "2", false })
                                        }
                                    }
                                )
                            );
                        },
                        l => { return Task.FromResult(l with { Command = l.Command with { Name = "resolved-c" } }); },
                        (_, _) => new CodeLensRegistrationOptions {
                            DocumentSelector = DocumentSelector.ForPattern("**/*.cs")
                        }
                    );

                    options.OnCodeLens(
                        codeLensParams => {
                            return Task.FromResult(
                                new CodeLensContainer(
                                    new CodeLens {
                                        Command = new Command {
                                            Name = "not-included",
                                            Arguments = JArray.FromObject(new object[] { 1, "2", false })
                                        }
                                    }
                                )
                            );
                        },
                        l => { return Task.FromResult(l with { Command = l.Command with { Name = "resolved-d" } }); },
                        (_, _) => new CodeLensRegistrationOptions {
                            DocumentSelector = DocumentSelector.ForLanguage("vb")
                        }
                    );
                }
            );

            var codeLens = await client.RequestCodeLens(
                new CodeLensParams {
                    TextDocument = new TextDocumentIdentifier("/some/path/file.cs"),
                }
            );

            var lens = codeLens.ToArray();

            var responses = await Task.WhenAll(lens.Select(z => client.ResolveCodeLens(z)));
            responses.Select(z => z.Command!.Name).Should().Contain(new[] { "resolved-a", "resolved-b", "resolved-c" });
            responses.Select(z => z.Command!.Name).Should().NotContain("resolved-d");
            lens.Length.Should().Be(3);
        }

        [Fact]
        public async Task Should_Resolve_With_Data_Capability()
        {
            var (client, _) = await Initialize(
                options => { }, options => {
                    options.OnCodeLens(
                        (codeLensParams, capability, token) => {
                            return Task.FromResult(
                                new CodeLensContainer<Fixtures.Data>(
                                    new CodeLens<Fixtures.Data> {
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
                        (lens, capability, token) => {
                            lens.Data.Id.Should().NotBeEmpty();
                            lens.Data.Child.Should().NotBeNull();
                            lens.Data.Name.Should().Be("name");
                            return Task.FromResult(lens with { Command = lens.Command with { Name = "resolved" } });
                            return Task.FromResult(lens);
                        },
                        (_, _) => new CodeLensRegistrationOptions()
                    );
                }
            );

            var items = await client.RequestCodeLens(new CodeLensParams());

            var item = items.Single();

            item = await client.ResolveCodeLens(item);
            item.Command!.Name.Should().Be("resolved");
        }

        [RetryFact]
        public async Task Should_Resolve_With_Partial_Data_Capability()
        {
            var (client, _) = await Initialize(
                options => { }, options => {
                    options.ObserveCodeLens<Fixtures.Data>(
                        (codeLensParams, observer, capability, token) => {
                            var a = new CodeLensContainer<Fixtures.Data>(
                                new CodeLens<Fixtures.Data> {
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
                        (codeLens, capability, token) => {
                            codeLens.Data.Id.Should().NotBeEmpty();
                            codeLens.Data.Child.Should().NotBeNull();
                            codeLens.Data.Name.Should().Be("name");
                            return Task.FromResult(codeLens with { Command = codeLens.Command with { Name = "resolved" } });
                            return Task.FromResult(codeLens);
                        },
                        (_, _) => new CodeLensRegistrationOptions()
                    );
                }
            );

            var item = await client.RequestCodeLens(new CodeLensParams()).SelectMany(z => z).Take(1).ToTask(CancellationToken);

            item = await client.ResolveCodeLens(item);
            item.Command!.Name.Should().Be("resolved");
        }

        [Fact]
        public async Task Should_Resolve_With_Data_CancellationToken()
        {
            var (client, _) = await Initialize(
                options => { }, options => {
                    options.OnCodeLens(
                        (codeLensParams, token) => {
                            return Task.FromResult(
                                new CodeLensContainer<Fixtures.Data>(
                                    new CodeLens<Fixtures.Data> {
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
                        (codeLens, token) => {
                            codeLens.Data.Id.Should().NotBeEmpty();
                            codeLens.Data.Child.Should().NotBeNull();
                            codeLens.Data.Name.Should().Be("name");
                            return Task.FromResult(codeLens with { Command = codeLens.Command with { Name = "resolved" } });
                            return Task.FromResult(codeLens);
                        },
                        (_, _) => new CodeLensRegistrationOptions()
                    );
                }
            );

            var items = await client.RequestCodeLens(new CodeLensParams());

            var item = items.Single();

            item = await client.ResolveCodeLens(item);
            item.Command!.Name.Should().Be("resolved");
        }

        [RetryFact]
        public async Task Should_Resolve_With_Partial_Data_CancellationToken()
        {
            var (client, _) = await Initialize(
                options => { }, options => {
                    options.ObserveCodeLens<Fixtures.Data>(
                        (codeLensParams, observer, token) => {
                            var a = new CodeLensContainer<Fixtures.Data>(
                                new CodeLens<Fixtures.Data> {
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
                        (codeLens, token) => {
                            codeLens.Data.Id.Should().NotBeEmpty();
                            codeLens.Data.Child.Should().NotBeNull();
                            codeLens.Data.Name.Should().Be("name");
                            return Task.FromResult(codeLens with { Command = codeLens.Command with { Name = "resolved" } });
                            return Task.FromResult(codeLens);
                        },
                        (_, _) => new CodeLensRegistrationOptions()
                    );
                }
            );

            var item = await client.RequestCodeLens(new CodeLensParams()).SelectMany(z => z).Take(1).ToTask(CancellationToken);

            item = await client.ResolveCodeLens(item);
            item.Command!.Name.Should().Be("resolved");
        }

        [Fact]
        public async Task Should_Resolve_With_Data()
        {
            var (client, _) = await Initialize(
                options => { }, options => {
                    options.OnCodeLens(
                        codeLensParams => {
                            return Task.FromResult(
                                new CodeLensContainer<Fixtures.Data>(
                                    new CodeLens<Data> {
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
                        codeLens => {
                            codeLens.Data.Id.Should().NotBeEmpty();
                            codeLens.Data.Child.Should().NotBeNull();
                            codeLens.Data.Name.Should().Be("name");
                            return Task.FromResult(codeLens with { Command = codeLens.Command with { Name = "resolved" } });
                            return Task.FromResult(codeLens);
                        },
                        (_, _) => new CodeLensRegistrationOptions()
                    );
                }
            );

            var items = await client.RequestCodeLens(new CodeLensParams());

            var item = items.Single();

            item = await client.ResolveCodeLens(item);
            item.Command!.Name.Should().Be("resolved");
        }

        [RetryFact]
        public async Task Should_Resolve_With_Partial_Data()
        {
            var (client, _) = await Initialize(
                options => { }, options => {
                    options.ObserveCodeLens<Data>(
                        (codeLensParams, observer) => {
                            var a = new CodeLensContainer<Data>(
                                new CodeLens<Data> {
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
                        codeLens => {
                            codeLens.Data.Id.Should().NotBeEmpty();
                            codeLens.Data.Child.Should().NotBeNull();
                            codeLens.Data.Name.Should().Be("name");
                            return Task.FromResult(codeLens with { Command = codeLens.Command with { Name = "resolved" } });
                            return Task.FromResult(codeLens);
                        },
                        (_, _) => new CodeLensRegistrationOptions()
                    );
                }
            );

            var item = await client.RequestCodeLens(new CodeLensParams()).SelectMany(z => z).Take(1).ToTask(CancellationToken);

            item = await client.ResolveCodeLens(item);
            item.Command!.Name.Should().Be("resolved");
        }


        [Fact]
        public async Task Should_Resolve_Capability()
        {
            var (client, _) = await Initialize(
                options => { }, options => {
                    options.OnCodeLens(
                        (codeLensParams, capability, token) => {
                            return Task.FromResult(
                                new CodeLensContainer(
                                    new CodeLens {
                                        Command = new Command {
                                            Name = "execute-a",
                                            Arguments = JArray.FromObject(new object[] { 1, "2", false })
                                        }
                                    }
                                )
                            );
                        },
                        (codeLens, capability, token) => {
                            return Task.FromResult(codeLens with { Command = codeLens.Command with { Name = "resolved" } });
                            return Task.FromResult(codeLens);
                        },
                        (_, _) => new CodeLensRegistrationOptions()
                    );
                }
            );

            var items = await client.RequestCodeLens(new CodeLensParams());

            var item = items.Single();

            item = await client.ResolveCodeLens(item);
            item.Command!.Name.Should().Be("resolved");
        }

        [RetryFact]
        public async Task Should_Resolve_Partial_Capability()
        {
            var (client, _) = await Initialize(
                options => { }, options => {
                    options.ObserveCodeLens(
                        (codeLensParams, observer, capability, token) => {
                            var a = new CodeLensContainer(
                                new CodeLens {
                                    Command = new Command {
                                        Name = "execute-a",
                                        Arguments = JArray.FromObject(new object[] { 1, "2", false })
                                    },
                                }
                            );

                            observer.OnNext(a);
                            observer.OnCompleted();
                        },
                        (codeLens, capability, token) => {
                            return Task.FromResult(codeLens with { Command = codeLens.Command with { Name = "resolved" } });
                            return Task.FromResult(codeLens);
                        },
                        (_, _) => new CodeLensRegistrationOptions()
                    );
                }
            );

            var item = await client.RequestCodeLens(new CodeLensParams()).SelectMany(z => z).Take(1).ToTask(CancellationToken);

            item = await client.ResolveCodeLens(item);
            item.Command!.Name.Should().Be("resolved");
        }

        [Fact]
        public async Task Should_Resolve_CancellationToken()
        {
            var (client, _) = await Initialize(
                options => { }, options => {
                    options.OnCodeLens(
                        (codeLensParams, token) => {
                            return Task.FromResult(
                                new CodeLensContainer(
                                    new CodeLens {
                                        Command = new Command {
                                            Name = "execute-a",
                                            Arguments = JArray.FromObject(new object[] { 1, "2", false })
                                        },
                                    }
                                )
                            );
                        },
                        (codeLens, token) => {
                            return Task.FromResult(codeLens with { Command = codeLens.Command with { Name = "resolved" } });
                            return Task.FromResult(codeLens);
                        },
                        (_, _) => new CodeLensRegistrationOptions()
                    );
                }
            );

            var items = await client.RequestCodeLens(new CodeLensParams());

            var item = items.Single();

            item = await client.ResolveCodeLens(item);
            item.Command!.Name.Should().Be("resolved");
        }

        [RetryFact]
        public async Task Should_Resolve_Partial_CancellationToken()
        {
            var (client, _) = await Initialize(
                options => { }, options => {
                    options.ObserveCodeLens(
                        (codeLensParams, observer, token) => {
                            var a = new CodeLensContainer(
                                new CodeLens {
                                    Command = new Command {
                                        Name = "execute-a",
                                        Arguments = JArray.FromObject(new object[] { 1, "2", false })
                                    },
                                }
                            );

                            observer.OnNext(a);
                            observer.OnCompleted();
                        },
                        (codeLens, token) => {
                            return Task.FromResult(codeLens with { Command = codeLens.Command with { Name = "resolved" } });
                            return Task.FromResult(codeLens);
                        },
                        (_, _) => new CodeLensRegistrationOptions()
                    );
                }
            );

            var item = await client.RequestCodeLens(new CodeLensParams()).SelectMany(z => z).Take(1).ToTask(CancellationToken);

            item = await client.ResolveCodeLens(item);
            item.Command!.Name.Should().Be("resolved");
        }

        [Fact]
        public async Task Should_Resolve()
        {
            var (client, _) = await Initialize(
                options => { }, options => {
                    options.OnCodeLens(
                        codeLensParams => {
                            return Task.FromResult(
                                new CodeLensContainer(
                                    new CodeLens {
                                        Command = new Command {
                                            Name = "execute-a",
                                            Arguments = JArray.FromObject(new object[] { 1, "2", false })
                                        },
                                    }
                                )
                            );
                        },
                        codeLens => {
                            return Task.FromResult(codeLens with { Command = codeLens.Command with { Name = "resolved" } });
                            return Task.FromResult(codeLens);
                        },
                        (_, _) => new CodeLensRegistrationOptions()
                    );
                }
            );

            var items = await client.RequestCodeLens(new CodeLensParams());

            var item = items.Single();

            item = await client.ResolveCodeLens(item);
            item.Command!.Name.Should().Be("resolved");
        }

        [RetryFact]
        public async Task Should_Resolve_Partial()
        {
            var (client, _) = await Initialize(
                options => { }, options => {
                    options.ObserveCodeLens(
                        (codeLensParams, observer) => {
                            var a = new CodeLensContainer(
                                new CodeLens {
                                    Command = new Command {
                                        Name = "execute-a",
                                        Arguments = JArray.FromObject(new object[] { 1, "2", false })
                                    },
                                }
                            );

                            observer.OnNext(a);
                            observer.OnCompleted();
                        },
                        codeLens => {
                            return Task.FromResult(codeLens with { Command = codeLens.Command with { Name = "resolved" } });
                            return Task.FromResult(codeLens);
                        },
                        (_, _) => new CodeLensRegistrationOptions()
                    );
                }
            );

            var item = await client.RequestCodeLens(new CodeLensParams()).SelectMany(z => z).Take(1).ToTask(CancellationToken);

            item = await client.ResolveCodeLens(item);
            item.Command!.Name.Should().Be("resolved");
        }
    }
}
