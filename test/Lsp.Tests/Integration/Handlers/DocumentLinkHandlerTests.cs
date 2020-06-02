using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc.Testing;
using OmniSharp.Extensions.LanguageProtocol.Testing;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Xunit;
using Xunit.Abstractions;

namespace Lsp.Tests.Integration.Handlers
{
    public class DocumentLinkHandlerTests : LanguageProtocolTestBase
    {
        public DocumentLinkHandlerTests(ITestOutputHelper outputHelper)  : base(new JsonRpcTestOptions().ConfigureForXUnit(outputHelper))
        {
        }

        [Fact]
        public async Task Should_Return_And_Resolve_DocumentLink()
        {
            var (client, server) = await Initialize(ConfigureClient, options => {
                options.OnDocumentLink<Data>(
                    async (request, capability, ct) => {
                        return new DocumentLinkContainer<Data>(new DocumentLink<Data>() {
                                Data = new Data() {
                                    Position = new Position(1,1),
                                    Range = new Range(new Position(1,1), new Position(1,2))
                                }
                            },
                            new DocumentLink<Data>() {
                                Data = new Data() {
                                    Position = new Position(2,2),
                                    Range = new Range(new Position(2,2), new Position(2,3))
                                }
                            },
                            new DocumentLink<Data>() {
                                Data = new Data() {
                                    Position = new Position(3,3),
                                    Range = new Range(new Position(3,3), new Position(3,4))
                                }
                            });
                    },
                    async (request, capability, ct) => {
                        AssertionExtensions.Should((object) request.Data.Position).BeEquivalentTo(new Position(2,2));
                        AssertionExtensions.Should((object) request.Data.Range).BeEquivalentTo(new Range(new Position(2,2), new Position(2,3)));
                        return new DocumentLink<Data>() {
                            Target = DocumentUri.File("/some/path"),
                            Tooltip = "Tooltip",
                            Data = request.Data,
                            Range = request.Range
                        };
                    },
                    new DocumentLinkRegistrationOptions() {
                        DocumentSelector =DocumentSelector.ForLanguage("csharp")
                    }
                );
            });

            var documentLink = await client.TextDocument.RequestDocumentLink(new DocumentLinkParams<ResolvedData>() {
                TextDocument = new TextDocumentIdentifier(DocumentUri.File("/some/path/file.cs"))
            }, CancellationToken);

            documentLink.Should().HaveCount(3);

            var cl = documentLink.Skip(1).First();
            cl.Tooltip.Should().BeNull();
            cl.Target.Should().BeNull();
            cl.Data.Data.Should().ContainKeys("position", "range");

            var resolved = await client.TextDocument.ResolveDocumentLink(cl, CancellationToken);
            resolved.Tooltip.Should().NotBeNull();
            resolved.Target.Should().NotBeNull();
            resolved.Data.Data.Should().ContainKeys("position", "range");
        }

        [Fact]
        public async Task Should_Return_And_Resolve_DocumentLink_With_Known_Model()
        {
            var (client, server) = await Initialize(ConfigureClient, options => {
                options.OnDocumentLink<Data>(
                    async (request, capability, ct) => {
                        return new DocumentLinkContainer<Data>(new DocumentLink<Data>() {
                                Data = new Data() {
                                    Position = new Position(1,1),
                                    Range = new Range(new Position(1,1), new Position(1,2))
                                }
                            },
                            new DocumentLink<Data>() {
                                Data = new Data() {
                                    Position = new Position(2,2),
                                    Range = new Range(new Position(2,2), new Position(2,3))
                                }
                            },
                            new DocumentLink<Data>() {
                                Data = new Data() {
                                    Position = new Position(3,3),
                                    Range = new Range(new Position(3,3), new Position(3,4))
                                }
                            });
                    },
                    async (request, capability, ct) => {
                        request.Data.Position.Should().BeEquivalentTo(new Position(2,2));
                        request.Data.Range.Should().BeEquivalentTo(new Range(new Position(2,2), new Position(2,3)));
                        request.Data.Position = new Position(3, 3);
                        return new DocumentLink<Data>() {
                            Target = DocumentUri.File("/some/path"),
                            Tooltip = "Tooltip",
                            Data = request.Data,
                            Range = request.Range
                        };
                    },
                    new DocumentLinkRegistrationOptions() {
                        DocumentSelector =DocumentSelector.ForLanguage("csharp")
                    }
                );
            });

            var documentLink = await client.TextDocument.RequestDocumentLink(new DocumentLinkParams<ClientData>() {
                TextDocument = new TextDocumentIdentifier(DocumentUri.File("/some/path/file.cs"))
            }, CancellationToken);

            documentLink.Should().HaveCount(3);

            var cl = documentLink.Skip(1).First();
            cl.Tooltip.Should().BeNull();
            cl.Target.Should().BeNull();
            cl.Data.Position.Should().BeEquivalentTo(new Position(2,2));

            var resolved = await client.TextDocument.ResolveDocumentLink(cl, CancellationToken);
            resolved.Tooltip.Should().NotBeNull();
            resolved.Target.Should().NotBeNull();
            resolved.Data.Position.Should().BeEquivalentTo(new Position(3,3));
        }

        [Fact]
        public async Task Should_Return_And_Resolve_DocumentLink_With_Different_Data_Models()
        {
            var (client, server) = await Initialize(ConfigureClient, options => {
                options.AddTextDocumentIdentifier(WellKnownLanguages.CSharp, WellKnownLanguages.VisualBasic);
                options.OnDocumentLink<Data>(
                    async (request, capability, ct) => {
                        return new DocumentLinkContainer<Data>(new DocumentLink<Data>() {
                                Data = new Data() {
                                    Position = new Position(1,1),
                                    Range = new Range(new Position(1,1), new Position(1,2))
                                }
                            },
                            new DocumentLink<Data>() {
                                Data = new Data() {
                                    Position = new Position(2,2),
                                    Range = new Range(new Position(2,2), new Position(2,3))
                                }
                            },
                            new DocumentLink<Data>() {
                                Data = new Data() {
                                    Position = new Position(3,3),
                                    Range = new Range(new Position(3,3), new Position(3,4))
                                }
                            });
                    },
                    async (request, capability, ct) => {
                        request.Data.Position.Should().BeEquivalentTo(new Position(2,2));
                        request.Data.Range.Should().BeEquivalentTo(new Range(new Position(2,2), new Position(2,3)));
                        request.Data.Position = new Position(3, 3);
                        return new DocumentLink<Data>() {
                            Target = DocumentUri.File("/some/path"),
                            Tooltip = "Tooltip",
                            Data = request.Data,
                            Range = request.Range
                        };
                    },
                    new DocumentLinkRegistrationOptions() {
                        DocumentSelector =DocumentSelector.ForPattern("**/*.cs")
                    }
                );
                options.OnDocumentLink<DataSecond>(
                    async (request, capability, ct) => {
                        return new DocumentLinkContainer<DataSecond>(new DocumentLink<DataSecond>() {
                                Data = new DataSecond() {
                                    Id = "1",
                                }
                            },
                            new DocumentLink<DataSecond>() {
                                Data = new DataSecond() {
                                    Id = "2",
                                }
                            },
                            new DocumentLink<DataSecond>() {
                                Data = new DataSecond() {
                                    Id = "3",
                                }
                            });
                    },
                    async (request, capability, ct) => {
                        request.Data.Id.Should().Be("2");

                        return new DocumentLink<DataSecond>() {
                            Target = DocumentUri.File("/some/path"),
                            Tooltip = "Tooltip",
                            Data = request.Data,
                            Range = request.Range
                        };
                    },
                    new DocumentLinkRegistrationOptions() {
                        DocumentSelector =DocumentSelector.ForPattern("**/*.vb")
                    }
                );
            });

            var documentLinkCs = await client.TextDocument.RequestDocumentLink(new DocumentLinkParams<ResolvedData>() {
                TextDocument = new TextDocumentIdentifier(DocumentUri.File("/some/path/file.cs"))
            }, CancellationToken);
            var documentLinkVb = await client.TextDocument.RequestDocumentLink(new DocumentLinkParams<ResolvedData>() {
                TextDocument = new TextDocumentIdentifier(DocumentUri.File("/some/path/file.vb"))
            }, CancellationToken);

            documentLinkCs.Should().HaveCount(3);
            documentLinkVb.Should().HaveCount(3);
            documentLinkCs.Should().NotBeEquivalentTo(documentLinkVb);

            var clCs = documentLinkCs.Skip(1).First();
            clCs.Tooltip.Should().BeNull();
            clCs.Target.Should().BeNull();
            clCs.Data.Data.Should().ContainKeys("position", "range");

            var clVb = documentLinkVb.Skip(1).First();
            clVb.Tooltip.Should().BeNull();
            clVb.Target.Should().BeNull();
            clVb.Data.Data.Should().ContainKeys("id");

            var resolvedCs = await client.TextDocument.ResolveDocumentLink(clCs, CancellationToken);
            resolvedCs.Tooltip.Should().NotBeNull();
            resolvedCs.Target.Should().NotBeNull();
            resolvedCs.Should().BeEquivalentTo(clCs, x => x.Excluding(x => x.Tooltip).Excluding(x => x.Target));

            var resolvedVb = await client.TextDocument.ResolveDocumentLink(clVb, CancellationToken);
            resolvedVb.Tooltip.Should().NotBeNull();
            resolvedVb.Target.Should().NotBeNull();
            resolvedVb.Should().BeEquivalentTo(clVb, x => x.Excluding(x => x.Tooltip).Excluding(x => x.Target));
        }


        [Fact]
        public async Task Should_Return_And_Aggregate_DocumentLink_With_Different_Data_Models_But_Same_Selector()
        {
            var (client, server) = await Initialize(ConfigureClient, options => {
                options.AddTextDocumentIdentifier(WellKnownLanguages.CSharp, WellKnownLanguages.VisualBasic);
                options.OnDocumentLink<Data>(
                    async (request, capability, ct) => {
                        return new DocumentLinkContainer<Data>(new DocumentLink<Data>() {
                                Data = new Data() {
                                    Position = new Position(1,1),
                                    Range = new Range(new Position(1,1), new Position(1,2))
                                }
                            },
                            new DocumentLink<Data>() {
                                Data = new Data() {
                                    Position = new Position(2,2),
                                    Range = new Range(new Position(2,2), new Position(2,3))
                                }
                            },
                            new DocumentLink<Data>() {
                                Data = new Data() {
                                    Position = new Position(3,3),
                                    Range = new Range(new Position(3,3), new Position(3,4))
                                }
                            });
                    },
                    async (request, capability, ct) => {
                        request.Data.Position.Should().BeEquivalentTo(new Position(2,2));
                        request.Data.Range.Should().BeEquivalentTo(new Range(new Position(2,2), new Position(2,3)));
                        request.Data.Position = new Position(3, 3);

                        return new DocumentLink<Data>() {
                            Target = DocumentUri.File("/some/path"),
                            Tooltip = "Tooltip",
                            Data = request.Data,
                            Range = request.Range
                        };
                    },
                    new DocumentLinkRegistrationOptions() {
                        DocumentSelector =DocumentSelector.ForPattern("**/*.cs")
                    }
                );
                options.OnDocumentLink<DataSecond>(
                    async (request, capability, ct) => {
                        return new DocumentLinkContainer<DataSecond>(new DocumentLink<DataSecond>() {
                                Data = new DataSecond() {
                                    Id = "1",
                                }
                            },
                            new DocumentLink<DataSecond>() {
                                Data = new DataSecond() {
                                    Id = "2",
                                }
                            },
                            new DocumentLink<DataSecond>() {
                                Data = new DataSecond() {
                                    Id = "3",
                                }
                            });
                    },
                    async (request, capability, ct) => {
                        request.Data.Id.Should().Be("2");
                        return new DocumentLink<DataSecond>() {
                            Target = DocumentUri.File("/some/path"),
                            Tooltip = "Tooltip",
                            Data = request.Data,
                            Range = request.Range
                        };
                    },
                    new DocumentLinkRegistrationOptions() {
                        DocumentSelector =DocumentSelector.ForPattern("**/*.cs")
                    }
                );
            });

            var documentLinkCs = await client.TextDocument.RequestDocumentLink(new DocumentLinkParams<ResolvedData>() {
                TextDocument = new TextDocumentIdentifier(DocumentUri.File("/some/path/file.cs"))
            }, CancellationToken);

            documentLinkCs.Should().HaveCount(6);

            documentLinkCs.Should().Match(x => x.Any(z => z.Data.Data.Keys.Contains("position") && z.Data.Data.Keys.Contains("range")));
            documentLinkCs.Should().Match(x => x.Any(z => z.Data.Data.Keys.Contains("id")));
        }


        [Fact]
        public async Task Should_Return_And_Resolve_DocumentLink_Using_ClassHandler()
        {
            var (client, server) = await Initialize(ConfigureClient, options => {
                options.WithHandler(new DataDocumentLinkHandler(new DocumentLinkContainer<Data>(new DocumentLink<Data>() {
                            Data = new Data() {
                                Position = new Position(1, 1),
                                Range = new Range(new Position(1, 1), new Position(1, 2))
                            }
                        },
                        new DocumentLink<Data>() {
                            Data = new Data() {
                                Position = new Position(2, 2),
                                Range = new Range(new Position(2, 2), new Position(2, 3))
                            }
                        },
                        new DocumentLink<Data>() {
                            Data = new Data() {
                                Position = new Position(3, 3),
                                Range = new Range(new Position(3, 3), new Position(3, 4))
                            }
                        }),
                    new DocumentLinkRegistrationOptions() {
                        DocumentSelector = DocumentSelector.ForLanguage("csharp")
                    }
                ));
            });

            var documentLink = await client.TextDocument.RequestDocumentLink(new DocumentLinkParams<ResolvedData>() {
                TextDocument = new TextDocumentIdentifier(DocumentUri.File("/some/path/file.cs"))
            }, CancellationToken);

            documentLink.Should().HaveCount(3);

            var cl = documentLink.Skip(1).First();
            cl.Tooltip.Should().BeNull();
            cl.Target.Should().BeNull();
            cl.Data.Data.Should().ContainKeys("position", "range");

            var resolved = await client.TextDocument.ResolveDocumentLink(cl, CancellationToken);
            resolved.Tooltip.Should().NotBeNull();
            resolved.Target.Should().NotBeNull();
            resolved.Data.Data.Should().ContainKeys("position", "range");
        }

        [Fact]
        public async Task Should_Return_And_Resolve_DocumentLink_With_Known_Model_ClassHandler()
        {
            var (client, server) = await Initialize(ConfigureClient, options => {
                options.WithHandler(new DataDocumentLinkHandler(new DocumentLinkContainer<Data>(new DocumentLink<Data>() {
                            Data = new Data() {
                                Position = new Position(1, 1),
                                Range = new Range(new Position(1, 1), new Position(1, 2))
                            }
                        },
                        new DocumentLink<Data>() {
                            Data = new Data() {
                                Position = new Position(2, 2),
                                Range = new Range(new Position(2, 2), new Position(2, 3))
                            }
                        },
                        new DocumentLink<Data>() {
                            Data = new Data() {
                                Position = new Position(3, 3),
                                Range = new Range(new Position(3, 3), new Position(3, 4))
                            }
                        }),
                    new DocumentLinkRegistrationOptions() {
                        DocumentSelector = DocumentSelector.ForLanguage("csharp")
                    }
                ));
            });

            var documentLink = await client.TextDocument.RequestDocumentLink(new DocumentLinkParams<ClientData>() {
                TextDocument = new TextDocumentIdentifier(DocumentUri.File("/some/path/file.cs"))
            }, CancellationToken);

            documentLink.Should().HaveCount(3);

            var cl = documentLink.Skip(1).First();
            cl.Tooltip.Should().BeNull();
            cl.Target.Should().BeNull();
            cl.Data.Position.Should().BeEquivalentTo(new Position(2, 2));

            var resolved = await client.TextDocument.ResolveDocumentLink(cl, CancellationToken);
            resolved.Tooltip.Should().NotBeNull();
            resolved.Target.Should().NotBeNull();
            resolved.Data.Position.Should().BeEquivalentTo(new Position(3, 3));
        }

        [Fact]
        public async Task Should_Return_And_Resolve_DocumentLink_With_Different_Data_Models_ClassHandler()
        {
            var (client, server) = await Initialize(ConfigureClient, options => {
                options.AddTextDocumentIdentifier(WellKnownLanguages.CSharp, WellKnownLanguages.VisualBasic);

                options.WithHandler(new DataDocumentLinkHandler(new DocumentLinkContainer<Data>(new DocumentLink<Data>() {
                            Data = new Data() {
                                Position = new Position(1, 1),
                                Range = new Range(new Position(1, 1), new Position(1, 2))
                            }
                        },
                        new DocumentLink<Data>() {
                            Data = new Data() {
                                Position = new Position(2, 2),
                                Range = new Range(new Position(2, 2), new Position(2, 3))
                            }
                        },
                        new DocumentLink<Data>() {
                            Data = new Data() {
                                Position = new Position(3, 3),
                                Range = new Range(new Position(3, 3), new Position(3, 4))
                            }
                        }),
                    new DocumentLinkRegistrationOptions() {
                        DocumentSelector =  DocumentSelector.ForPattern("**/*.cs")
                    }
                ));
                options.WithHandler(new DataSecondDocumentLinkHandler(new DocumentLinkContainer<DataSecond>(new DocumentLink<DataSecond>() {
                                Data = new DataSecond() {
                                    Id = "1",
                                }
                            },
                            new DocumentLink<DataSecond>() {
                                Data = new DataSecond() {
                                    Id = "2",
                                }
                            },
                            new DocumentLink<DataSecond>() {
                                Data = new DataSecond() {
                                    Id = "3",
                                }
                            }),
                    new DocumentLinkRegistrationOptions() {
                        DocumentSelector = DocumentSelector.ForPattern("**/*.vb")
                    }
                ));
            });

            var documentLinkCs = await client.TextDocument.RequestDocumentLink(new DocumentLinkParams<ResolvedData>() {
                TextDocument = new TextDocumentIdentifier(DocumentUri.File("/some/path/file.cs"))
            }, CancellationToken);
            var documentLinkVb = await client.TextDocument.RequestDocumentLink(new DocumentLinkParams<ResolvedData>() {
                TextDocument = new TextDocumentIdentifier(DocumentUri.File("/some/path/file.vb"))
            }, CancellationToken);

            documentLinkCs.Should().HaveCount(3);
            documentLinkVb.Should().HaveCount(3);
            documentLinkCs.Should().NotBeEquivalentTo(documentLinkVb);

            var clCs = documentLinkCs.Skip(1).First();
            clCs.Target.Should().BeNull();
            clCs.Tooltip.Should().BeNull();
            clCs.Data.Data.Should().ContainKeys("position", "range");

            var clVb = documentLinkVb.Skip(1).First();
            clVb.Target.Should().BeNull();
            clVb.Tooltip.Should().BeNull();
            clVb.Data.Data.Should().ContainKeys("id");

            var resolvedCs = await client.TextDocument.ResolveDocumentLink(clCs, CancellationToken);
            resolvedCs.Target.Should().NotBeNull();
            resolvedCs.Tooltip.Should().NotBeNull();
            resolvedCs.Should().BeEquivalentTo(clCs, x => x.Excluding(x => x.Target).Excluding(x => x.Tooltip));

            var resolvedVb = await client.TextDocument.ResolveDocumentLink(clVb, CancellationToken);
            resolvedVb.Target.Should().NotBeNull();
            resolvedVb.Tooltip.Should().NotBeNull();
            resolvedVb.Should().BeEquivalentTo(clVb, x => x.Excluding(x => x.Target).Excluding(x => x.Tooltip));
        }


        [Fact]
        public async Task Should_Return_And_Aggregate_DocumentLink_With_Different_Data_Models_But_Same_Selector_ClassHandler()
        {
            var (client, server) = await Initialize(ConfigureClient, options => {
                options.AddTextDocumentIdentifier(WellKnownLanguages.CSharp, WellKnownLanguages.VisualBasic);

                options.WithHandler(new DataDocumentLinkHandler(new DocumentLinkContainer<Data>(new DocumentLink<Data>() {
                            Data = new Data() {
                                Position = new Position(1, 1),
                                Range = new Range(new Position(1, 1), new Position(1, 2))
                            }
                        },
                        new DocumentLink<Data>() {
                            Data = new Data() {
                                Position = new Position(2, 2),
                                Range = new Range(new Position(2, 2), new Position(2, 3))
                            }
                        },
                        new DocumentLink<Data>() {
                            Data = new Data() {
                                Position = new Position(3, 3),
                                Range = new Range(new Position(3, 3), new Position(3, 4))
                            }
                        }),
                    new DocumentLinkRegistrationOptions() {
                        DocumentSelector =  DocumentSelector.ForPattern("**/*.cs")
                    }
                ));
                options.WithHandler(new DataSecondDocumentLinkHandler(new DocumentLinkContainer<DataSecond>(new DocumentLink<DataSecond>() {
                                Data = new DataSecond() {
                                    Id = "1",
                                }
                            },
                            new DocumentLink<DataSecond>() {
                                Data = new DataSecond() {
                                    Id = "2",
                                }
                            },
                            new DocumentLink<DataSecond>() {
                                Data = new DataSecond() {
                                    Id = "3",
                                }
                            }),
                    new DocumentLinkRegistrationOptions() {
                        DocumentSelector = DocumentSelector.ForPattern("**/*.cs")
                    }
                ));
            });

            var documentLinkCs = await client.TextDocument.RequestDocumentLink(new DocumentLinkParams<ResolvedData>() {
                TextDocument = new TextDocumentIdentifier(DocumentUri.File("/some/path/file.cs"))
            }, CancellationToken);

            documentLinkCs.Should().HaveCount(6);

            documentLinkCs.Should().Match(x => x.Any(z => z.Data.Data.Keys.Contains("position") && z.Data.Data.Keys.Contains("range")));
            documentLinkCs.Should().Match(x => x.Any(z => z.Data.Data.Keys.Contains("id")));
        }

        // custom data
        // resolve custom data
        // resolve as resolved data

        // add two different types and negotiate with resolve

        private void ConfigureClient(LanguageClientOptions options)
        {
        }

        class Data : CanBeResolvedData
        {
            public Position Position { get; set; }
            public Range Range { get; set; }
        }

        class DataSecond : CanBeResolvedData
        {
            public string Id { get; set; }
        }

        class DataDocumentLinkHandler : DocumentLinkHandler<Data>
        {
            private readonly DocumentLinkContainer<Data> _container;

            public DataDocumentLinkHandler(DocumentLinkContainer<Data> container, DocumentLinkRegistrationOptions registrationOptions) : base(registrationOptions)
            {
                _container = container;
                registrationOptions.ResolveProvider = true;
            }

            public override async Task<DocumentLinkContainer<Data>> Handle(DocumentLinkParams<Data> request, CancellationToken cancellationToken) => _container;

            public override async Task<DocumentLink<Data>> Handle(DocumentLink<Data> request, CancellationToken cancellationToken)
            {
                request.Data.Position.Should().BeEquivalentTo(new Position(2,2));
                request.Data.Range.Should().BeEquivalentTo(new Range(new Position(2,2), new Position(2,3)));
                request.Data.Position = new Position(3, 3);
                return new DocumentLink<Data>() {
                    Target = DocumentUri.File("/some/path"),
                    Tooltip = "Tooltip",
                    Data = request.Data,
                    Range = request.Range
                };
            }
        }

        class DataSecondDocumentLinkHandler : DocumentLinkHandler<DataSecond>
        {
            private readonly DocumentLinkContainer<DataSecond> _container;

            public DataSecondDocumentLinkHandler(DocumentLinkContainer<DataSecond> container, DocumentLinkRegistrationOptions registrationOptions) : base(registrationOptions)
            {
                _container = container;
                registrationOptions.ResolveProvider = true;
            }

            public override async Task<DocumentLinkContainer<DataSecond>> Handle(DocumentLinkParams<DataSecond> request, CancellationToken cancellationToken) => _container;

            public override async Task<DocumentLink<DataSecond>> Handle(DocumentLink<DataSecond> request, CancellationToken cancellationToken)
            {
                request.Data.Id.Should().Be("2");
                return new DocumentLink<DataSecond>() {
                    Target = DocumentUri.File("/some/path"),
                    Tooltip = "Tooltip",
                    Data = request.Data,
                    Range = request.Range
                };
            }
        }

        class ClientData : CanBeResolvedData
        {
            public Position Position { get; set; }
        }
    }
}
