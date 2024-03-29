using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Lsp.Integration.Tests.Fixtures;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc.Testing;
using OmniSharp.Extensions.LanguageProtocol.Testing;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Server;
using TestingUtils;
using Xunit;
using Xunit.Abstractions;

namespace Lsp.Integration.Tests
{
    public static class DynamicRegistration
    {
        public class DynamicRegistrationTests : LanguageProtocolTestBase
        {
            [Fact]
            public async Task Should_Register_Dynamically_After_Initialization()
            {
                var (client, _) = await Initialize(new ConfigureClient().Configure, new ConfigureServer().Configure);
                await client.RegistrationManager.Registrations.Take(1);
                client.ServerSettings.Capabilities.CompletionProvider.Should().BeNull();

                await TestHelper.DelayUntil(
                    () => client.RegistrationManager.CurrentRegistrations,
                    registrations => registrations
                       .Any(x => x.Method == TextDocumentNames.Completion && SelectorMatches(x, z => z.HasLanguage && z.Language == "csharp")),
                    CancellationToken
                );

                client.RegistrationManager.CurrentRegistrations.Should()
                      .Contain(
                           x =>
                               x.Method == TextDocumentNames.Completion &&
                               SelectorMatches(x, z => z.HasLanguage && z.Language == "csharp")
                       );
            }

            [Fact]
            public async Task Should_Register_Dynamically_While_Server_Is_Running()
            {
                var (client, server) = await Initialize(new ConfigureClient().Configure, new ConfigureServer().Configure);
                await client.RegistrationManager.Registrations.Take(1);
                client.ServerSettings.Capabilities.CompletionProvider.Should().BeNull();

                using var _ = server.Register(
                    x => x
                       .OnCompletion(
                            (@params, token) => Task.FromResult(new CompletionList()),
                            (_, _) => new CompletionRegistrationOptions
                            {
                                DocumentSelector = TextDocumentSelector.ForLanguage("vb")
                            }
                        )
                );

                await TestHelper.DelayUntil(
                    () => client.RegistrationManager.CurrentRegistrations,
                    registrations => registrations
                       .Any(
                            registration => SelectorMatches(registration, z => z.HasLanguage && z.Language == "vb")
                        ),
                    CancellationToken
                );

                client.RegistrationManager.CurrentRegistrations.Should().Contain(
                    x =>
                        x.Method == TextDocumentNames.Completion && SelectorMatches(x, z => z.HasLanguage && z.Language == "vb")
                );
            }

            [Fact(Skip = "Not sure if this applies so well")]
            public async Task Should_Gather_Linked_Registrations()
            {
                var (client, server) = await Initialize(new ConfigureClient().Configure, new ConfigureServer().Configure);
                await client.RegistrationManager.Registrations.Take(1);
                using var _ = server.Register(r => r.AddHandlerLink(TextDocumentNames.Completion, "@/" + TextDocumentNames.Completion));

                await TestHelper.DelayUntil(
                    () => client.RegistrationManager.CurrentRegistrations,
                    registrations => registrations.Any(registration => registration.Method.StartsWith("@/")),
                    CancellationToken
                );

                client.RegistrationManager.CurrentRegistrations.Should().Contain(x => x.Method == TextDocumentNames.Completion);
                client.RegistrationManager.CurrentRegistrations.Should().Contain(x => x.Method == "@/" + TextDocumentNames.Completion);
            }

            [Fact]
            public async Task Should_Unregister_Dynamically_While_Server_Is_Running()
            {
                var (client, server) = await Initialize(new ConfigureClient().Configure, new ConfigureServer().Configure);
                await client.RegistrationManager.Registrations.Take(1);

                client.ServerSettings.Capabilities.CompletionProvider.Should().BeNull();

                var disposable = server.Register(
                    x => x.OnCompletion(
                        (@params, token) => Task.FromResult(new CompletionList()),
                        (_, _) => new CompletionRegistrationOptions
                        {
                            DocumentSelector = TextDocumentSelector.ForLanguage("vb")
                        }
                    )
                );

                // Sometimes we come through and this fails
                disposable.Should().BeOfType<CompositeDisposable>().Subject.Count.Should().Be(2);

                await TestHelper.DelayUntil(
                    () => client.RegistrationManager.CurrentRegistrations,
                    registrations => registrations.Any(registration => SelectorMatches(registration, x => x.HasLanguage && x.Language == "vb")),
                    CancellationToken
                );

                disposable.Dispose();

                await TestHelper.DelayUntil(
                    () => client.RegistrationManager.CurrentRegistrations,
                    registrations => !registrations.Any(registration => SelectorMatches(registration, x => x.HasLanguage && x.Language == "vb")),
                    CancellationToken
                );
                await Task.Delay(200);

                client.RegistrationManager.CurrentRegistrations.Should().NotContain(
                    x =>
                        x.Method == TextDocumentNames.Completion && SelectorMatches(x, z => z.HasLanguage && z.Language == "vb")
                );
            }

            [Fact]
            public async Task Should_Only_Register_Semantic_Tokens_Registration_Once()
            {
                var tokens = Substitute.For<SemanticTokensHandlerBase>();
                tokens.CreateRegistrationOptions(Arg.Any<SemanticTokensCapability>(), Arg.Any<ClientCapabilities>())
                      .Returns(new SemanticTokensRegistrationOptions());
                var (client, server) = await Initialize(
                    new ConfigureClient().Configure, options =>
                    {
                        new ConfigureServer().Configure(options);
                        options.AddHandler(tokens);
                    }
                );

                await TestHelper.DelayUntil(
                    () => client.RegistrationManager.CurrentRegistrations,
                    registrations => registrations.Any(registration => registration.Method == TextDocumentNames.SemanticTokensRegistration),
                    CancellationToken
                );

                tokens.Received(1).CreateRegistrationOptions(Arg.Any<SemanticTokensCapability>(), Arg.Any<ClientCapabilities>());

                client.RegistrationManager.CurrentRegistrations.Should().ContainSingle(x => x.Method == TextDocumentNames.SemanticTokensRegistration);
            }

            private bool SelectorMatches(Registration registration, Func<TextDocumentFilter, bool> documentFilter)
            {
                return SelectorMatches(registration.RegisterOptions!, documentFilter);
            }

            private bool SelectorMatches(object options, Func<TextDocumentFilter, bool> documentFilter)
            {
                if (options is Registration registration)
                    return SelectorMatches(registration.RegisterOptions!, documentFilter);
                if (options is ITextDocumentRegistrationOptions tdro)
                    return tdro.DocumentSelector?.Any(documentFilter) == true;
                if (options is TextDocumentSelector selector)
                    return selector.Any(documentFilter);
                return false;
            }

            public DynamicRegistrationTests(ITestOutputHelper testOutputHelper) : base(
                new JsonRpcTestOptions().ConfigureForXUnit(testOutputHelper)
            )
            {
            }
        }

        public class StaticDynamicRegistrationTests : LanguageProtocolTestBase
        {
            public StaticDynamicRegistrationTests(ITestOutputHelper testOutputHelper) : base(new JsonRpcTestOptions().ConfigureForXUnit(testOutputHelper))
            {
            }

            [Fact]
            public async Task Should_Gather_Static_Registrations()
            {
                var (client, _) = await Initialize(
                    new ConfigureClient().Configure,
                    options =>
                    {
                        new ConfigureServer().Configure(options);
                        var semanticRegistrationOptions = new SemanticTokensRegistrationOptions
                        {
                            Id = Guid.NewGuid().ToString(),
                            Legend = new SemanticTokensLegend(),
                            Full = new SemanticTokensCapabilityRequestFull { Delta = true },
                            Range = new SemanticTokensCapabilityRequestRange(),
                            DocumentSelector = TextDocumentSelector.ForLanguage("csharp")
                        };

                        // Our server only statically registers when it detects a server that does not support dynamic capabilities
                        // This forces it to do that.
                        options.OnInitialized(
                            (server, request, response, token) =>
                            {
                                response.Capabilities.SemanticTokensProvider = new SemanticTokensRegistrationOptions.StaticOptions
                                    { Id = semanticRegistrationOptions.Id };
                                return Task.CompletedTask;
                            }
                        );
                    }
                );

                await TestHelper.DelayUntil(
                    () => client.RegistrationManager.CurrentRegistrations,
                    registrations => registrations.Any(r => r.Method == TextDocumentNames.SemanticTokensRegistration),
                    CancellationToken
                );

                client.RegistrationManager.CurrentRegistrations.Should().Contain(x => x.Method == TextDocumentNames.SemanticTokensRegistration);
            }

            [Fact]
            public async Task Should_Register_Static_When_Dynamic_Is_Disabled()
            {
                var (client, server) = await Initialize(
                    options =>
                    {
                        new ConfigureClient().Configure(options);
                        options.DisableDynamicRegistration();
                    }, new ConfigureServer().Configure
                );

                client.ServerSettings.Capabilities.CompletionProvider.Should().BeEquivalentTo(
                    new CompletionRegistrationOptions.StaticOptions
                    {
                        ResolveProvider = true,
                        TriggerCharacters = new Container<string>("a", "b"),
                        AllCommitCharacters = new Container<string>("1", "2"),
                    }, x => x.Excluding(z => z.WorkDoneProgress)
                );
                server.ClientSettings.Capabilities!.TextDocument!.Completion.Value.Should().BeEquivalentTo(
                    new CompletionCapability
                    {
                        CompletionItem = new CompletionItemCapabilityOptions
                        {
                            DeprecatedSupport = true,
                            DocumentationFormat = new[] { MarkupKind.Markdown },
                            PreselectSupport = true,
                            SnippetSupport = true,
                            TagSupport = new CompletionItemTagSupportCapabilityOptions
                            {
                                ValueSet = new[]
                                {
                                    CompletionItemTag.Deprecated
                                }
                            },
                            CommitCharactersSupport = true
                        },
                        ContextSupport = true,
                        CompletionItemKind = new CompletionItemKindCapabilityOptions
                        {
                            ValueSet = new Container<CompletionItemKind>(
                                Enum.GetValues(typeof(CompletionItemKind))
                                    .Cast<CompletionItemKind>()
                            )
                        }
                    }, x => x.ConfigureForSupports().Excluding(z => z.DynamicRegistration)
                );
                client.ClientSettings.Capabilities!.TextDocument!.Completion.Value.Should().BeEquivalentTo(
                    new CompletionCapability
                    {
                        CompletionItem = new CompletionItemCapabilityOptions
                        {
                            DeprecatedSupport = true,
                            DocumentationFormat = new[] { MarkupKind.Markdown },
                            PreselectSupport = true,
                            SnippetSupport = true,
                            TagSupport = new CompletionItemTagSupportCapabilityOptions
                            {
                                ValueSet = new[]
                                {
                                    CompletionItemTag.Deprecated
                                }
                            },
                            CommitCharactersSupport = true
                        },
                        ContextSupport = true,
                        CompletionItemKind = new CompletionItemKindCapabilityOptions
                        {
                            ValueSet = new Container<CompletionItemKind>(
                                Enum.GetValues(typeof(CompletionItemKind))
                                    .Cast<CompletionItemKind>()
                            )
                        }
                    }, x => x.ConfigureForSupports().Excluding(z => z.DynamicRegistration)
                );

                client.RegistrationManager.CurrentRegistrations.Should().NotContain(x => x.Method == TextDocumentNames.SemanticTokensFull);
            }
        }


        public class ConfigureClient : IConfigureLanguageClientOptions
        {
            public void Configure(LanguageClientOptions options)
            {
                options.EnableDynamicRegistration();
                options.WithCapability(
                    new CompletionCapability
                    {
                        CompletionItem = new CompletionItemCapabilityOptions
                        {
                            DeprecatedSupport = true,
                            DocumentationFormat = new[] { MarkupKind.Markdown },
                            PreselectSupport = true,
                            SnippetSupport = true,
                            TagSupport = new CompletionItemTagSupportCapabilityOptions
                            {
                                ValueSet = new[]
                                {
                                    CompletionItemTag.Deprecated
                                }
                            },
                            CommitCharactersSupport = true
                        },
                        ContextSupport = true,
                        CompletionItemKind = new CompletionItemKindCapabilityOptions
                        {
                            ValueSet = new Container<CompletionItemKind>(
                                Enum.GetValues(typeof(CompletionItemKind))
                                    .Cast<CompletionItemKind>()
                            )
                        }
                    }
                );

                options.WithCapability(
                    new SemanticTokensCapability
                    {
                        TokenModifiers = SemanticTokenModifier.Defaults.ToArray(),
                        TokenTypes = SemanticTokenType.Defaults.ToArray()
                    }
                );
            }
        }

        public class ConfigureServer : IConfigureLanguageServerOptions
        {
            public void Configure(LanguageServerOptions options)
            {
                options.OnCompletion(
                    (@params, token) => Task.FromResult(new CompletionList()),
                    (_, _) => new CompletionRegistrationOptions
                    {
                        DocumentSelector = TextDocumentSelector.ForLanguage("csharp"),
                        ResolveProvider = true,
                        TriggerCharacters = new Container<string>("a", "b"),
                        AllCommitCharacters = new Container<string>("1", "2"),
                    }
                );

                options.OnSemanticTokens(
                    (builder, @params, ct) => Task.CompletedTask,
                    (@params, token) => Task.FromResult(new SemanticTokensDocument(new SemanticTokensLegend())),
                    (_, _) => new SemanticTokensRegistrationOptions()
                );
            }
        }
    }
}
