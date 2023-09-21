using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using FluentAssertions;
using Lsp.Integration.Tests.Fixtures;
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
    public class TypedTypeHierarchyTests : LanguageProtocolTestBase
    {
        public TypedTypeHierarchyTests(ITestOutputHelper outputHelper) : base(new JsonRpcTestOptions().ConfigureForXUnit(outputHelper, LogEventLevel.Verbose))
        {
        }

        [Fact]
        public async Task Should_Aggregate_With_All_Related_Handlers()
        {
            var subtypeHandlerA = Substitute.For<Func<TypeHierarchySubtypesParams<Data>, Task<Container<TypeHierarchyItem>?>>>();
            var subtypeHandlerB = Substitute.For<Func<TypeHierarchySubtypesParams<Nested>, Task<Container<TypeHierarchyItem>?>>>();
            var supertypeHandlerA = Substitute.For<Func<TypeHierarchySupertypesParams<Data>, Task<Container<TypeHierarchyItem>?>>>();
            var supertypeHandlerB = Substitute.For<Func<TypeHierarchySupertypesParams<Nested>, Task<Container<TypeHierarchyItem>?>>>();
            var (client, _) = await Initialize(
                options => { options.EnableAllCapabilities(); }, options =>
                {
                    var identifier = Substitute.For<ITextDocumentIdentifier>();
                    identifier.GetTextDocumentAttributes(Arg.Any<DocumentUri>()).Returns(
                        call => new TextDocumentAttributes(call.ArgAt<DocumentUri>(0), "file", "csharp")
                    );
                    options.AddTextDocumentIdentifier(identifier);

                    options.OnTypeHierarchy(
                        @params => Task.FromResult(
                            new Container<TypeHierarchyItem<Data>?>(
                                new TypeHierarchyItem<Data>
                                {
                                    Name = "Test",
                                    Kind = SymbolKind.Boolean,
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
                        )!,
                        supertypeHandlerA,
                        subtypeHandlerA,
                        (_, _) => new()
                        {
                            DocumentSelector = TextDocumentSelector.ForPattern("**/*.cs")
                        }
                    );

                    options.OnTypeHierarchy(
                        @params => Task.FromResult(
                            new Container<TypeHierarchyItem<Nested>?>(
                                new TypeHierarchyItem<Nested>
                                {
                                    Name = "Test Nested",
                                    Kind = SymbolKind.Constant,
                                    Data = new Nested
                                    {
                                        Date = DateTimeOffset.MinValue
                                    }
                                }
                            )
                        )!,
                        supertypeHandlerB,
                        subtypeHandlerB,
                        (_, _) => new()
                        {
                            DocumentSelector = TextDocumentSelector.ForPattern("**/*.cs")
                        }
                    );
                }
            );

            var items = await client.RequestTypeHierarchyPrepare(
                new TypeHierarchyPrepareParams
                {
                    TextDocument = new TextDocumentIdentifier("/some/path/file.cs"),
                }
            );

            var lens = items.ToArray();
            lens.Select(z => z.Name).Should().Contain(new[] { "Test", "Test Nested" });

            var subtypeItems = await Task.WhenAll(
                lens.Select(z => client.RequestTypeHierarchySubtypes(new TypeHierarchySubtypesParams { Item = z }).AsTask())
            );
            subtypeHandlerA.Received(1).Invoke(Arg.Any<TypeHierarchySubtypesParams<Data>>());
            subtypeHandlerB.Received(1).Invoke(Arg.Any<TypeHierarchySubtypesParams<Nested>>());
            var supertypeItems = await Task.WhenAll(
                lens.Select(z => client.RequestTypeHierarchySupertypes(new TypeHierarchySupertypesParams { Item = z }).AsTask())
            );
            supertypeHandlerA.Received(1).Invoke(Arg.Any<TypeHierarchySupertypesParams<Data>>());
            supertypeHandlerB.Received(1).Invoke(Arg.Any<TypeHierarchySupertypesParams<Nested>>());
        }

        [Fact]
        public async Task Should_Resolve_With_Data_Capability()
        {
            var subtypeHandler = Substitute.For<Func<TypeHierarchySubtypesParams<Data>, Task<Container<TypeHierarchyItem>?>>>();
            var supertypeHandler = Substitute.For<Func<TypeHierarchySupertypesParams<Data>, Task<Container<TypeHierarchyItem>?>>>();
            var (client, _) = await Initialize(
                options => { options.EnableAllCapabilities(); }, options =>
                {
                    options.OnTypeHierarchy(
                        @params => Task.FromResult(
                            new Container<TypeHierarchyItem<Data>?>(
                                new TypeHierarchyItem<Data>
                                {
                                    Name = "Test",
                                    Kind = SymbolKind.Boolean,
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
                        )!,
                        supertypeHandler,
                        subtypeHandler,
                        (_, _) => new()
                        {
                            DocumentSelector = TextDocumentSelector.ForPattern("**/*.cs")
                        }
                    );
                }
            );

            var items = await client.RequestTypeHierarchyPrepare(
                new TypeHierarchyPrepareParams
                {
                    TextDocument = new TextDocumentIdentifier("/some/path/file.cs"),
                }
            );

            var item = items.Single();

            var subtypeItems = await client.RequestTypeHierarchySubtypes(new TypeHierarchySubtypesParams { Item = item });
            subtypeHandler.Received(1).Invoke(Arg.Any<TypeHierarchySubtypesParams<Data>>());
            var supertypeItems = await client.RequestTypeHierarchySupertypes(new TypeHierarchySupertypesParams { Item = item });
            supertypeHandler.Received(1).Invoke(Arg.Any<TypeHierarchySupertypesParams<Data>>());
        }

        [Fact]
        public async Task Should_Resolve_With_Partial_Data_Capability()
        {
            var subtypeHandler = Substitute.For<Action<TypeHierarchySubtypesParams<Data>, IObserver<IEnumerable<TypeHierarchyItem>>>>();
            var supertypeHandler = Substitute.For<Action<TypeHierarchySupertypesParams<Data>, IObserver<IEnumerable<TypeHierarchyItem>>>>();
            var (client, _) = await Initialize(
                options => { }, options =>
                {
                    options.OnTypeHierarchy<Data>(
                        (completionParams, observer) =>
                        {
                            var a = new Container<TypeHierarchyItem<Data>?>(
                                new Container<TypeHierarchyItem<Data>?>(
                                    new TypeHierarchyItem<Data>
                                    {
                                        Name = "Test",
                                        Kind = SymbolKind.Boolean,
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

                            observer.OnNext(a);
                            observer.OnCompleted();
                        },
                        (a, b) =>
                        {
                            supertypeHandler(a, b);
                            b.OnNext(Enumerable.Empty<TypeHierarchyItem>());
                            b.OnCompleted();
                        },
                        (a, b) =>
                        {
                            subtypeHandler(a, b);
                            b.OnNext(Enumerable.Empty<TypeHierarchyItem>());
                            b.OnCompleted();
                        },
                        (_, _) => new()
                    );
                }
            );

            var items = await client
                             .RequestTypeHierarchyPrepare(new TypeHierarchyPrepareParams { TextDocument = new TextDocumentIdentifier("/some/path/file.cs"), })
                             .Take(1)
                             .ToTask(CancellationToken);

            var item = items.Single();

            var subtypeItems = await client
                                     .RequestTypeHierarchySubtypes(new TypeHierarchySubtypesParams { Item = item })
                                     .Take(1)
                                     .ToTask(CancellationToken);
            subtypeHandler.Received(1).Invoke(Arg.Any<TypeHierarchySubtypesParams<Data>>(), Arg.Any<IObserver<IEnumerable<TypeHierarchyItem>>>());
            var supertypeItems = await client
                                     .RequestTypeHierarchySupertypes(new TypeHierarchySupertypesParams { Item = item })
                                     .Take(1)
                                     .ToTask(CancellationToken);
            supertypeHandler.Received(1).Invoke(Arg.Any<TypeHierarchySupertypesParams<Data>>(), Arg.Any<IObserver<IEnumerable<TypeHierarchyItem>>>());
        }
    }
}
