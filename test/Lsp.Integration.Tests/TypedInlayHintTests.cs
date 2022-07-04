using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using FluentAssertions;
using Lsp.Integration.Tests.Fixtures;
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

namespace Lsp.Integration.Tests
{
    public class TypedInlayHintTests : LanguageProtocolTestBase
    {
        public TypedInlayHintTests(ITestOutputHelper outputHelper) : base(new JsonRpcTestOptions().ConfigureForXUnit(outputHelper, LogEventLevel.Verbose))
        {
        }

        [Fact]
        public async Task Should_Aggregate_With_All_Related_Handlers()
        {
            var (client, _) = await Initialize(
                options => { }, options =>
                {
                    var identifier = Substitute.For<ITextDocumentIdentifier>();
                    identifier.GetTextDocumentAttributes(Arg.Any<DocumentUri>()).Returns(
                        call => new TextDocumentAttributes(call.ArgAt<DocumentUri>(0), "file", "csharp")
                    );
                    options.AddTextDocumentIdentifier(identifier);

                    options.OnInlayHints(
                        inlayHintParams =>
                        {
                            return Task.FromResult<InlayHintContainer<Data>?>(
                                new InlayHintContainer<Data>(
                                    new InlayHint<Data>
                                    {
                                        Data = new Data
                                        {
                                            Child = new Nested
                                            {
                                                Date = DateTimeOffset.MinValue
                                            },
                                            Id = Guid.NewGuid(),
                                            Name = "name"
                                        }
                                    }
                                )
                            );
                        },
                        l =>
                        {
                            return Task.FromResult(
                                l with
                                {
                                    TextEdits = new List<TextEdit>()
                                    {
                                        new AnnotatedTextEdit()
                                        {
                                            Range = new Range(( 1, 1 ), ( 1, 3 )),
                                            AnnotationId = "id1",
                                            NewText = "MyText1"
                                        }
                                    },
                                }
                            );
                        },
                        (_, _) => new InlayHintRegistrationOptions
                        {
                            DocumentSelector = TextDocumentSelector.ForPattern("**/*.cs")
                        }
                    );

                    options.OnInlayHints(
                        inlayHintParams =>
                        {
                            return Task.FromResult<InlayHintContainer?>(
                                new InlayHintContainer<Nested>(
                                    new InlayHint<Nested>
                                    {
                                        Kind = InlayHintKind.Parameter,
                                        Label = new StringOrInlayHintLabelParts("my hint"),
                                        Position = ( 1, 2 ),
                                        PaddingLeft = true,
                                        PaddingRight = true,
                                        Data = new Nested
                                        {
                                            Date = DateTimeOffset.Now
                                        }
                                    }
                                )
                            );
                        },
                        l =>
                        {
                            return Task.FromResult(
                                l with
                                {
                                    TextEdits = new List<TextEdit>()
                                    {
                                        new TextEdit()
                                        {
                                            Range = new Range(( 1, 1 ), ( 1, 3 )),
                                            NewText = "MyText2"
                                        }
                                    },
                                }
                            );
                        },
                        (_, _) => new InlayHintRegistrationOptions
                        {
                            DocumentSelector = TextDocumentSelector.ForPattern("**/*.cs")
                        }
                    );

                    options.OnInlayHints(
                        inlayHintParams =>
                        {
                            return Task.FromResult<InlayHintContainer?>(
                                new InlayHintContainer(
                                    new InlayHint
                                    {
                                        Kind = InlayHintKind.Parameter,
                                        Label = "my hint",
                                        Position = ( 1, 2 ),
                                        PaddingLeft = true,
                                        PaddingRight = true,
                                    }
                                )
                            );
                        },
                        l =>
                        {
                            return Task.FromResult(
                                l with
                                {
                                    TextEdits = new List<TextEdit>()
                                    {
                                        new AnnotatedTextEdit()
                                        {
                                            Range = new Range(( 1, 1 ), ( 1, 3 )),
                                            AnnotationId = "id3",
                                            NewText = "MyText3"
                                        }
                                    },
                                }
                            );
                        },
                        (_, _) => new InlayHintRegistrationOptions
                        {
                            DocumentSelector = TextDocumentSelector.ForPattern("**/*.cs")
                        }
                    );

                    options.OnInlayHints(
                        inlayHintParams =>
                        {
                            return Task.FromResult<InlayHintContainer?>(
                                new InlayHintContainer(
                                    new InlayHint
                                    {
                                    }
                                )
                            );
                        },
                        l => { return Task.FromResult(l with
                        {
                            TextEdits = new List<TextEdit>()
                            {
                                new AnnotatedTextEdit()
                                {
                                    Range = new Range(( 1, 1 ), ( 1, 3 )),
                                    AnnotationId = "id4",
                                    NewText = "Not Found"
                                }
                            }
                        }); },
                        (_, _) => new InlayHintRegistrationOptions
                        {
                            DocumentSelector = TextDocumentSelector.ForLanguage("vb")
                        }
                    );
                }
            );

            var InlayHint = await client.RequestInlayHints(
                new InlayHintParams
                {
                    TextDocument = new TextDocumentIdentifier("/some/path/file.cs"),
                }
            );

            var lens = InlayHint.ToArray();

            var responses = (await Task.WhenAll(lens.Select(z => client.ResolveInlayHint(z)))).SelectMany(z => z.TextEdits ?? Array.Empty<TextEdit>()).ToArray();
            responses.Select(z => z.NewText).Should().Contain(new[] { "MyText1", "MyText2", "MyText3" });
            responses.Select(z => z.NewText).Should().NotContain("Not Found");
            lens.Length.Should().Be(3);
            responses.OfType<AnnotatedTextEdit>().Should().HaveCount(2);
        }

        [Fact]
        public async Task Should_Resolve_With_Data_Capability()
        {
            var (client, _) = await Initialize(
                options => { }, options =>
                {
                    options.OnInlayHints(
                        (InlayHintParams, capability, token) =>
                        {
                            return Task.FromResult<InlayHintContainer<Data>?>(
                                new InlayHintContainer<Data>(
                                    new InlayHint<Data>
                                    {
                                        Data = new Data
                                        {
                                            Child = new Nested
                                            {
                                                Date = DateTimeOffset.MinValue
                                            },
                                            Id = Guid.NewGuid(),
                                            Name = "name"
                                        }
                                    }
                                )
                            );
                        },
                        (lens, capability, token) =>
                        {
                            lens.Data.Id.Should().NotBeEmpty();
                            lens.Data.Child.Should().NotBeNull();
                            lens.Data.Name.Should().Be("name");
                            return Task.FromResult(lens with { Data = lens.Data with { Name = "Resolved"} });
                        },
                        (_, _) => new InlayHintRegistrationOptions()
                    );
                }
            );

            var items = await client.SendRequest(TextDocumentNames.InlayHint,  new InlayHintParams()).Returning<InlayHintContainer<Data>?>(CancellationToken);
            client.TextDocument.RequestInlayHints()
            var item = items.Single();

            item = await client.ResolveInlayHint(item);
            item.Data!.Name.Should().Be("resolved");
        }

        [Fact]
        public async Task Should_Resolve_With_Partial_Data_Capability()
        {
            var (client, _) = await Initialize(
                options => { }, options =>
                {
                    options.ObserveInlayHint<Data>(
                        (InlayHintParams, observer, capability, token) =>
                        {
                            var a = new InlayHintContainer<Data>(
                                new InlayHint<Data>
                                {
                                    Command = new Command
                                    {
                                        Name = "execute-a",
                                        Arguments = JArray.FromObject(new object[] { 1, "2", false })
                                    },
                                    Data = new Data
                                    {
                                        Child = new Nested
                                        {
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
                        (InlayHint, capability, token) =>
                        {
                            InlayHint.Data.Id.Should().NotBeEmpty();
                            InlayHint.Data.Child.Should().NotBeNull();
                            InlayHint.Data.Name.Should().Be("name");
                            return Task.FromResult(InlayHint with { Command = InlayHint.Command with { Name = "resolved" } });
                            return Task.FromResult(InlayHint);
                        },
                        (_, _) => new InlayHintRegistrationOptions()
                    );
                }
            );

            var item = await client.RequestInlayHint(new InlayHintParams()).SelectMany(z => z).Take(1).ToTask(CancellationToken);

            item = await client.ResolveInlayHint(item);
            item.Command!.Name.Should().Be("resolved");
        }

        [Fact]
        public async Task Should_Resolve_With_Data_CancellationToken()
        {
            var (client, _) = await Initialize(
                options => { }, options =>
                {
                    options.OnInlayHints(
                        (InlayHintParams, token) =>
                        {
                            return Task.FromResult(
                                new InlayHintContainer<Data>(
                                    new InlayHint<Data>
                                    {
                                        Command = new Command
                                        {
                                            Name = "execute-a",
                                            Arguments = JArray.FromObject(new object[] { 1, "2", false })
                                        },
                                        Data = new Data
                                        {
                                            Child = new Nested
                                            {
                                                Date = DateTimeOffset.MinValue
                                            },
                                            Id = Guid.NewGuid(),
                                            Name = "name"
                                        }
                                    }
                                )
                            );
                        },
                        (InlayHint, token) =>
                        {
                            InlayHint.Data.Id.Should().NotBeEmpty();
                            InlayHint.Data.Child.Should().NotBeNull();
                            InlayHint.Data.Name.Should().Be("name");
                            return Task.FromResult(InlayHint with { Command = InlayHint.Command with { Name = "resolved" } });
                            return Task.FromResult(InlayHint);
                        },
                        (_, _) => new InlayHintRegistrationOptions()
                    );
                }
            );

            var items = await client.RequestInlayHint(new InlayHintParams());

            var item = items.Single();

            item = await client.ResolveInlayHint(item);
            item.Command!.Name.Should().Be("resolved");
        }

        [Fact]
        public async Task Should_Resolve_With_Partial_Data_CancellationToken()
        {
            var (client, _) = await Initialize(
                options => { }, options =>
                {
                    options.ObserveInlayHint<Data>(
                        (InlayHintParams, observer, token) =>
                        {
                            var a = new InlayHintContainer<Data>(
                                new InlayHint<Data>
                                {
                                    Command = new Command
                                    {
                                        Name = "execute-a",
                                        Arguments = JArray.FromObject(new object[] { 1, "2", false })
                                    },
                                    Data = new Data
                                    {
                                        Child = new Nested
                                        {
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
                        (InlayHint, token) =>
                        {
                            InlayHint.Data.Id.Should().NotBeEmpty();
                            InlayHint.Data.Child.Should().NotBeNull();
                            InlayHint.Data.Name.Should().Be("name");
                            return Task.FromResult(InlayHint with { Command = InlayHint.Command with { Name = "resolved" } });
                            return Task.FromResult(InlayHint);
                        },
                        (_, _) => new InlayHintRegistrationOptions()
                    );
                }
            );

            var item = await client.RequestInlayHint(new InlayHintParams()).SelectMany(z => z).Take(1).ToTask(CancellationToken);

            item = await client.ResolveInlayHint(item);
            item.Command!.Name.Should().Be("resolved");
        }

        [Fact]
        public async Task Should_Resolve_With_Data()
        {
            var (client, _) = await Initialize(
                options => { }, options =>
                {
                    options.OnInlayHints(
                        inlayHintParams =>
                        {
                            return Task.FromResult(
                                new InlayHintContainer<Data>(
                                    new InlayHint<Data>
                                    {
                                        Command = new Command
                                        {
                                            Name = "execute-a",
                                            Arguments = JArray.FromObject(new object[] { 1, "2", false })
                                        },
                                        Data = new Data
                                        {
                                            Child = new Nested
                                            {
                                                Date = DateTimeOffset.MinValue
                                            },
                                            Id = Guid.NewGuid(),
                                            Name = "name"
                                        }
                                    }
                                )
                            );
                        },
                        InlayHint =>
                        {
                            InlayHint.Data.Id.Should().NotBeEmpty();
                            InlayHint.Data.Child.Should().NotBeNull();
                            InlayHint.Data.Name.Should().Be("name");
                            return Task.FromResult(InlayHint with { Command = InlayHint.Command with { Name = "resolved" } });
                            return Task.FromResult(InlayHint);
                        },
                        (_, _) => new InlayHintRegistrationOptions()
                    );
                }
            );

            var items = await client.RequestInlayHint(new InlayHintParams());

            var item = items.Single();

            item = await client.ResolveInlayHint(item);
            item.Command!.Name.Should().Be("resolved");
        }

        [Fact]
        public async Task Should_Resolve_With_Partial_Data()
        {
            var (client, _) = await Initialize(
                options => { }, options =>
                {
                    options.ObserveInlayHint<Data>(
                        (InlayHintParams, observer) =>
                        {
                            var a = new InlayHintContainer<Data>(
                                new InlayHint<Data>
                                {
                                    Command = new Command
                                    {
                                        Name = "execute-a",
                                        Arguments = JArray.FromObject(new object[] { 1, "2", false })
                                    },
                                    Data = new Data
                                    {
                                        Child = new Nested
                                        {
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
                        InlayHint =>
                        {
                            InlayHint.Data.Id.Should().NotBeEmpty();
                            InlayHint.Data.Child.Should().NotBeNull();
                            InlayHint.Data.Name.Should().Be("name");
                            return Task.FromResult(InlayHint with { Command = InlayHint.Command with { Name = "resolved" } });
                            return Task.FromResult(InlayHint);
                        },
                        (_, _) => new InlayHintRegistrationOptions()
                    );
                }
            );

            var item = await client.RequestInlayHint(new InlayHintParams()).SelectMany(z => z).Take(1).ToTask(CancellationToken);

            item = await client.ResolveInlayHint(item);
            item.Command!.Name.Should().Be("resolved");
        }


        [Fact]
        public async Task Should_Resolve_Capability()
        {
            var (client, _) = await Initialize(
                options => { }, options =>
                {
                    options.OnInlayHints(
                        (InlayHintParams, capability, token) =>
                        {
                            return Task.FromResult(
                                new InlayHintContainer(
                                    new InlayHint
                                    {
                                        Command = new Command
                                        {
                                            Name = "execute-a",
                                            Arguments = JArray.FromObject(new object[] { 1, "2", false })
                                        }
                                    }
                                )
                            );
                        },
                        (InlayHint, capability, token) =>
                        {
                            return Task.FromResult(InlayHint with { Command = InlayHint.Command with { Name = "resolved" } });
                            return Task.FromResult(InlayHint);
                        },
                        (_, _) => new InlayHintRegistrationOptions()
                    );
                }
            );

            var items = await client.RequestInlayHint(new InlayHintParams());

            var item = items.Single();

            item = await client.ResolveInlayHint(item);
            item.Command!.Name.Should().Be("resolved");
        }

        [Fact]
        public async Task Should_Resolve_Partial_Capability()
        {
            var (client, _) = await Initialize(
                options => { }, options =>
                {
                    options.ObserveInlayHint(
                        (InlayHintParams, observer, capability, token) =>
                        {
                            var a = new InlayHintContainer(
                                new InlayHint
                                {
                                    Command = new Command
                                    {
                                        Name = "execute-a",
                                        Arguments = JArray.FromObject(new object[] { 1, "2", false })
                                    },
                                }
                            );

                            observer.OnNext(a);
                            observer.OnCompleted();
                        },
                        (InlayHint, capability, token) =>
                        {
                            return Task.FromResult(InlayHint with { Command = InlayHint.Command with { Name = "resolved" } });
                            return Task.FromResult(InlayHint);
                        },
                        (_, _) => new InlayHintRegistrationOptions()
                    );
                }
            );

            var item = await client.RequestInlayHint(new InlayHintParams()).SelectMany(z => z).Take(1).ToTask(CancellationToken);

            item = await client.ResolveInlayHint(item);
            item.Command!.Name.Should().Be("resolved");
        }

        [Fact]
        public async Task Should_Resolve_CancellationToken()
        {
            var (client, _) = await Initialize(
                options => { }, options =>
                {
                    options.OnInlayHints(
                        (InlayHintParams, token) =>
                        {
                            return Task.FromResult(
                                new InlayHintContainer(
                                    new InlayHint
                                    {
                                        Command = new Command
                                        {
                                            Name = "execute-a",
                                            Arguments = JArray.FromObject(new object[] { 1, "2", false })
                                        },
                                    }
                                )
                            );
                        },
                        (InlayHint, token) =>
                        {
                            return Task.FromResult(InlayHint with { Command = InlayHint.Command with { Name = "resolved" } });
                            return Task.FromResult(InlayHint);
                        },
                        (_, _) => new InlayHintRegistrationOptions()
                    );
                }
            );

            var items = await client.RequestInlayHint(new InlayHintParams());

            var item = items.Single();

            item = await client.ResolveInlayHint(item);
            item.Command!.Name.Should().Be("resolved");
        }

        [Fact]
        public async Task Should_Resolve_Partial_CancellationToken()
        {
            var (client, _) = await Initialize(
                options => { }, options =>
                {
                    options.ObserveInlayHint(
                        (InlayHintParams, observer, token) =>
                        {
                            var a = new InlayHintContainer(
                                new InlayHint
                                {
                                    Command = new Command
                                    {
                                        Name = "execute-a",
                                        Arguments = JArray.FromObject(new object[] { 1, "2", false })
                                    },
                                }
                            );

                            observer.OnNext(a);
                            observer.OnCompleted();
                        },
                        (InlayHint, token) =>
                        {
                            return Task.FromResult(InlayHint with { Command = InlayHint.Command with { Name = "resolved" } });
                            return Task.FromResult(InlayHint);
                        },
                        (_, _) => new InlayHintRegistrationOptions()
                    );
                }
            );

            var item = await client.RequestInlayHint(new InlayHintParams()).SelectMany(z => z).Take(1).ToTask(CancellationToken);

            item = await client.ResolveInlayHint(item);
            item.Command!.Name.Should().Be("resolved");
        }

        [Fact]
        public async Task Should_Resolve()
        {
            var (client, _) = await Initialize(
                options => { }, options =>
                {
                    options.OnInlayHints(
                        inlayHintParams =>
                        {
                            return Task.FromResult(
                                new InlayHintContainer(
                                    new InlayHint
                                    {
                                        Command = new Command
                                        {
                                            Name = "execute-a",
                                            Arguments = JArray.FromObject(new object[] { 1, "2", false })
                                        },
                                    }
                                )
                            );
                        },
                        InlayHint =>
                        {
                            return Task.FromResult(InlayHint with { Command = InlayHint.Command with { Name = "resolved" } });
                            return Task.FromResult(InlayHint);
                        },
                        (_, _) => new InlayHintRegistrationOptions()
                    );
                }
            );

            var items = await client.RequestInlayHint(new InlayHintParams());

            var item = items.Single();

            item = await client.ResolveInlayHint(item);
            item.Command!.Name.Should().Be("resolved");
        }

        [Fact]
        public async Task Should_Resolve_Partial()
        {
            var (client, _) = await Initialize(
                options => { }, options =>
                {
                    options.ObserveInlayHint(
                        (InlayHintParams, observer) =>
                        {
                            var a = new InlayHintContainer(
                                new InlayHint
                                {
                                    Command = new Command
                                    {
                                        Name = "execute-a",
                                        Arguments = JArray.FromObject(new object[] { 1, "2", false })
                                    },
                                }
                            );

                            observer.OnNext(a);
                            observer.OnCompleted();
                        },
                        InlayHint =>
                        {
                            return Task.FromResult(InlayHint with { Command = InlayHint.Command with { Name = "resolved" } });
                            return Task.FromResult(InlayHint);
                        },
                        (_, _) => new InlayHintRegistrationOptions()
                    );
                }
            );

            var item = await client.RequestInlayHint(new InlayHintParams()).SelectMany(z => z).Take(1).ToTask(CancellationToken);

            item = await client.ResolveInlayHint(item);
            item.Command!.Name.Should().Be("resolved");
        }
    }
}
