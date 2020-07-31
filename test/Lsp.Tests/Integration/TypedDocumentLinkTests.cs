using System;
using System.Collections.Generic;
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

namespace Lsp.Tests.Integration
{
    public class TypedDocumentLinkTests : LanguageProtocolTestBase
    {

        public TypedDocumentLinkTests(ITestOutputHelper outputHelper) : base(new JsonRpcTestOptions().ConfigureForXUnit(outputHelper, LogEventLevel.Verbose))
        {
        }

        [Fact]
        public async Task Should_Aggregate_With_All_Related_Handlers()
        {

            var (client, server) = await Initialize(
                options => { }, options => {
                    var identifier = Substitute.For<ITextDocumentIdentifier>();
                    identifier.GetTextDocumentAttributes(Arg.Any<DocumentUri>()).Returns(call => new List<TextDocumentAttributes>() {
                        new TextDocumentAttributes(call.ArgAt<DocumentUri>(0), "file", "csharp")
                    });
                    options.AddTextDocumentIdentifier(identifier);

                    options.OnDocumentLink<Data>((codeLensParams) => {
                            return Task.FromResult(new DocumentLinkContainer<Data>(new DocumentLink<Data>() {
                                Tooltip = "data-a",
                                Data = new Data() {
                                    Child = new Nested() {
                                        Date = DateTimeOffset.MinValue
                                    },
                                    Id = Guid.NewGuid(),
                                    Name = "name"
                                }
                            }));
                        },
                        (codeLens) => {
                            codeLens.Tooltip = "resolved-a";
                            return Task.FromResult(codeLens);
                        },

                        new DocumentLinkRegistrationOptions() {
                            DocumentSelector = DocumentSelector.ForPattern("**/*.cs")
                        });

                    options.OnDocumentLink<Nested>((codeLensParams) => {
                            return Task.FromResult(new DocumentLinkContainer<Nested>(new DocumentLink<Nested>() {
                                Tooltip =  "nested-b",
                                Data = new Nested() {
                                    Date = DateTimeOffset.Now
                                }
                            }));
                        },
                        (codeLens) => {
                            codeLens.Tooltip = "resolved-b";
                            return Task.FromResult(codeLens);
                        },

                        new DocumentLinkRegistrationOptions() {
                            DocumentSelector = DocumentSelector.ForPattern("**/*.cs")
                        });

                    options.OnDocumentLink((codeLensParams) => {
                            return Task.FromResult(new DocumentLinkContainer(new DocumentLink() {
                                Tooltip = "no-data-c",
                            }));
                        },
                        (codeLens) => {
                            codeLens.Tooltip = "resolved-c";
                            return Task.FromResult(codeLens);
                        },

                        new DocumentLinkRegistrationOptions() {
                            DocumentSelector = DocumentSelector.ForPattern("**/*.cs")
                        });

                    options.OnDocumentLink((codeLensParams) => {
                            return Task.FromResult(new DocumentLinkContainer(new DocumentLink() {
                                Tooltip =  "not-included",
                            }));
                        },
                        (codeLens) => {
                            codeLens.Tooltip = "resolved-d";
                            return Task.FromResult(codeLens);
                        },

                        new DocumentLinkRegistrationOptions() {
                            DocumentSelector = DocumentSelector.ForLanguage("vb")
                        });
                });

            var codeLens = await client.RequestDocumentLink(new DocumentLinkParams() {
                TextDocument = new TextDocumentIdentifier("/some/path/file.cs"),
            });

            var lens = codeLens.ToArray();

            var responses = await Task.WhenAll(lens.Select(z => client.ResolveDocumentLink(z)));
            responses.Select(z => z.Tooltip).Should().Contain("resolved-a", "resolved-b", "resolved-c");
            responses.Select(z => z.Tooltip).Should().NotContain("resolved-d");
            lens.Length.Should().Be(3);
        }

        [Fact]
        public async Task Should_Resolve_With_Data_Capability()
        {

            var (client, server) = await Initialize(
                options => { }, options => {
                    options.OnDocumentLink((documentLinkParams, capability, token) => {
                        return Task.FromResult(new DocumentLinkContainer<Data>(new DocumentLink<Data>() {
                            Tooltip = "execute-a",
                            Data = new Data() {
                                Child = new Nested() {
                                    Date = DateTimeOffset.MinValue
                                },
                                Id = Guid.NewGuid(),
                                Name = "name"
                            }
                        }));
                    },
                        (documentLink, capability, token) => {
                            documentLink.Data.Id.Should().NotBeEmpty();
                            documentLink.Data.Child.Should().NotBeNull();
                            documentLink.Data.Name.Should().Be("name");
                            documentLink.Tooltip = "resolved";
                            return Task.FromResult(documentLink);
                        },

                        new DocumentLinkRegistrationOptions() {
                        });
                });

            var items = await client.RequestDocumentLink(new DocumentLinkParams());

            var item = items.Single();

            item = await client.ResolveDocumentLink(item);
            item.Tooltip.Should().Be("resolved");
        }

        [Fact]
        public async Task Should_Resolve_With_Partial_Data_Capability()
        {

            var (client, server) = await Initialize(
                options => { }, options => {
                    options.OnDocumentLink<Data>((documentLinkParams, observer, capability, token) => {
                        var a = new DocumentLinkContainer<Data>(new DocumentLink<Data>() {
                            Tooltip = "execute-a",
                            Data = new Data() {
                                Child = new Nested() {
                                    Date = DateTimeOffset.MinValue
                                },
                                Id = Guid.NewGuid(),
                                Name = "name"
                            }
                        });

                        observer.OnNext(a);
                        observer.OnCompleted();
                    },
                        (documentLink, capability, token) => {
                            documentLink.Data.Id.Should().NotBeEmpty();
                            documentLink.Data.Child.Should().NotBeNull();
                            documentLink.Data.Name.Should().Be("name");
                            documentLink.Tooltip = "resolved";
                            return Task.FromResult(documentLink);
                        },

                        new DocumentLinkRegistrationOptions() {
                        });
                });

            var item = await client.RequestDocumentLink(new DocumentLinkParams()).SelectMany(z => z).Take(1).ToTask(CancellationToken);

            item = await client.ResolveDocumentLink(item);
            item.Tooltip.Should().Be("resolved");
        }

        [Fact]
        public async Task Should_Resolve_With_Data_CancellationToken()
        {

            var (client, server) = await Initialize(
                options => { }, options => {
                    options.OnDocumentLink((documentLinkParams, token) => {
                        return Task.FromResult(new DocumentLinkContainer<Data>(new DocumentLink<Data>() {
                            Tooltip = "execute-a",
                            Data = new Data() {
                                Child = new Nested() {
                                    Date = DateTimeOffset.MinValue
                                },
                                Id = Guid.NewGuid(),
                                Name = "name"
                            }
                        }));
                    },
                        (documentLink, token) => {
                            documentLink.Data.Id.Should().NotBeEmpty();
                            documentLink.Data.Child.Should().NotBeNull();
                            documentLink.Data.Name.Should().Be("name");
                            documentLink.Tooltip = "resolved";
                            return Task.FromResult(documentLink);
                        },

                        new DocumentLinkRegistrationOptions() {
                        });
                });

            var items = await client.RequestDocumentLink(new DocumentLinkParams());

            var item = items.Single();

            item = await client.ResolveDocumentLink(item);
            item.Tooltip.Should().Be("resolved");
        }

        [Fact]
        public async Task Should_Resolve_With_Partial_Data_CancellationToken()
        {

            var (client, server) = await Initialize(
                options => { }, options => {
                    options.OnDocumentLink<Data>((documentLinkParams, observer, token) => {
                        var a = new DocumentLinkContainer<Data>(new DocumentLink<Data>() {
                            Tooltip = "execute-a",
                            Data = new Data() {
                                Child = new Nested() {
                                    Date = DateTimeOffset.MinValue
                                },
                                Id = Guid.NewGuid(),
                                Name = "name"
                            }
                        });

                        observer.OnNext(a);
                        observer.OnCompleted();
                    },
                        (documentLink, token) => {
                            documentLink.Data.Id.Should().NotBeEmpty();
                            documentLink.Data.Child.Should().NotBeNull();
                            documentLink.Data.Name.Should().Be("name");
                            documentLink.Tooltip = "resolved";
                            return Task.FromResult(documentLink);
                        },

                        new DocumentLinkRegistrationOptions() {
                        });
                });

            var item = await client.RequestDocumentLink(new DocumentLinkParams()).SelectMany(z => z).Take(1).ToTask(CancellationToken);

            item = await client.ResolveDocumentLink(item);
            item.Tooltip.Should().Be("resolved");
        }

        [Fact]
        public async Task Should_Resolve_With_Data()
        {

            var (client, server) = await Initialize(
                options => { }, options => {
                    options.OnDocumentLink((documentLinkParams) => {
                        return Task.FromResult(new DocumentLinkContainer<Data>(new DocumentLink<Data>() {
                            Tooltip = "execute-a",
                            Data = new Data() {
                                Child = new Nested() {
                                    Date = DateTimeOffset.MinValue
                                },
                                Id = Guid.NewGuid(),
                                Name = "name"
                            }
                        }));
                    },
                        (documentLink) => {
                            documentLink.Data.Id.Should().NotBeEmpty();
                            documentLink.Data.Child.Should().NotBeNull();
                            documentLink.Data.Name.Should().Be("name");
                            documentLink.Tooltip = "resolved";
                            return Task.FromResult(documentLink);
                        },

                        new DocumentLinkRegistrationOptions() {
                        });
                });

            var items = await client.RequestDocumentLink(new DocumentLinkParams());

            var item = items.Single();

            item = await client.ResolveDocumentLink(item);
            item.Tooltip.Should().Be("resolved");
        }

        [Fact]
        public async Task Should_Resolve_With_Partial_Data()
        {

            var (client, server) = await Initialize(
                options => { }, options => {
                    options.OnDocumentLink<Data>((documentLinkParams, observer) => {
                        var a = new DocumentLinkContainer<Data>(new DocumentLink<Data>() {
                            Tooltip = "execute-a",
                            Data = new Data() {
                                Child = new Nested() {
                                    Date = DateTimeOffset.MinValue
                                },
                                Id = Guid.NewGuid(),
                                Name = "name"
                            }
                        });

                        observer.OnNext(a);
                        observer.OnCompleted();
                    },
                        (documentLink) => {
                            documentLink.Data.Id.Should().NotBeEmpty();
                            documentLink.Data.Child.Should().NotBeNull();
                            documentLink.Data.Name.Should().Be("name");
                            documentLink.Tooltip = "resolved";
                            return Task.FromResult(documentLink);
                        },

                        new DocumentLinkRegistrationOptions() {
                        });
                });

            var item = await client.RequestDocumentLink(new DocumentLinkParams()).SelectMany(z => z).Take(1).ToTask(CancellationToken);

            item = await client.ResolveDocumentLink(item);
            item.Tooltip.Should().Be("resolved");
        }


        [Fact]
        public async Task Should_Resolve_Capability()
        {

            var (client, server) = await Initialize(
                options => { }, options => {
                    options.OnDocumentLink((documentLinkParams, capability, token) => {
                        return Task.FromResult(new DocumentLinkContainer(new DocumentLink() {
                            Tooltip = "execute-a",
                        }));
                    },
                        (documentLink, capability, token) => {
                            documentLink.Tooltip = "resolved";
                            return Task.FromResult(documentLink);
                        },
                        new DocumentLinkRegistrationOptions() {
                        });
                });

            var items = await client.RequestDocumentLink(new DocumentLinkParams());

            var item = items.Single();

            item = await client.ResolveDocumentLink(item);
            item.Tooltip.Should().Be("resolved");
        }

        [Fact]
        public async Task Should_Resolve_Partial_Capability()
        {

            var (client, server) = await Initialize(
                options => { }, options => {
                    options.OnDocumentLink((documentLinkParams, observer, capability, token) => {
                        var a = new DocumentLinkContainer(new DocumentLink() {
                            Tooltip = "execute-a",
                        });

                        observer.OnNext(a);
                        observer.OnCompleted();
                    },

                        (documentLink, capability, token) => {
                            documentLink.Tooltip = "resolved";
                            return Task.FromResult(documentLink);
                        },
                        new DocumentLinkRegistrationOptions() {
                        });
                });

            var item = await client.RequestDocumentLink(new DocumentLinkParams()).SelectMany(z => z).Take(1).ToTask(CancellationToken);

            item = await client.ResolveDocumentLink(item);
            item.Tooltip.Should().Be("resolved");
        }

        [Fact]
        public async Task Should_Resolve_CancellationToken()
        {

            var (client, server) = await Initialize(
                options => { }, options => {
                    options.OnDocumentLink((documentLinkParams, token) => {
                        return Task.FromResult(new DocumentLinkContainer(new DocumentLink() {
                            Tooltip = "execute-a",
                        }));
                    },
                        (documentLink, token) => {
                            documentLink.Tooltip = "resolved";
                            return Task.FromResult(documentLink);
                        },

                        new DocumentLinkRegistrationOptions() {
                        });
                });

            var items = await client.RequestDocumentLink(new DocumentLinkParams());

            var item = items.Single();

            item = await client.ResolveDocumentLink(item);
            item.Tooltip.Should().Be("resolved");
        }

        [Fact]
        public async Task Should_Resolve_Partial_CancellationToken()
        {

            var (client, server) = await Initialize(
                options => { }, options => {
                    options.OnDocumentLink((documentLinkParams, observer, token) => {
                        var a = new DocumentLinkContainer(new DocumentLink() {
                            Tooltip = "execute-a",
                        });

                        observer.OnNext(a);
                        observer.OnCompleted();
                    },
                        (documentLink, token) => {
                            documentLink.Tooltip = "resolved";
                            return Task.FromResult(documentLink);
                        },

                        new DocumentLinkRegistrationOptions() {
                        });
                });

            var item = await client.RequestDocumentLink(new DocumentLinkParams()).SelectMany(z => z).Take(1).ToTask(CancellationToken);

            item = await client.ResolveDocumentLink(item);
            item.Tooltip.Should().Be("resolved");
        }

        [Fact]
        public async Task Should_Resolve()
        {

            var (client, server) = await Initialize(
                options => { }, options => {
                    options.OnDocumentLink((documentLinkParams) => {
                        return Task.FromResult(new DocumentLinkContainer(new DocumentLink() {
                            Tooltip = "execute-a",
                        }));
                    },
                        (documentLink) => {
                            documentLink.Tooltip = "resolved";
                            return Task.FromResult(documentLink);
                        },

                        new DocumentLinkRegistrationOptions() {
                        });
                });

            var items = await client.RequestDocumentLink(new DocumentLinkParams());

            var item = items.Single();

            item = await client.ResolveDocumentLink(item);
            item.Tooltip.Should().Be("resolved");
        }

        [Fact]
        public async Task Should_Resolve_Partial()
        {

            var (client, server) = await Initialize(
                options => { }, options => {
                    options.OnDocumentLink((documentLinkParams, observer) => {
                        var a = new DocumentLinkContainer(new DocumentLink() {
                            Tooltip = "execute-a",
                        });

                        observer.OnNext(a);
                        observer.OnCompleted();
                    },
                        (documentLink) => {
                            documentLink.Tooltip = "resolved";
                            return Task.FromResult(documentLink);
                        },

                        new DocumentLinkRegistrationOptions() {
                        });
                });

            var item = await client.RequestDocumentLink(new DocumentLinkParams()).SelectMany(z => z).Take(1).ToTask(CancellationToken);

            item = await client.ResolveDocumentLink(item);
            item.Tooltip.Should().Be("resolved");
        }

        class Data
        {
            public string Name { get; set; }
            public Guid Id { get; set; }
            public Nested Child { get; set; }

        }

        class Nested
        {
            public DateTimeOffset Date { get; set; }
        }
    }
}
