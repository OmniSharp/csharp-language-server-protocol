using System;
using System.Collections.Generic;
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
using OmniSharp.Extensions.LanguageServer.Protocol.Document.Proposals;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals;
using Serilog.Events;
using TestingUtils;
using Xunit;
using Xunit.Abstractions;

namespace Lsp.Tests.Integration
{
    public class TypedCallHierarchyTests : LanguageProtocolTestBase
    {
        public TypedCallHierarchyTests(ITestOutputHelper outputHelper) : base(new JsonRpcTestOptions().ConfigureForXUnit(outputHelper, LogEventLevel.Verbose))
        {
        }

        [RetryFact]
        public async Task Should_Aggregate_With_All_Related_Handlers()
        {
            var incomingHandlerA = Substitute.For<Func<CallHierarchyIncomingCallsParams<Data>, Task<Container<CallHierarchyIncomingCall>?>>>();
            var incomingHandlerB = Substitute.For<Func<CallHierarchyIncomingCallsParams<Nested>, Task<Container<CallHierarchyIncomingCall>?>>>();
            var outgoingHandlerA = Substitute.For<Func<CallHierarchyOutgoingCallsParams<Data>, Task<Container<CallHierarchyOutgoingCall>?>>>();
            var outgoingHandlerB = Substitute.For<Func<CallHierarchyOutgoingCallsParams<Nested>, Task<Container<CallHierarchyOutgoingCall>?>>>();
            var (client, _) = await Initialize(
                options => { options.EnableAllCapabilities(); }, options => {
                    var identifier = Substitute.For<ITextDocumentIdentifier>();
                    identifier.GetTextDocumentAttributes(Arg.Any<DocumentUri>()).Returns(
                        call => new TextDocumentAttributes(call.ArgAt<DocumentUri>(0), "file", "csharp")
                    );
                    options.AddTextDocumentIdentifier(identifier);

                    options.OnCallHierarchy(
                        @params => Task.FromResult(
                            new Container<CallHierarchyItem<Data>?>(
                                new CallHierarchyItem<Data> {
                                    Name = "Test",
                                    Kind = SymbolKind.Boolean,
                                    Data = new Data {
                                        Child = new Nested {
                                            Date = DateTimeOffset.MinValue
                                        },
                                        Id = Guid.NewGuid(),
                                        Name = "name"
                                    }
                                }
                            )
                        )!,
                        incomingHandlerA,
                        outgoingHandlerA,
                        (_, _) => new() {
                            DocumentSelector = DocumentSelector.ForPattern("**/*.cs")
                        }
                    );

                    options.OnCallHierarchy(
                        @params => Task.FromResult(
                            new Container<CallHierarchyItem<Nested>?>(
                                new CallHierarchyItem<Nested> {
                                    Name = "Test Nested",
                                    Kind = SymbolKind.Constant,
                                    Data = new Nested {
                                        Date = DateTimeOffset.MinValue
                                    }
                                }
                            )
                        )!,
                        incomingHandlerB,
                        outgoingHandlerB,
                        (_, _) => new() {
                            DocumentSelector = DocumentSelector.ForPattern("**/*.cs")
                        }
                    );
                }
            );

            var items = await client.RequestCallHierarchyPrepare(
                new CallHierarchyPrepareParams() {
                    TextDocument = new TextDocumentIdentifier("/some/path/file.cs"),
                }
            );

            var lens = items.ToArray();
            lens.Select(z => z.Name).Should().Contain(new[] { "Test", "Test Nested" });

            var incomingItems = await Task.WhenAll(lens.Select(z => client.RequestCallHierarchyIncoming(new CallHierarchyIncomingCallsParams() { Item = z }).AsTask()));
            incomingHandlerA.Received(1).Invoke(Arg.Any<CallHierarchyIncomingCallsParams<Data>>());
            incomingHandlerB.Received(1).Invoke(Arg.Any<CallHierarchyIncomingCallsParams<Nested>>());
            var outgoingItems = await Task.WhenAll(lens.Select(z => client.RequestCallHierarchyOutgoing(new CallHierarchyOutgoingCallsParams() { Item = z }).AsTask()));
            outgoingHandlerA.Received(1).Invoke(Arg.Any<CallHierarchyOutgoingCallsParams<Data>>());
            outgoingHandlerB.Received(1).Invoke(Arg.Any<CallHierarchyOutgoingCallsParams<Nested>>());
        }

        [RetryFact]
        public async Task Should_Resolve_With_Data_Capability()
        {
            var incomingHandler = Substitute.For<Func<CallHierarchyIncomingCallsParams<Data>, Task<Container<CallHierarchyIncomingCall>?>>>();
            var outgoingHandler = Substitute.For<Func<CallHierarchyOutgoingCallsParams<Data>, Task<Container<CallHierarchyOutgoingCall>?>>>();
            var (client, _) = await Initialize(
                options => { options.EnableAllCapabilities(); }, options => {
                    options.OnCallHierarchy(
                        @params => Task.FromResult(
                            new Container<CallHierarchyItem<Data>?>(
                                new CallHierarchyItem<Data> {
                                    Name = "Test",
                                    Kind = SymbolKind.Boolean,
                                    Data = new Data {
                                        Child = new Nested {
                                            Date = DateTimeOffset.MinValue
                                        },
                                        Id = Guid.NewGuid(),
                                        Name = "name"
                                    }
                                }
                            )
                        )!,
                        incomingHandler,
                        outgoingHandler,
                        (_, _) => new() {
                            DocumentSelector = DocumentSelector.ForPattern("**/*.cs")
                        }
                    );
                }
            );

            var items = await client.RequestCallHierarchyPrepare(
                new CallHierarchyPrepareParams() {
                    TextDocument = new TextDocumentIdentifier("/some/path/file.cs"),
                }
            );

            var item = items.Single();

            var incomingItems = await client.RequestCallHierarchyIncoming(new CallHierarchyIncomingCallsParams() { Item = item });
            incomingHandler.Received(1).Invoke(Arg.Any<CallHierarchyIncomingCallsParams<Data>>());
            var outgoingItems = await client.RequestCallHierarchyOutgoing(new CallHierarchyOutgoingCallsParams() { Item = item });
            outgoingHandler.Received(1).Invoke(Arg.Any<CallHierarchyOutgoingCallsParams<Data>>());
        }

        [RetryFact]
        public async Task Should_Resolve_With_Partial_Data_Capability()
        {
            var incomingHandler = Substitute.For<Action<CallHierarchyIncomingCallsParams<Data>, IObserver<IEnumerable<CallHierarchyIncomingCall>>>>();
            var outgoingHandler = Substitute.For<Action<CallHierarchyOutgoingCallsParams<Data>, IObserver<IEnumerable<CallHierarchyOutgoingCall>>>>();
            var (client, _) = await Initialize(
                options => { }, options => {
                    options.OnCallHierarchy<Data>(
                        (completionParams, observer) => {
                            var a = new Container<CallHierarchyItem<Data>?>(
                                new Container<CallHierarchyItem<Data>?>(
                                    new CallHierarchyItem<Data> {
                                        Name = "Test",
                                        Kind = SymbolKind.Boolean,
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

                            observer.OnNext(a);
                            observer.OnCompleted();
                        },
                        (a, b) => {
                            incomingHandler(a, b);
                            b.OnNext(Enumerable.Empty<CallHierarchyIncomingCall>());
                            b.OnCompleted();
                        },
                        (a, b) => {
                            outgoingHandler(a, b);
                            b.OnNext(Enumerable.Empty<CallHierarchyOutgoingCall>());
                            b.OnCompleted();
                        },
                        (_, _) => new()
                    );
                }
            );

            var items = await client
                             .RequestCallHierarchyPrepare(new CallHierarchyPrepareParams() { TextDocument = new TextDocumentIdentifier("/some/path/file.cs"), })
                             .Take(1)
                             .ToTask(CancellationToken);

            var item = items.Single();

            var incomingItems = await client
                                     .RequestCallHierarchyIncoming(new CallHierarchyIncomingCallsParams() { Item = item })
                                     .Take(1)
                                     .ToTask(CancellationToken);
            incomingHandler.Received(1).Invoke(Arg.Any<CallHierarchyIncomingCallsParams<Data>>(), Arg.Any<IObserver<IEnumerable<CallHierarchyIncomingCall>>>());
            var outgoingItems = await client
                                     .RequestCallHierarchyOutgoing(new CallHierarchyOutgoingCallsParams() { Item = item })
                                     .Take(1)
                                     .ToTask(CancellationToken);
            outgoingHandler.Received(1).Invoke(Arg.Any<CallHierarchyOutgoingCallsParams<Data>>(), Arg.Any<IObserver<IEnumerable<CallHierarchyOutgoingCall>>>());
        }
    }
}
