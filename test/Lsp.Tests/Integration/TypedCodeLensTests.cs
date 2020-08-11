using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc.Testing;
using OmniSharp.Extensions.LanguageProtocol.Testing;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Serilog.Events;
using Xunit;
using Xunit.Abstractions;
using HandlerIdentity = OmniSharp.Extensions.LanguageServer.Protocol.Models.HandlerIdentity;

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
            var (client, server) = await Initialize(
                options => { }, options => {
                    var identifier = Substitute.For<ITextDocumentIdentifier>();
                    identifier.GetTextDocumentAttributes(Arg.Any<DocumentUri>()).Returns(
                        call => new TextDocumentAttributes(call.ArgAt<DocumentUri>(0), "file", "csharp")
                    );
                    options.AddTextDocumentIdentifier(identifier);

                    options.OnCodeLens(
                        codeLensParams => {
                            return Task.FromResult(
                                new CodeLensContainer<Data>(
                                    new CodeLens<Data> {
                                        Command = new Command {
                                            Name = "data-a",
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
                            codeLens.Command.Name = "resolved-a";
                            return Task.FromResult(codeLens);
                        },
                        new CodeLensRegistrationOptions {
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
                        codeLens => {
                            codeLens.Command.Name = "resolved-b";
                            return Task.FromResult(codeLens);
                        },
                        new CodeLensRegistrationOptions {
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
                        codeLens => {
                            codeLens.Command.Name = "resolved-c";
                            return Task.FromResult(codeLens);
                        },
                        new CodeLensRegistrationOptions {
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
                        codeLens => {
                            codeLens.Command.Name = "resolved-d";
                            return Task.FromResult(codeLens);
                        },
                        new CodeLensRegistrationOptions {
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
            responses.Select(z => z.Command.Name).Should().Contain(new[] { "resolved-a", "resolved-b", "resolved-c" });
            responses.Select(z => z.Command.Name).Should().NotContain("resolved-d");
            lens.Length.Should().Be(3);
        }

        [Fact]
        public async Task Should_Resolve_With_Data_Capability()
        {
            var (client, server) = await Initialize(
                options => { }, options => {
                    options.OnCodeLens(
                        (codeLensParams, capability, token) => {
                            return Task.FromResult(
                                new CodeLensContainer<Data>(
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
                        (codeLens, capability, token) => {
                            codeLens.Data.Id.Should().NotBeEmpty();
                            codeLens.Data.Child.Should().NotBeNull();
                            codeLens.Data.Name.Should().Be("name");
                            codeLens.Command.Name = "resolved";
                            return Task.FromResult(codeLens);
                        },
                        new CodeLensRegistrationOptions()
                    );
                }
            );

            var items = await client.RequestCodeLens(new CodeLensParams());

            var item = items.Single();

            item = await client.ResolveCodeLens(item);
            item.Command.Name.Should().Be("resolved");
        }

        [Fact]
        public async Task Should_Resolve_With_Partial_Data_Capability()
        {
            var (client, server) = await Initialize(
                options => { }, options => {
                    options.OnCodeLens<Data>(
                        (codeLensParams, observer, capability, token) => {
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
                        (codeLens, capability, token) => {
                            codeLens.Data.Id.Should().NotBeEmpty();
                            codeLens.Data.Child.Should().NotBeNull();
                            codeLens.Data.Name.Should().Be("name");
                            codeLens.Command.Name = "resolved";
                            return Task.FromResult(codeLens);
                        },
                        new CodeLensRegistrationOptions()
                    );
                }
            );

            var item = await client.RequestCodeLens(new CodeLensParams()).SelectMany(z => z).Take(1).ToTask(CancellationToken);

            item = await client.ResolveCodeLens(item);
            item.Command.Name.Should().Be("resolved");
        }

        [Fact]
        public async Task Should_Resolve_With_Data_CancellationToken()
        {
            var (client, server) = await Initialize(
                options => { }, options => {
                    options.OnCodeLens(
                        (codeLensParams, token) => {
                            return Task.FromResult(
                                new CodeLensContainer<Data>(
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
                        (codeLens, token) => {
                            codeLens.Data.Id.Should().NotBeEmpty();
                            codeLens.Data.Child.Should().NotBeNull();
                            codeLens.Data.Name.Should().Be("name");
                            codeLens.Command.Name = "resolved";
                            return Task.FromResult(codeLens);
                        },
                        new CodeLensRegistrationOptions()
                    );
                }
            );

            var items = await client.RequestCodeLens(new CodeLensParams());

            var item = items.Single();

            item = await client.ResolveCodeLens(item);
            item.Command.Name.Should().Be("resolved");
        }

        [Fact]
        public async Task Should_Resolve_With_Partial_Data_CancellationToken()
        {
            var (client, server) = await Initialize(
                options => { }, options => {
                    options.OnCodeLens<Data>(
                        (codeLensParams, observer, token) => {
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
                        (codeLens, token) => {
                            codeLens.Data.Id.Should().NotBeEmpty();
                            codeLens.Data.Child.Should().NotBeNull();
                            codeLens.Data.Name.Should().Be("name");
                            codeLens.Command.Name = "resolved";
                            return Task.FromResult(codeLens);
                        },
                        new CodeLensRegistrationOptions()
                    );
                }
            );

            var item = await client.RequestCodeLens(new CodeLensParams()).SelectMany(z => z).Take(1).ToTask(CancellationToken);

            item = await client.ResolveCodeLens(item);
            item.Command.Name.Should().Be("resolved");
        }

        [Fact]
        public async Task Should_Resolve_With_Data()
        {
            var (client, server) = await Initialize(
                options => { }, options => {
                    options.OnCodeLens(
                        codeLensParams => {
                            return Task.FromResult(
                                new CodeLensContainer<Data>(
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
                            codeLens.Command.Name = "resolved";
                            return Task.FromResult(codeLens);
                        },
                        new CodeLensRegistrationOptions()
                    );
                }
            );

            var items = await client.RequestCodeLens(new CodeLensParams());

            var item = items.Single();

            item = await client.ResolveCodeLens(item);
            item.Command.Name.Should().Be("resolved");
        }

        [Fact]
        public async Task Should_Resolve_With_Partial_Data()
        {
            var (client, server) = await Initialize(
                options => { }, options => {
                    options.OnCodeLens<Data>(
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
                            codeLens.Command.Name = "resolved";
                            return Task.FromResult(codeLens);
                        },
                        new CodeLensRegistrationOptions()
                    );
                }
            );

            var item = await client.RequestCodeLens(new CodeLensParams()).SelectMany(z => z).Take(1).ToTask(CancellationToken);

            item = await client.ResolveCodeLens(item);
            item.Command.Name.Should().Be("resolved");
        }


        [Fact]
        public async Task Should_Resolve_Capability()
        {
            var (client, server) = await Initialize(
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
                            codeLens.Command.Name = "resolved";
                            return Task.FromResult(codeLens);
                        },
                        new CodeLensRegistrationOptions()
                    );
                }
            );

            var items = await client.RequestCodeLens(new CodeLensParams());

            var item = items.Single();

            item = await client.ResolveCodeLens(item);
            item.Command.Name.Should().Be("resolved");
        }

        [Fact]
        public async Task Should_Resolve_Partial_Capability()
        {
            var (client, server) = await Initialize(
                options => { }, options => {
                    options.OnCodeLens(
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
                            codeLens.Command.Name = "resolved";
                            return Task.FromResult(codeLens);
                        },
                        new CodeLensRegistrationOptions()
                    );
                }
            );

            var item = await client.RequestCodeLens(new CodeLensParams()).SelectMany(z => z).Take(1).ToTask(CancellationToken);

            item = await client.ResolveCodeLens(item);
            item.Command.Name.Should().Be("resolved");
        }

        [Fact]
        public async Task Should_Resolve_CancellationToken()
        {
            var (client, server) = await Initialize(
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
                            codeLens.Command.Name = "resolved";
                            return Task.FromResult(codeLens);
                        },
                        new CodeLensRegistrationOptions()
                    );
                }
            );

            var items = await client.RequestCodeLens(new CodeLensParams());

            var item = items.Single();

            item = await client.ResolveCodeLens(item);
            item.Command.Name.Should().Be("resolved");
        }

        [Fact]
        public async Task Should_Resolve_Partial_CancellationToken()
        {
            var (client, server) = await Initialize(
                options => { }, options => {
                    options.OnCodeLens(
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
                            codeLens.Command.Name = "resolved";
                            return Task.FromResult(codeLens);
                        },
                        new CodeLensRegistrationOptions()
                    );
                }
            );

            var item = await client.RequestCodeLens(new CodeLensParams()).SelectMany(z => z).Take(1).ToTask(CancellationToken);

            item = await client.ResolveCodeLens(item);
            item.Command.Name.Should().Be("resolved");
        }

        [Fact]
        public async Task Should_Resolve()
        {
            var (client, server) = await Initialize(
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
                            codeLens.Command.Name = "resolved";
                            return Task.FromResult(codeLens);
                        },
                        new CodeLensRegistrationOptions()
                    );
                }
            );

            var items = await client.RequestCodeLens(new CodeLensParams());

            var item = items.Single();

            item = await client.ResolveCodeLens(item);
            item.Command.Name.Should().Be("resolved");
        }

        [Fact]
        public async Task Should_Resolve_Partial()
        {
            var (client, server) = await Initialize(
                options => { }, options => {
                    options.OnCodeLens(
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
                            codeLens.Command.Name = "resolved";
                            return Task.FromResult(codeLens);
                        },
                        new CodeLensRegistrationOptions()
                    );
                }
            );

            var item = await client.RequestCodeLens(new CodeLensParams()).SelectMany(z => z).Take(1).ToTask(CancellationToken);

            item = await client.ResolveCodeLens(item);
            item.Command.Name.Should().Be("resolved");
        }

        private class Data : HandlerIdentity
        {
            public string Name { get; set; }
            public Guid Id { get; set; }
            public Nested Child { get; set; }
        }

        private class Nested : HandlerIdentity
        {
            public DateTimeOffset Date { get; set; }
        }
    }
}
