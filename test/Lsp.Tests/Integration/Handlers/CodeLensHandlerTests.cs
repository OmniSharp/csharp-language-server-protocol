using System;
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
using OmniSharp.Extensions.LanguageServer.Server;
using Xunit;
using Xunit.Abstractions;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Lsp.Tests.Integration.Handlers
{
    public class CodeLensHandlerTests : LanguageProtocolTestBase
    {
        public CodeLensHandlerTests(ITestOutputHelper outputHelper) : base(new JsonRpcTestOptions().ConfigureForXUnit(outputHelper))
        {
        }

        [Fact]
        public async Task Should_Return_And_Resolve_CodeLens()
        {
            var (client, server) = await Initialize(ConfigureClient, options => {
                options.OnCodeLens<Data>(
                    async (request, capability, ct) => {
                        return new CodeLensContainer<Data>(new CodeLens<Data>() {
                                Data = new Data() {
                                    Position = new Position(1, 1),
                                    Range = new Range(new Position(1, 1), new Position(1, 2))
                                }
                            },
                            new CodeLens<Data>() {
                                Data = new Data() {
                                    Position = new Position(2, 2),
                                    Range = new Range(new Position(2, 2), new Position(2, 3))
                                }
                            },
                            new CodeLens<Data>() {
                                Data = new Data() {
                                    Position = new Position(3, 3),
                                    Range = new Range(new Position(3, 3), new Position(3, 4))
                                }
                            });
                    },
                    async (request, capability, ct) => {
                        request.Data.Position.Should().BeEquivalentTo(new Position(2, 2));
                        request.Data.Range.Should().BeEquivalentTo(new Range(new Position(2, 2), new Position(2, 3)));
                        return new CodeLens<Data>() {
                            Command = new Command() {
                                Arguments = new JArray(),
                                Name = "my command",
                                Title = "My Command"
                            },
                            Data = request.Data,
                            Range = request.Range
                        };
                    },
                    new CodeLensRegistrationOptions() {
                        DocumentSelector = DocumentSelector.ForLanguage("csharp")
                    }
                );
            });

            var codeLens = await client.TextDocument.RequestCodeLens(new CodeLensParams<ResolvedData>() {
                TextDocument = new TextDocumentIdentifier(DocumentUri.File("/some/path/file.cs"))
            }, CancellationToken);

            codeLens.Should().HaveCount(3);

            var cl = codeLens.Skip(1).First();
            cl.Command.Should().BeNull();
            cl.Data.Data.Should().ContainKeys("position", "range");

            var resolved = await client.TextDocument.ResolveCodeLens(cl, CancellationToken);
            resolved.Command.Should().NotBeNull();
            resolved.Data.Data.Should().ContainKeys("position", "range");
        }

        [Fact]
        public async Task Should_Return_And_Resolve_CodeLens_With_Known_Model()
        {
            var (client, server) = await Initialize(ConfigureClient, options => {
                options.OnCodeLens<Data>(
                    async (request, capability, ct) => {
                        return new CodeLensContainer<Data>(new CodeLens<Data>() {
                                Data = new Data() {
                                    Position = new Position(1, 1),
                                    Range = new Range(new Position(1, 1), new Position(1, 2))
                                }
                            },
                            new CodeLens<Data>() {
                                Data = new Data() {
                                    Position = new Position(2, 2),
                                    Range = new Range(new Position(2, 2), new Position(2, 3))
                                }
                            },
                            new CodeLens<Data>() {
                                Data = new Data() {
                                    Position = new Position(3, 3),
                                    Range = new Range(new Position(3, 3), new Position(3, 4))
                                }
                            });
                    },
                    async (request, capability, ct) => {
                        request.Data.Position.Should().BeEquivalentTo(new Position(2, 2));
                        request.Data.Range.Should().BeEquivalentTo(new Range(new Position(2, 2), new Position(2, 3)));
                        request.Data.Position = new Position(3, 3);
                        return new CodeLens<Data>() {
                            Command = new Command() {
                                Arguments = new JArray(),
                                Name = "my command",
                                Title = "My Command"
                            },
                            Data = request.Data,
                            Range = request.Range
                        };
                    },
                    new CodeLensRegistrationOptions() {
                        DocumentSelector = DocumentSelector.ForLanguage("csharp")
                    }
                );
            });

            var codeLens = await client.TextDocument.RequestCodeLens(new CodeLensParams<ClientData>() {
                TextDocument = new TextDocumentIdentifier(DocumentUri.File("/some/path/file.cs"))
            }, CancellationToken);

            codeLens.Should().HaveCount(3);

            var cl = codeLens.Skip(1).First();
            cl.Command.Should().BeNull();
            cl.Data.Position.Should().BeEquivalentTo(new Position(2, 2));

            var resolved = await client.TextDocument.ResolveCodeLens(cl, CancellationToken);
            resolved.Command.Should().NotBeNull();
            resolved.Data.Position.Should().BeEquivalentTo(new Position(3, 3));
        }

        [Fact]
        public async Task Should_Return_And_Resolve_CodeLens_With_Different_Data_Models()
        {
            var (client, server) = await Initialize(ConfigureClient, options => {
                options.AddTextDocumentIdentifier(WellKnownLanguages.CSharp, WellKnownLanguages.VisualBasic);
                options.OnCodeLens<Data>(
                    async (request, capability, ct) => {
                        return new CodeLensContainer<Data>(new CodeLens<Data>() {
                                Data = new Data() {
                                    Position = new Position(1, 1),
                                    Range = new Range(new Position(1, 1), new Position(1, 2))
                                }
                            },
                            new CodeLens<Data>() {
                                Data = new Data() {
                                    Position = new Position(2, 2),
                                    Range = new Range(new Position(2, 2), new Position(2, 3))
                                }
                            },
                            new CodeLens<Data>() {
                                Data = new Data() {
                                    Position = new Position(3, 3),
                                    Range = new Range(new Position(3, 3), new Position(3, 4))
                                }
                            });
                    },
                    async (request, capability, ct) => {
                        request.Data.Position.Should().BeEquivalentTo(new Position(2, 2));
                        request.Data.Range.Should().BeEquivalentTo(new Range(new Position(2, 2), new Position(2, 3)));
                        request.Data.Position = new Position(3, 3);
                        return new CodeLens<Data>() {
                            Command = new Command() {
                                Arguments = new JArray(),
                                Name = "my command",
                                Title = "My Command"
                            },
                            Data = request.Data,
                            Range = request.Range
                        };
                    },
                    new CodeLensRegistrationOptions() {
                        DocumentSelector = DocumentSelector.ForPattern("**/*.cs")
                    }
                );
                options.OnCodeLens<DataSecond>(
                    async (request, capability, ct) => {
                        return new CodeLensContainer<DataSecond>(new CodeLens<DataSecond>() {
                                Data = new DataSecond() {
                                    Id = "1",
                                }
                            },
                            new CodeLens<DataSecond>() {
                                Data = new DataSecond() {
                                    Id = "2",
                                }
                            },
                            new CodeLens<DataSecond>() {
                                Data = new DataSecond() {
                                    Id = "3",
                                }
                            });
                    },
                    async (request, capability, ct) => {
                        request.Data.Id.Should().Be("2");

                        return new CodeLens<DataSecond>() {
                            Command = new Command() {
                                Arguments = new JArray(),
                                Name = "my command",
                                Title = "My Command"
                            },
                            Data = request.Data,
                            Range = request.Range
                        };
                    },
                    new CodeLensRegistrationOptions() {
                        DocumentSelector = DocumentSelector.ForPattern("**/*.vb")
                    }
                );
            });

            var codeLensCs = await client.TextDocument.RequestCodeLens(new CodeLensParams<ResolvedData>() {
                TextDocument = new TextDocumentIdentifier(DocumentUri.File("/some/path/file.cs"))
            }, CancellationToken);
            var codeLensVb = await client.TextDocument.RequestCodeLens(new CodeLensParams<ResolvedData>() {
                TextDocument = new TextDocumentIdentifier(DocumentUri.File("/some/path/file.vb"))
            }, CancellationToken);

            codeLensCs.Should().HaveCount(3);
            codeLensVb.Should().HaveCount(3);
            codeLensCs.Should().NotBeEquivalentTo(codeLensVb);

            var clCs = codeLensCs.Skip(1).First();
            clCs.Command.Should().BeNull();
            clCs.Data.Data.Should().ContainKeys("position", "range");

            var clVb = codeLensVb.Skip(1).First();
            clVb.Command.Should().BeNull();
            clVb.Data.Data.Should().ContainKeys("id");

            var resolvedCs = await client.TextDocument.ResolveCodeLens(clCs, CancellationToken);
            resolvedCs.Command.Should().NotBeNull();
            resolvedCs.Should().BeEquivalentTo(clCs, x => x.Excluding(x => x.Command));

            var resolvedVb = await client.TextDocument.ResolveCodeLens(clVb, CancellationToken);
            resolvedVb.Command.Should().NotBeNull();
            resolvedVb.Should().BeEquivalentTo(clVb, x => x.Excluding(x => x.Command));
        }


        [Fact]
        public async Task Should_Return_And_Aggregate_CodeLens_With_Different_Data_Models_But_Same_Selector()
        {
            var (client, server) = await Initialize(ConfigureClient, options => {
                options.AddTextDocumentIdentifier(WellKnownLanguages.CSharp, WellKnownLanguages.VisualBasic);
                options.OnCodeLens<Data>(
                    async (request, capability, ct) => {
                        return new CodeLensContainer<Data>(new CodeLens<Data>() {
                                Data = new Data() {
                                    Position = new Position(1, 1),
                                    Range = new Range(new Position(1, 1), new Position(1, 2))
                                }
                            },
                            new CodeLens<Data>() {
                                Data = new Data() {
                                    Position = new Position(2, 2),
                                    Range = new Range(new Position(2, 2), new Position(2, 3))
                                }
                            },
                            new CodeLens<Data>() {
                                Data = new Data() {
                                    Position = new Position(3, 3),
                                    Range = new Range(new Position(3, 3), new Position(3, 4))
                                }
                            });
                    },
                    async (request, capability, ct) => {
                        request.Data.Position.Should().BeEquivalentTo(new Position(2, 2));
                        request.Data.Range.Should().BeEquivalentTo(new Range(new Position(2, 2), new Position(2, 3)));
                        request.Data.Position = new Position(3, 3);

                        return new CodeLens<Data>() {
                            Command = new Command() {
                                Arguments = new JArray(),
                                Name = "my command",
                                Title = "My Command"
                            },
                            Data = request.Data,
                            Range = request.Range
                        };
                    },
                    new CodeLensRegistrationOptions() {
                        DocumentSelector = DocumentSelector.ForPattern("**/*.cs")
                    }
                );
                options.OnCodeLens<DataSecond>(
                    async (request, capability, ct) => {
                        return new CodeLensContainer<DataSecond>(new CodeLens<DataSecond>() {
                                Data = new DataSecond() {
                                    Id = "1",
                                }
                            },
                            new CodeLens<DataSecond>() {
                                Data = new DataSecond() {
                                    Id = "2",
                                }
                            },
                            new CodeLens<DataSecond>() {
                                Data = new DataSecond() {
                                    Id = "3",
                                }
                            });
                    },
                    async (request, capability, ct) => {
                        request.Data.Id.Should().Be("2");
                        return new CodeLens<DataSecond>() {
                            Command = new Command() {
                                Arguments = new JArray(),
                                Name = "my command",
                                Title = "My Command"
                            },
                            Data = request.Data,
                            Range = request.Range
                        };
                    },
                    new CodeLensRegistrationOptions() {
                        DocumentSelector = DocumentSelector.ForPattern("**/*.cs")
                    }
                );
            });

            var codeLensCs = await client.TextDocument.RequestCodeLens(new CodeLensParams<ResolvedData>() {
                TextDocument = new TextDocumentIdentifier(DocumentUri.File("/some/path/file.cs"))
            }, CancellationToken);

            codeLensCs.Should().HaveCount(6);

            codeLensCs.Should().Match(x => x.Any(z => z.Data.Data.Keys.Contains("position") && z.Data.Data.Keys.Contains("range")));
            codeLensCs.Should().Match(x => x.Any(z => z.Data.Data.Keys.Contains("id")));
        }


        [Fact]
        public async Task Should_Return_And_Resolve_CodeLens_Using_ClassHandler()
        {
            var (client, server) = await Initialize(ConfigureClient, options => {
                options.WithHandler(new DataCodeLensHandler(new CodeLensContainer<Data>(new CodeLens<Data>() {
                            Data = new Data() {
                                Position = new Position(1, 1),
                                Range = new Range(new Position(1, 1), new Position(1, 2))
                            }
                        },
                        new CodeLens<Data>() {
                            Data = new Data() {
                                Position = new Position(2, 2),
                                Range = new Range(new Position(2, 2), new Position(2, 3))
                            }
                        },
                        new CodeLens<Data>() {
                            Data = new Data() {
                                Position = new Position(3, 3),
                                Range = new Range(new Position(3, 3), new Position(3, 4))
                            }
                        }),
                    new CodeLensRegistrationOptions() {
                        DocumentSelector = DocumentSelector.ForLanguage("csharp")
                    }
                ));
            });

            var codeLens = await client.TextDocument.RequestCodeLens(new CodeLensParams<ResolvedData>() {
                TextDocument = new TextDocumentIdentifier(DocumentUri.File("/some/path/file.cs"))
            }, CancellationToken);

            codeLens.Should().HaveCount(3);

            var cl = codeLens.Skip(1).First();
            cl.Command.Should().BeNull();
            cl.Data.Data.Should().ContainKeys("position", "range");

            var resolved = await client.TextDocument.ResolveCodeLens(cl, CancellationToken);
            resolved.Command.Should().NotBeNull();
            resolved.Data.Data.Should().ContainKeys("position", "range");
        }

        [Fact]
        public async Task Should_Return_And_Resolve_CodeLens_With_Known_Model_ClassHandler()
        {
            var (client, server) = await Initialize(ConfigureClient, options => {
                options.WithHandler(new DataCodeLensHandler(new CodeLensContainer<Data>(new CodeLens<Data>() {
                            Data = new Data() {
                                Position = new Position(1, 1),
                                Range = new Range(new Position(1, 1), new Position(1, 2))
                            }
                        },
                        new CodeLens<Data>() {
                            Data = new Data() {
                                Position = new Position(2, 2),
                                Range = new Range(new Position(2, 2), new Position(2, 3))
                            }
                        },
                        new CodeLens<Data>() {
                            Data = new Data() {
                                Position = new Position(3, 3),
                                Range = new Range(new Position(3, 3), new Position(3, 4))
                            }
                        }),
                    new CodeLensRegistrationOptions() {
                        DocumentSelector = DocumentSelector.ForLanguage("csharp")
                    }
                ));
            });

            var codeLens = await client.TextDocument.RequestCodeLens(new CodeLensParams<ClientData>() {
                TextDocument = new TextDocumentIdentifier(DocumentUri.File("/some/path/file.cs"))
            }, CancellationToken);

            codeLens.Should().HaveCount(3);

            var cl = codeLens.Skip(1).First();
            cl.Command.Should().BeNull();
            cl.Data.Position.Should().BeEquivalentTo(new Position(2, 2));

            var resolved = await client.TextDocument.ResolveCodeLens(cl, CancellationToken);
            resolved.Command.Should().NotBeNull();
            resolved.Data.Position.Should().BeEquivalentTo(new Position(3, 3));
        }

        [Fact]
        public async Task Should_Return_And_Resolve_CodeLens_With_Different_Data_Models_ClassHandler()
        {
            var (client, server) = await Initialize(ConfigureClient, options => {
                options.AddTextDocumentIdentifier(WellKnownLanguages.CSharp, WellKnownLanguages.VisualBasic);

                options.WithHandler(new DataCodeLensHandler(new CodeLensContainer<Data>(new CodeLens<Data>() {
                            Data = new Data() {
                                Position = new Position(1, 1),
                                Range = new Range(new Position(1, 1), new Position(1, 2))
                            }
                        },
                        new CodeLens<Data>() {
                            Data = new Data() {
                                Position = new Position(2, 2),
                                Range = new Range(new Position(2, 2), new Position(2, 3))
                            }
                        },
                        new CodeLens<Data>() {
                            Data = new Data() {
                                Position = new Position(3, 3),
                                Range = new Range(new Position(3, 3), new Position(3, 4))
                            }
                        }),
                    new CodeLensRegistrationOptions() {
                        DocumentSelector =  DocumentSelector.ForPattern("**/*.cs")
                    }
                ));
                options.WithHandler(new DataSecondCodeLensHandler(new CodeLensContainer<DataSecond>(new CodeLens<DataSecond>() {
                                Data = new DataSecond() {
                                    Id = "1",
                                }
                            },
                            new CodeLens<DataSecond>() {
                                Data = new DataSecond() {
                                    Id = "2",
                                }
                            },
                            new CodeLens<DataSecond>() {
                                Data = new DataSecond() {
                                    Id = "3",
                                }
                            }),
                    new CodeLensRegistrationOptions() {
                        DocumentSelector = DocumentSelector.ForPattern("**/*.vb")
                    }
                ));
            });

            var codeLensCs = await client.TextDocument.RequestCodeLens(new CodeLensParams<ResolvedData>() {
                TextDocument = new TextDocumentIdentifier(DocumentUri.File("/some/path/file.cs"))
            }, CancellationToken);
            var codeLensVb = await client.TextDocument.RequestCodeLens(new CodeLensParams<ResolvedData>() {
                TextDocument = new TextDocumentIdentifier(DocumentUri.File("/some/path/file.vb"))
            }, CancellationToken);

            codeLensCs.Should().HaveCount(3);
            codeLensVb.Should().HaveCount(3);
            codeLensCs.Should().NotBeEquivalentTo(codeLensVb);

            var clCs = codeLensCs.Skip(1).First();
            clCs.Command.Should().BeNull();
            clCs.Data.Data.Should().ContainKeys("position", "range");

            var clVb = codeLensVb.Skip(1).First();
            clVb.Command.Should().BeNull();
            clVb.Data.Data.Should().ContainKeys("id");

            var resolvedCs = await client.TextDocument.ResolveCodeLens(clCs, CancellationToken);
            resolvedCs.Command.Should().NotBeNull();
            resolvedCs.Should().BeEquivalentTo(clCs, x => x.Excluding(x => x.Command));

            var resolvedVb = await client.TextDocument.ResolveCodeLens(clVb, CancellationToken);
            resolvedVb.Command.Should().NotBeNull();
            resolvedVb.Should().BeEquivalentTo(clVb, x => x.Excluding(x => x.Command));
        }


        [Fact]
        public async Task Should_Return_And_Aggregate_CodeLens_With_Different_Data_Models_But_Same_Selector_ClassHandler()
        {
            var (client, server) = await Initialize(ConfigureClient, options => {
                options.AddTextDocumentIdentifier(WellKnownLanguages.CSharp, WellKnownLanguages.VisualBasic);

                options.WithHandler(new DataCodeLensHandler(new CodeLensContainer<Data>(new CodeLens<Data>() {
                            Data = new Data() {
                                Position = new Position(1, 1),
                                Range = new Range(new Position(1, 1), new Position(1, 2))
                            }
                        },
                        new CodeLens<Data>() {
                            Data = new Data() {
                                Position = new Position(2, 2),
                                Range = new Range(new Position(2, 2), new Position(2, 3))
                            }
                        },
                        new CodeLens<Data>() {
                            Data = new Data() {
                                Position = new Position(3, 3),
                                Range = new Range(new Position(3, 3), new Position(3, 4))
                            }
                        }),
                    new CodeLensRegistrationOptions() {
                        DocumentSelector =  DocumentSelector.ForPattern("**/*.cs")
                    }
                ));
                options.WithHandler(new DataSecondCodeLensHandler(new CodeLensContainer<DataSecond>(new CodeLens<DataSecond>() {
                                Data = new DataSecond() {
                                    Id = "1",
                                }
                            },
                            new CodeLens<DataSecond>() {
                                Data = new DataSecond() {
                                    Id = "2",
                                }
                            },
                            new CodeLens<DataSecond>() {
                                Data = new DataSecond() {
                                    Id = "3",
                                }
                            }),
                    new CodeLensRegistrationOptions() {
                        DocumentSelector = DocumentSelector.ForPattern("**/*.cs")
                    }
                ));
            });

            var codeLensCs = await client.TextDocument.RequestCodeLens(new CodeLensParams<ResolvedData>() {
                TextDocument = new TextDocumentIdentifier(DocumentUri.File("/some/path/file.cs"))
            }, CancellationToken);

            codeLensCs.Should().HaveCount(6);

            codeLensCs.Should().Match(x => x.Any(z => z.Data.Data.Keys.Contains("position") && z.Data.Data.Keys.Contains("range")));
            codeLensCs.Should().Match(x => x.Any(z => z.Data.Data.Keys.Contains("id")));
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

        class DataCodeLensHandler : CodeLensHandler<Data>
        {
            private readonly CodeLensContainer<Data> _container;

            public DataCodeLensHandler(CodeLensContainer<Data> container, CodeLensRegistrationOptions registrationOptions) : base(registrationOptions)
            {
                _container = container;
                registrationOptions.ResolveProvider = true;
            }

            public override async Task<CodeLensContainer<Data>> Handle(CodeLensParams<Data> request, CancellationToken cancellationToken) => _container;

            public override async Task<CodeLens<Data>> Handle(CodeLens<Data> request, CancellationToken cancellationToken)
            {
                request.Data.Position.Should().BeEquivalentTo(new Position(2, 2));
                request.Data.Range.Should().BeEquivalentTo(new Range(new Position(2, 2), new Position(2, 3)));
                request.Data.Position = new Position(3, 3);

                return new CodeLens<Data>() {
                    Command = new Command() {
                        Arguments = new JArray(),
                        Name = "my command",
                        Title = "My Command"
                    },
                    Data = request.Data,
                    Range = request.Range
                };
            }
        }

        class DataSecondCodeLensHandler : CodeLensHandler<DataSecond>
        {
            private readonly CodeLensContainer<DataSecond> _container;

            public DataSecondCodeLensHandler(CodeLensContainer<DataSecond> container, CodeLensRegistrationOptions registrationOptions) : base(registrationOptions)
            {
                _container = container;
                registrationOptions.ResolveProvider = true;
            }

            public override async Task<CodeLensContainer<DataSecond>> Handle(CodeLensParams<DataSecond> request, CancellationToken cancellationToken) => _container;

            public override async Task<CodeLens<DataSecond>> Handle(CodeLens<DataSecond> request, CancellationToken cancellationToken)
            {
                request.Data.Id.Should().Be("2");
                return new CodeLens<DataSecond>() {
                    Command = new Command() {
                        Arguments = new JArray(),
                        Name = "my command",
                        Title = "My Command"
                    },
                    Data = request.Data,
                    Range = request.Range
                };
            }
        }

        class DataSecond : CanBeResolvedData
        {
            public string Id { get; set; }
        }

        class ClientData : CanBeResolvedData
        {
            public Position Position { get; set; }
        }
    }
}
