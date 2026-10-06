using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Shared;
using OmniSharp.Extensions.LanguageServer.Server.Matchers;
using OmniSharp.Extensions.LanguageServer.Server.Pipelines;
using TestingUtils;
using Xunit;
using Xunit.Abstractions;

namespace Lsp.Tests.Matchers
{
    public class ResolveCommandMatcherTests : AutoTestBase
    {
        private readonly Guid _trueId = Guid.NewGuid();
        private readonly Guid _falseId = Guid.Empty;

        public ResolveCommandMatcherTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Fact]
        public void Should_Not_Return_Null()
        {
            // Given
            var handlerDescriptors = Enumerable.Empty<ILspHandlerDescriptor>();
            var handlerMatcher = AutoSubstitute.Resolve<ResolveCommandMatcher>();

            // When
            var result = handlerMatcher.FindHandler(1, handlerDescriptors);

            // Then
            result.Should().NotBeNull();
        }

        [Fact]
        public void Should_Return_Empty_Descriptor()
        {
            // Given
            var handlerDescriptors = Enumerable.Empty<ILspHandlerDescriptor>();
            var handlerMatcher = AutoSubstitute.Resolve<ResolveCommandMatcher>();

            // When
            var result = handlerMatcher.FindHandler(1, handlerDescriptors);

            // Then
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task Should_Not_Throw_Given_Another_Descriptor()
        {
            // Given
            var resolveHandler = Substitute.For<ICodeLensHandler>();
            var handlerDescriptor = new LspHandlerDescriptor(
                0,
                TextDocumentNames.CodeLens,
                "Key",
                resolveHandler,
                resolveHandler.GetType(),
                typeof(CodeLensParams),
                null,
                null,
                null,
                null,
                null,
                () => { },
                Substitute.For<ILspHandlerTypeDescriptor>()
            );
            var handlerMatcher = new ResolveCommandPipeline<CodeLensParams, CodeLensContainer>(
                new RequestContext { Descriptor = handlerDescriptor },
                LoggerFactory.CreateLogger<ResolveCommandPipeline<CodeLensParams, CodeLensContainer>>()
            );

            // When
            Func<Task> a = async () => await handlerMatcher.Handle(new CodeLensParams(), () => Task.FromResult(new CodeLensContainer()), CancellationToken.None);
            await a.Should().NotThrowAsync();
        }

        [Fact]
        public void Should_Return_CodeLensResolve_Descriptor()
        {
            // Given
            var handlerMatcher = AutoSubstitute.Resolve<ResolveCommandMatcher>();
            var resolveHandler = (ICodeLensResolveHandler) Substitute.For(new[] { typeof(ICodeLensResolveHandler), typeof(ICanBeIdentifiedHandler) }, Array.Empty<object>());
            var resolveHandler2 = (ICodeLensResolveHandler) Substitute.For(new[] { typeof(ICodeLensResolveHandler), typeof(ICanBeIdentifiedHandler) }, Array.Empty<object>());
            resolveHandler.Id.Returns(_falseId);
            resolveHandler2.Id.Returns(_trueId);

            // When
            var result = handlerMatcher.FindHandler(
                                            new CodeLens {
                                                Data = JObject.FromObject(new Dictionary<string, object> { [Constants.PrivateHandlerId] = _trueId, ["a"] = 1 })
                                            },
                                            new List<LspHandlerDescriptor> {
                                                new LspHandlerDescriptor(
                                                    0,
                                                    TextDocumentNames.CodeLensResolve,
                                                    "Key",
                                                    resolveHandler!,
                                                    resolveHandler.GetType(),
                                                    typeof(CodeLens),
                                                    null,
                                                    null,
                                                    null,
                                                    null,
                                                    null,
                                                    () => { },
                                                    Substitute.For<ILspHandlerTypeDescriptor>()
                                                ),
                                                new LspHandlerDescriptor(
                                                    0,
                                                    TextDocumentNames.CodeLensResolve,
                                                    "Key2",
                                                    resolveHandler2,
                                                    typeof(ICodeLensResolveHandler),
                                                    typeof(CodeLens),
                                                    null,
                                                    null,
                                                    null,
                                                    null,
                                                    null,
                                                    () => { },
                                                    Substitute.For<ILspHandlerTypeDescriptor>()
                                                ),
                                            }
                                        )
                                       .ToArray();

            // Then
            result.Should().NotBeNullOrEmpty();
            result.OfType<IHandlerDescriptor>().Should().Contain(x => x.Handler == resolveHandler2);
        }

        [Fact]
        public void Should_Handle_Null_Data()
        {
            // Given
            var handlerMatcher = AutoSubstitute.Resolve<ResolveCommandMatcher>();
            var resolveHandler = Substitute.For<ICompletionResolveHandler>();

            // When
            var result = handlerMatcher.FindHandler(
                                            new CompletionItem(),
                                            new List<LspHandlerDescriptor> {
                                                new LspHandlerDescriptor(
                                                    0,
                                                    TextDocumentNames.CompletionResolve,
                                                    "Key",
                                                    resolveHandler,
                                                    resolveHandler.GetType(),
                                                    typeof(CompletionItem),
                                                    null,
                                                    null,
                                                    null,
                                                    null,
                                                    null,
                                                    () => { },
                                                    Substitute.For<ILspHandlerTypeDescriptor>()
                                                ),
                                            }
                                        )
                                       .ToArray();

            // Then
            result.Should().BeNullOrEmpty();
        }

        [Fact]
        public void Should_Return_CompletionResolve_Descriptor()
        {
            // Given
            var handlerMatcher = AutoSubstitute.Resolve<ResolveCommandMatcher>();
            var resolveHandler = Substitute.For(new[] { typeof(ICompletionResolveHandler), typeof(ICanBeIdentifiedHandler) }, Array.Empty<object>()) as ICompletionResolveHandler;
            var resolveHandler2 = Substitute.For(new[] { typeof(ICompletionResolveHandler), typeof(ICanBeIdentifiedHandler) }, Array.Empty<object>()) as ICompletionResolveHandler;
            ( (ICanBeIdentifiedHandler) resolveHandler! ).Id.Returns(_falseId);
            ( (ICanBeIdentifiedHandler) resolveHandler2! ).Id.Returns(_trueId);

            // When
            var result = handlerMatcher.FindHandler(
                                            new CompletionItem {
                                                Data = JObject.FromObject(new Dictionary<string, object> { [Constants.PrivateHandlerId] = _trueId, ["a"] = 1 })
                                            },
                                            new List<LspHandlerDescriptor> {
                                                new LspHandlerDescriptor(
                                                    0,
                                                    TextDocumentNames.CompletionResolve,
                                                    "Key",
                                                    resolveHandler!,
                                                    resolveHandler.GetType(),
                                                    typeof(CompletionItem),
                                                    null,
                                                    null,
                                                    null,
                                                    null,
                                                    null,
                                                    () => { },
                                                    Substitute.For<ILspHandlerTypeDescriptor>()
                                                ),
                                                new LspHandlerDescriptor(
                                                    0,
                                                    TextDocumentNames.CompletionResolve,
                                                    "Key2",
                                                    resolveHandler2!,
                                                    typeof(ICompletionResolveHandler),
                                                    typeof(CompletionItem),
                                                    null,
                                                    null,
                                                    null,
                                                    null,
                                                    null,
                                                    () => { },
                                                    Substitute.For<ILspHandlerTypeDescriptor>()
                                                ),
                                            }
                                        )
                                       .ToArray();

            // Then
            result.Should().NotBeNullOrEmpty();
            result.OfType<IHandlerDescriptor>().Should().Contain(x => x.Handler == resolveHandler2);
        }

        [Fact]
        public async Task Should_Update_CompletionItems_With_HandlerType()
        {
            // Given
            var resolveHandler = Substitute.For(
                new[] {
                    typeof(ICompletionHandler),
                    typeof(ICompletionResolveHandler),
                    typeof(ICanBeIdentifiedHandler)
                }, new object[0]
            );
            ( resolveHandler as ICanBeIdentifiedHandler )?.Id.Returns(_trueId);
            var descriptor = new LspHandlerDescriptor(
                0,
                TextDocumentNames.Completion,
                "Key",
                ( resolveHandler as IJsonRpcHandler )!,
                resolveHandler.GetType(),
                typeof(CompletionParams),
                null,
                null,
                null,
                null,
                null,
                () => { },
                Substitute.For<ILspHandlerTypeDescriptor>()
            );
            var handlerMatcher = new ResolveCommandPipeline<CompletionParams, CompletionList>(
                new RequestContext { Descriptor = descriptor },
                Substitute.For<ILogger<ResolveCommandPipeline<CompletionParams, CompletionList>>>()
            );

            var item = new CompletionItem {
                Data = JObject.FromObject(new { hello = "world" })
            };
            var list = new CompletionList(item);

            // ReSharper disable once IsExpressionAlwaysTrue
            ( list is IEnumerable<ICanBeResolved> ).Should().BeTrue();

            // When
            var response = await handlerMatcher.Handle(new CompletionParams(), () => Task.FromResult(list), CancellationToken.None);

            // Then
            response.Should().BeEquivalentTo(list, x => x.UsingStructuralRecordEquality());
            response.Items.Should().Contain(item);
            var responseItem = response.Items.First();
            responseItem.Data![Constants.PrivateHandlerId].Value<Guid>().Should().Be(_trueId);
            responseItem.Data!["hello"].Value<string>().Should().Be("world");
        }

        [Fact]
        public async Task Should_Update_CodeLensContainer_With_HandlerType()
        {
            // Given
            var resolveHandler = Substitute.For(
                new[] {
                    typeof(ICodeLensHandler),
                    typeof(ICodeLensResolveHandler),
                    typeof(ICanBeIdentifiedHandler)
                }, new object[0]
            );
            ( resolveHandler as ICanBeIdentifiedHandler )?.Id.Returns(_trueId);
            var descriptor = new LspHandlerDescriptor(
                0,
                TextDocumentNames.CodeLens,
                "Key",
                ( resolveHandler as IJsonRpcHandler )!,
                resolveHandler.GetType(),
                typeof(CodeLensParams),
                null,
                null,
                null,
                null,
                null,
                () => { },
                Substitute.For<ILspHandlerTypeDescriptor>()
            );
            var handlerMatcher = new ResolveCommandPipeline<CodeLensParams, CodeLensContainer>(
                new RequestContext { Descriptor = descriptor },
                Substitute.For<ILogger<ResolveCommandPipeline<CodeLensParams, CodeLensContainer>>>()
            );

            var item = new CodeLens {
                Data = JObject.FromObject(new { hello = "world" })
            };
            var list = new CodeLensContainer(item);

            // ReSharper disable once IsExpressionAlwaysTrue
            ( list is IEnumerable<ICanBeResolved> ).Should().BeTrue();

            // When
            var response = await handlerMatcher.Handle(new CodeLensParams(), () => Task.FromResult(list), CancellationToken.None);

            // Then
            response.Should().BeEquivalentTo(list, x => x.UsingStructuralRecordEquality());
            response.Should().Contain(item);
            var responseItem = response.First();
            responseItem.Data![Constants.PrivateHandlerId].Value<Guid>().Should().Be(_trueId);
            responseItem.Data["hello"].Value<string>().Should().Be("world");
        }

        [Fact]
        public async Task Should_Update_CodeLens_Removing_HandlerType()
        {
            // Given
            var resolveHandler = Substitute.For(
                new[] {
                    typeof(ICodeLensHandler),
                    typeof(ICodeLensResolveHandler),
                    typeof(ICanBeIdentifiedHandler)
                }, new object[0]
            );
            ( resolveHandler as ICanBeIdentifiedHandler )?.Id.Returns(_falseId);
            var descriptor = new LspHandlerDescriptor(
                0,
                TextDocumentNames.CodeLensResolve,
                "Key",
                ( resolveHandler as IJsonRpcHandler )!,
                resolveHandler.GetType(),
                typeof(CodeLens),
                null,
                null,
                null,
                null,
                null,
                () => { },
                Substitute.For<ILspHandlerTypeDescriptor>()
            );
            var handlerMatcher = new ResolveCommandPipeline<CodeLens, CodeLens>(
                new RequestContext { Descriptor = descriptor },
                Substitute.For<ILogger<ResolveCommandPipeline<CodeLens, CodeLens>>>()
            );

            var item = new CodeLens {
                Data = JObject.FromObject(new { hello = "world" })
            };

            // When
            var response = await handlerMatcher.Handle(item, () => Task.FromResult(item), CancellationToken.None);

            // Then
            response.Should().BeEquivalentTo(item, x => x.UsingStructuralRecordEquality());
            var data = item.Data.Should().BeOfType<JObject>().Subject;
            data.Should().NotContainKey(Constants.PrivateHandlerId);
            data["hello"].Value<string>().Should().Be("world");
        }
    }
}
