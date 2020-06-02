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
    public class CompletionHandlerTests : LanguageProtocolTestBase
    {
        public CompletionHandlerTests(ITestOutputHelper outputHelper)  : base(new JsonRpcTestOptions().ConfigureForXUnit(outputHelper))
        {
        }

        [Fact]
        public async Task Should_Return_And_Resolve_Completion()
        {
            var (client, server) = await Initialize(ConfigureClient, options => {
                options.OnCompletion<Data>(
                    async (request, capability, ct) => {
                        return new CompletionList<Data>(new CompletionItem<Data>() {
                                Data = new Data() {
                                    Position = new Position(1,1),
                                    Range = new Range(new Position(1,1), new Position(1,2))
                                }
                            },
                            new CompletionItem<Data>() {
                                Data = new Data() {
                                    Position = new Position(2,2),
                                    Range = new Range(new Position(2,2), new Position(2,3))
                                }
                            },
                            new CompletionItem<Data>() {
                                Data = new Data() {
                                    Position = new Position(3,3),
                                    Range = new Range(new Position(3,3), new Position(3,4))
                                }
                            });
                    },
                    async (request, capability, ct) => {
                        AssertionExtensions.Should((object) request.Data.Position).BeEquivalentTo(new Position(2,2));
                        AssertionExtensions.Should((object) request.Data.Range).BeEquivalentTo(new Range(new Position(2,2), new Position(2,3)));
                        return new CompletionItem<Data>() {
                            Command = new Command() {
                                Arguments = new JArray(),
                                Name = "my command",
                                Title = "My Command"
                            },
                            Data = request.Data,
                        };
                    },
                    new CompletionRegistrationOptions() {
                        DocumentSelector =DocumentSelector.ForLanguage("csharp")
                    }
                );
            });

            var Completion = await client.TextDocument.RequestCompletion(new CompletionParams<ResolvedData>() {
                TextDocument = new TextDocumentIdentifier(DocumentUri.File("/some/path/file.cs"))
            }, CancellationToken);

            Completion.Should().HaveCount(3);

            var cl = Completion.Skip(1).First();
            cl.Command.Should().BeNull();
            cl.Data.Data.Should().ContainKeys("position", "range");

            var resolved = await client.TextDocument.ResolveCompletion(cl, CancellationToken);
            resolved.Command.Should().NotBeNull();
            resolved.Data.Data.Should().ContainKeys("position", "range");
        }

        [Fact]
        public async Task Should_Return_And_Resolve_Completion_With_Known_Model()
        {
            var (client, server) = await Initialize(ConfigureClient, options => {
                options.OnCompletion<Data>(
                    async (request, capability, ct) => {
                        return new CompletionList<Data>(new CompletionItem<Data>() {
                                Data = new Data() {
                                    Position = new Position(1,1),
                                    Range = new Range(new Position(1,1), new Position(1,2))
                                }
                            },
                            new CompletionItem<Data>() {
                                Data = new Data() {
                                    Position = new Position(2,2),
                                    Range = new Range(new Position(2,2), new Position(2,3))
                                }
                            },
                            new CompletionItem<Data>() {
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
                        return new CompletionItem<Data>() {
                            Command = new Command() {
                                Arguments = new JArray(),
                                Name = "my command",
                                Title = "My Command"
                            },
                            Data = request.Data,
                        };
                    },
                    new CompletionRegistrationOptions() {
                        DocumentSelector =DocumentSelector.ForLanguage("csharp")
                    }
                );
            });

            var Completion = await client.TextDocument.RequestCompletion(new CompletionParams<ClientData>() {
                TextDocument = new TextDocumentIdentifier(DocumentUri.File("/some/path/file.cs"))
            }, CancellationToken);

            Completion.Should().HaveCount(3);

            var cl = Completion.Skip(1).First();
            cl.Command.Should().BeNull();
            cl.Data.Position.Should().BeEquivalentTo(new Position(2,2));

            var resolved = await client.TextDocument.ResolveCompletion(cl, CancellationToken);
            resolved.Command.Should().NotBeNull();
            resolved.Data.Position.Should().BeEquivalentTo(new Position(3,3));
        }

        [Fact]
        public async Task Should_Return_And_Resolve_Completion_With_Different_Data_Models()
        {
            var (client, server) = await Initialize(ConfigureClient, options => {
                options.AddTextDocumentIdentifier(WellKnownLanguages.CSharp, WellKnownLanguages.VisualBasic);
                options.OnCompletion<Data>(
                    async (request, capability, ct) => {
                        return new CompletionList<Data>(new CompletionItem<Data>() {
                                Data = new Data() {
                                    Position = new Position(1,1),
                                    Range = new Range(new Position(1,1), new Position(1,2))
                                }
                            },
                            new CompletionItem<Data>() {
                                Data = new Data() {
                                    Position = new Position(2,2),
                                    Range = new Range(new Position(2,2), new Position(2,3))
                                }
                            },
                            new CompletionItem<Data>() {
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
                        return new CompletionItem<Data>() {
                            Command = new Command() {
                                Arguments = new JArray(),
                                Name = "my command",
                                Title = "My Command"
                            },
                            Data = request.Data,
                        };
                    },
                    new CompletionRegistrationOptions() {
                        DocumentSelector =DocumentSelector.ForPattern("**/*.cs")
                    }
                );
                options.OnCompletion<DataSecond>(
                    async (request, capability, ct) => {
                        return new CompletionList<DataSecond>(new CompletionItem<DataSecond>() {
                                Data = new DataSecond() {
                                    Id = "1",
                                }
                            },
                            new CompletionItem<DataSecond>() {
                                Data = new DataSecond() {
                                    Id = "2",
                                }
                            },
                            new CompletionItem<DataSecond>() {
                                Data = new DataSecond() {
                                    Id = "3",
                                }
                            });
                    },
                    async (request, capability, ct) => {
                        request.Data.Id.Should().Be("2");

                        return new CompletionItem<DataSecond>() {
                            Command = new Command() {
                                Arguments = new JArray(),
                                Name = "my command",
                                Title = "My Command"
                            },
                            Data = request.Data,
                        };
                    },
                    new CompletionRegistrationOptions() {
                        DocumentSelector =DocumentSelector.ForPattern("**/*.vb")
                    }
                );
            });

            var CompletionCs = await client.TextDocument.RequestCompletion(new CompletionParams<ResolvedData>() {
                TextDocument = new TextDocumentIdentifier(DocumentUri.File("/some/path/file.cs"))
            }, CancellationToken);
            var CompletionVb = await client.TextDocument.RequestCompletion(new CompletionParams<ResolvedData>() {
                TextDocument = new TextDocumentIdentifier(DocumentUri.File("/some/path/file.vb"))
            }, CancellationToken);

            CompletionCs.Should().HaveCount(3);
            CompletionVb.Should().HaveCount(3);
            CompletionCs.Should().NotBeEquivalentTo(CompletionVb);

            var clCs = CompletionCs.Skip(1).First();
            clCs.Command.Should().BeNull();
            clCs.Data.Data.Should().ContainKeys("position", "range");

            var clVb = CompletionVb.Skip(1).First();
            clVb.Command.Should().BeNull();
            clVb.Data.Data.Should().ContainKeys("id");

            var resolvedCs = await client.TextDocument.ResolveCompletion(clCs, CancellationToken);
            resolvedCs.Command.Should().NotBeNull();
            resolvedCs.Should().BeEquivalentTo(clCs, x => x.Excluding(x => x.Command));

            var resolvedVb = await client.TextDocument.ResolveCompletion(clVb, CancellationToken);
            resolvedVb.Command.Should().NotBeNull();
            resolvedVb.Should().BeEquivalentTo(clVb, x => x.Excluding(x => x.Command));
        }


        [Fact]
        public async Task Should_Return_And_Aggregate_Completion_With_Different_Data_Models_But_Same_Selector()
        {
            var (client, server) = await Initialize(ConfigureClient, options => {
                options.AddTextDocumentIdentifier(WellKnownLanguages.CSharp, WellKnownLanguages.VisualBasic);
                options.OnCompletion<Data>(
                    async (request, capability, ct) => {
                        return new CompletionList<Data>(new CompletionItem<Data>() {
                                Data = new Data() {
                                    Position = new Position(1,1),
                                    Range = new Range(new Position(1,1), new Position(1,2))
                                }
                            },
                            new CompletionItem<Data>() {
                                Data = new Data() {
                                    Position = new Position(2,2),
                                    Range = new Range(new Position(2,2), new Position(2,3))
                                }
                            },
                            new CompletionItem<Data>() {
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

                        return new CompletionItem<Data>() {
                            Command = new Command() {
                                Arguments = new JArray(),
                                Name = "my command",
                                Title = "My Command"
                            },
                            Data = request.Data,
                        };
                    },
                    new CompletionRegistrationOptions() {
                        DocumentSelector =DocumentSelector.ForPattern("**/*.cs")
                    }
                );
                options.OnCompletion<DataSecond>(
                    async (request, capability, ct) => {
                        return new CompletionList<DataSecond>(new CompletionItem<DataSecond>() {
                                Data = new DataSecond() {
                                    Id = "1",
                                }
                            },
                            new CompletionItem<DataSecond>() {
                                Data = new DataSecond() {
                                    Id = "2",
                                }
                            },
                            new CompletionItem<DataSecond>() {
                                Data = new DataSecond() {
                                    Id = "3",
                                }
                            });
                    },
                    async (request, capability, ct) => {
                        request.Data.Id.Should().Be("2");
                        return new CompletionItem<DataSecond>() {
                            Command = new Command() {
                                Arguments = new JArray(),
                                Name = "my command",
                                Title = "My Command"
                            },
                            Data = request.Data,
                        };
                    },
                    new CompletionRegistrationOptions() {
                        DocumentSelector =DocumentSelector.ForPattern("**/*.cs")
                    }
                );
            });

            var CompletionCs = await client.TextDocument.RequestCompletion(new CompletionParams<ResolvedData>() {
                TextDocument = new TextDocumentIdentifier(DocumentUri.File("/some/path/file.cs"))
            }, CancellationToken);

            CompletionCs.Should().HaveCount(6);

            CompletionCs.Should().Match(x => x.Any(z => z.Data.Data.Keys.Contains("position") && z.Data.Data.Keys.Contains("range")));
            CompletionCs.Should().Match(x => x.Any(z => z.Data.Data.Keys.Contains("id")));
        }


        [Fact]
        public async Task Should_Return_And_Resolve_Completion_Using_ClassHandler()
        {
            var (client, server) = await Initialize(ConfigureClient, options => {
                options.WithHandler(new DataCompletionHandler(new CompletionList<Data>(new CompletionItem<Data>() {
                            Data = new Data() {
                                Position = new Position(1, 1),
                                Range = new Range(new Position(1, 1), new Position(1, 2))
                            }
                        },
                        new CompletionItem<Data>() {
                            Data = new Data() {
                                Position = new Position(2, 2),
                                Range = new Range(new Position(2, 2), new Position(2, 3))
                            }
                        },
                        new CompletionItem<Data>() {
                            Data = new Data() {
                                Position = new Position(3, 3),
                                Range = new Range(new Position(3, 3), new Position(3, 4))
                            }
                        }),
                    new CompletionRegistrationOptions() {
                        DocumentSelector = DocumentSelector.ForLanguage("csharp")
                    }
                ));
            });

            var Completion = await client.TextDocument.RequestCompletion(new CompletionParams<ResolvedData>() {
                TextDocument = new TextDocumentIdentifier(DocumentUri.File("/some/path/file.cs"))
            }, CancellationToken);

            Completion.Should().HaveCount(3);

            var cl = Completion.Skip(1).First();
            cl.Command.Should().BeNull();
            cl.Data.Data.Should().ContainKeys("position", "range");

            var resolved = await client.TextDocument.ResolveCompletion(cl, CancellationToken);
            resolved.Command.Should().NotBeNull();
            resolved.Data.Data.Should().ContainKeys("position", "range");
        }

        [Fact]
        public async Task Should_Return_And_Resolve_Completion_With_Known_Model_ClassHandler()
        {
            var (client, server) = await Initialize(ConfigureClient, options => {
                options.WithHandler(new DataCompletionHandler(new CompletionList<Data>(new CompletionItem<Data>() {
                            Data = new Data() {
                                Position = new Position(1, 1),
                                Range = new Range(new Position(1, 1), new Position(1, 2))
                            }
                        },
                        new CompletionItem<Data>() {
                            Data = new Data() {
                                Position = new Position(2, 2),
                                Range = new Range(new Position(2, 2), new Position(2, 3))
                            }
                        },
                        new CompletionItem<Data>() {
                            Data = new Data() {
                                Position = new Position(3, 3),
                                Range = new Range(new Position(3, 3), new Position(3, 4))
                            }
                        }),
                    new CompletionRegistrationOptions() {
                        DocumentSelector = DocumentSelector.ForLanguage("csharp")
                    }
                ));
            });

            var Completion = await client.TextDocument.RequestCompletion(new CompletionParams<ClientData>() {
                TextDocument = new TextDocumentIdentifier(DocumentUri.File("/some/path/file.cs"))
            }, CancellationToken);

            Completion.Should().HaveCount(3);

            var cl = Completion.Skip(1).First();
            cl.Command.Should().BeNull();
            cl.Data.Position.Should().BeEquivalentTo(new Position(2, 2));

            var resolved = await client.TextDocument.ResolveCompletion(cl, CancellationToken);
            resolved.Command.Should().NotBeNull();
            resolved.Data.Position.Should().BeEquivalentTo(new Position(3, 3));
        }

        [Fact]
        public async Task Should_Return_And_Resolve_Completion_With_Different_Data_Models_ClassHandler()
        {
            var (client, server) = await Initialize(ConfigureClient, options => {
                options.AddTextDocumentIdentifier(WellKnownLanguages.CSharp, WellKnownLanguages.VisualBasic);

                options.WithHandler(new DataCompletionHandler(new CompletionList<Data>(new CompletionItem<Data>() {
                            Data = new Data() {
                                Position = new Position(1, 1),
                                Range = new Range(new Position(1, 1), new Position(1, 2))
                            }
                        },
                        new CompletionItem<Data>() {
                            Data = new Data() {
                                Position = new Position(2, 2),
                                Range = new Range(new Position(2, 2), new Position(2, 3))
                            }
                        },
                        new CompletionItem<Data>() {
                            Data = new Data() {
                                Position = new Position(3, 3),
                                Range = new Range(new Position(3, 3), new Position(3, 4))
                            }
                        }),
                    new CompletionRegistrationOptions() {
                        DocumentSelector =  DocumentSelector.ForPattern("**/*.cs")
                    }
                ));
                options.WithHandler(new DataSecondCompletionHandler(new CompletionList<DataSecond>(new CompletionItem<DataSecond>() {
                            Data = new DataSecond() {
                                Id = "1",
                            }
                        },
                        new CompletionItem<DataSecond>() {
                            Data = new DataSecond() {
                                Id = "2",
                            }
                        },
                        new CompletionItem<DataSecond>() {
                            Data = new DataSecond() {
                                Id = "3",
                            }
                        }),
                    new CompletionRegistrationOptions() {
                        DocumentSelector = DocumentSelector.ForPattern("**/*.vb")
                    }
                ));
            });

            var CompletionCs = await client.TextDocument.RequestCompletion(new CompletionParams<ResolvedData>() {
                TextDocument = new TextDocumentIdentifier(DocumentUri.File("/some/path/file.cs"))
            }, CancellationToken);
            var CompletionVb = await client.TextDocument.RequestCompletion(new CompletionParams<ResolvedData>() {
                TextDocument = new TextDocumentIdentifier(DocumentUri.File("/some/path/file.vb"))
            }, CancellationToken);

            CompletionCs.Should().HaveCount(3);
            CompletionVb.Should().HaveCount(3);
            CompletionCs.Should().NotBeEquivalentTo(CompletionVb);

            var clCs = CompletionCs.Skip(1).First();
            clCs.Command.Should().BeNull();
            clCs.Data.Data.Should().ContainKeys("position", "range");

            var clVb = CompletionVb.Skip(1).First();
            clVb.Command.Should().BeNull();
            clVb.Data.Data.Should().ContainKeys("id");

            var resolvedCs = await client.TextDocument.ResolveCompletion(clCs, CancellationToken);
            resolvedCs.Command.Should().NotBeNull();
            resolvedCs.Should().BeEquivalentTo(clCs, x => x.Excluding(x => x.Command));

            var resolvedVb = await client.TextDocument.ResolveCompletion(clVb, CancellationToken);
            resolvedVb.Command.Should().NotBeNull();
            resolvedVb.Should().BeEquivalentTo(clVb, x => x.Excluding(x => x.Command));
        }


        [Fact]
        public async Task Should_Return_And_Aggregate_Completion_With_Different_Data_Models_But_Same_Selector_ClassHandler()
        {
            var (client, server) = await Initialize(ConfigureClient, options => {
                options.AddTextDocumentIdentifier(WellKnownLanguages.CSharp, WellKnownLanguages.VisualBasic);

                options.WithHandler(new DataCompletionHandler(new CompletionList<Data>(new CompletionItem<Data>() {
                            Data = new Data() {
                                Position = new Position(1, 1),
                                Range = new Range(new Position(1, 1), new Position(1, 2))
                            }
                        },
                        new CompletionItem<Data>() {
                            Data = new Data() {
                                Position = new Position(2, 2),
                                Range = new Range(new Position(2, 2), new Position(2, 3))
                            }
                        },
                        new CompletionItem<Data>() {
                            Data = new Data() {
                                Position = new Position(3, 3),
                                Range = new Range(new Position(3, 3), new Position(3, 4))
                            }
                        }),
                    new CompletionRegistrationOptions() {
                        DocumentSelector =  DocumentSelector.ForPattern("**/*.cs")
                    }
                ));
                options.WithHandler(new DataSecondCompletionHandler(new CompletionList<DataSecond>(new CompletionItem<DataSecond>() {
                            Data = new DataSecond() {
                                Id = "1",
                            }
                        },
                        new CompletionItem<DataSecond>() {
                            Data = new DataSecond() {
                                Id = "2",
                            }
                        },
                        new CompletionItem<DataSecond>() {
                            Data = new DataSecond() {
                                Id = "3",
                            }
                        }),
                    new CompletionRegistrationOptions() {
                        DocumentSelector = DocumentSelector.ForPattern("**/*.cs")
                    }
                ));
            });

            var CompletionCs = await client.TextDocument.RequestCompletion(new CompletionParams<ResolvedData>() {
                TextDocument = new TextDocumentIdentifier(DocumentUri.File("/some/path/file.cs"))
            }, CancellationToken);

            CompletionCs.Should().HaveCount(6);

            CompletionCs.Should().Match(x => x.Any(z => z.Data.Data.Keys.Contains("position") && z.Data.Data.Keys.Contains("range")));
            CompletionCs.Should().Match(x => x.Any(z => z.Data.Data.Keys.Contains("id")));
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

        class DataCompletionHandler : CompletionHandler<Data>
        {
            private readonly CompletionList<Data> _container;

            public DataCompletionHandler(CompletionList<Data> container, CompletionRegistrationOptions registrationOptions) : base(registrationOptions)
            {
                _container = container;
                registrationOptions.ResolveProvider = true;
            }

            public override async Task<CompletionList<Data>> Handle(CompletionParams<Data> request, CancellationToken cancellationToken) => _container;

            public override async Task<CompletionItem<Data>> Handle(CompletionItem<Data> request, CancellationToken cancellationToken)
            {
                request.Data.Position.Should().BeEquivalentTo(new Position(2,2));
                request.Data.Range.Should().BeEquivalentTo(new Range(new Position(2,2), new Position(2,3)));
                request.Data.Position = new Position(3, 3);
                return new CompletionItem<Data>() {
                    Command = new Command() {
                        Arguments = new JArray(),
                        Name = "my command",
                        Title = "My Command"
                    },
                    Data = request.Data,
                };
            }
        }

        class DataSecondCompletionHandler : CompletionHandler<DataSecond>
        {
            private readonly CompletionList<DataSecond> _container;

            public DataSecondCompletionHandler(CompletionList<DataSecond> container, CompletionRegistrationOptions registrationOptions) : base(registrationOptions)
            {
                _container = container;
                registrationOptions.ResolveProvider = true;
            }

            public override async Task<CompletionList<DataSecond>> Handle(CompletionParams<DataSecond> request, CancellationToken cancellationToken) => _container;

            public override async Task<CompletionItem<DataSecond>> Handle(CompletionItem<DataSecond> request, CancellationToken cancellationToken)
            {
                request.Data.Id.Should().Be("2");
                return new CompletionItem<DataSecond>() {
                    Command = new Command() {
                        Arguments = new JArray(),
                        Name = "my command",
                        Title = "My Command"
                    },
                    Data = request.Data,
                };
            }
        }

        class ClientData : CanBeResolvedData
        {
            public Position Position { get; set; }
        }
    }
}
