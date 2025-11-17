using System.Reflection;
using DryIoc;
using FluentAssertions;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Shared;
using OmniSharp.Extensions.LanguageServer.Server;
using OmniSharp.Extensions.LanguageServer.Shared;

namespace Lsp.Tests
{
    public class ClientCapabilityProviderTests
    {
        private static readonly Type[] Capabilities = typeof(ClientCapabilities).Assembly.GetTypes()
                                                                                .Where(
                                                                                     x => typeof(ICapability).IsAssignableFrom(x) && x.IsClass && !x.IsAbstract
                                                                                 )
                                                                                .Where(z => typeof(IDynamicCapability).IsAssignableFrom(z))
                                                                                .ToArray();

        [Theory]
        [MemberData(nameof(AllowSupportedCapabilities))]
        public void Should_AllowSupportedCapabilities(IJsonRpcHandler handler, object instance)
        {
            var textDocumentSyncHandler =
                TextDocumentSyncHandlerExtensions.With(TextDocumentSelector.ForPattern("**/*.cs"), "csharp");

            var collection =
                new SharedHandlerCollection(
                        SupportedCapabilitiesFixture.AlwaysTrue, new TextDocumentIdentifiers(), Substitute.For<IResolverContext>(),
                        new LspHandlerTypeDescriptorProvider(
                            new[]
                            {
                                typeof(FoundationTests).Assembly, typeof(LanguageServer).Assembly, typeof(LanguageClient).Assembly,
                                typeof(IRegistrationManager).Assembly,
                                typeof(LspRequestRouter).Assembly
                            }
                        )
                    )
                    { textDocumentSyncHandler, handler };
            var provider = new ClientCapabilityProvider(collection, true);

            HasHandler(provider, instance).Should().BeTrue();
        }

        public static IEnumerable<object?[]> AllowSupportedCapabilities()
        {
            return GetItems(
                Capabilities, type =>
                {
                    var handlerTypes = GetHandlerTypes(type);
                    if (handlerTypes is { Length: 0 }) return null;
                    var handler = Substitute.For(handlerTypes.ToArray(), new object[0]);
                    return new[]
                    {
                        handler,
                        Activator.CreateInstance(
                            typeof(Supports<>).MakeGenericType(type), true,
                            Activator.CreateInstance(type)
                        )
                    };
                }
            );
        }

        [Theory]
        [MemberData(nameof(AllowUnsupportedCapabilities))]
        public void Should_AllowUnsupportedCapabilities(IJsonRpcHandler handler, object instance)
        {
            var textDocumentSyncHandler =
                TextDocumentSyncHandlerExtensions.With(TextDocumentSelector.ForPattern("**/*.cs"), "csharp");

            var collection =
                new SharedHandlerCollection(
                        SupportedCapabilitiesFixture.AlwaysTrue, new TextDocumentIdentifiers(), Substitute.For<IResolverContext>(),
                        new LspHandlerTypeDescriptorProvider(
                            new[]
                            {
                                typeof(FoundationTests).Assembly, typeof(LanguageServer).Assembly, typeof(LanguageClient).Assembly,
                                typeof(IRegistrationManager).Assembly,
                                typeof(LspRequestRouter).Assembly
                            }
                        )
                    )
                    { textDocumentSyncHandler, handler };
            var provider = new ClientCapabilityProvider(collection, true);

            HasHandler(provider, instance).Should().BeTrue();
        }

        public static IEnumerable<object?[]> AllowUnsupportedCapabilities()
        {
            return GetItems(
                Capabilities, type =>
                {
                    var handlerTypes = GetHandlerTypes(type);
                    if (handlerTypes is { Length: 0 }) return null;
                    var handler = Substitute.For(handlerTypes, new object[0]);
                    return new[] { handler, Activator.CreateInstance(typeof(Supports<>).MakeGenericType(type), false) };
                }
            );
        }

        [Theory]
        [MemberData(nameof(AllowNullSupportsCapabilities))]
        public void Should_AllowNullSupportedCapabilities(IJsonRpcHandler handler, object instance)
        {
            var textDocumentSyncHandler =
                TextDocumentSyncHandlerExtensions.With(TextDocumentSelector.ForPattern("**/*.cs"), "csharp");

            var collection =
                new SharedHandlerCollection(
                        SupportedCapabilitiesFixture.AlwaysTrue, new TextDocumentIdentifiers(), Substitute.For<IResolverContext>(),
                        new LspHandlerTypeDescriptorProvider(
                            new[]
                            {
                                typeof(FoundationTests).Assembly, typeof(LanguageServer).Assembly, typeof(LanguageClient).Assembly,
                                typeof(IRegistrationManager).Assembly,
                                typeof(LspRequestRouter).Assembly
                            }
                        )
                    )
                    { textDocumentSyncHandler, handler };
            var provider = new ClientCapabilityProvider(collection, true);

            HasHandler(provider, instance).Should().BeTrue();
        }

        public static IEnumerable<object?[]> AllowNullSupportsCapabilities()
        {
            return GetItems(
                Capabilities, type =>
                {
                    var handlerTypes = GetHandlerTypes(type);
                    if (handlerTypes is { Length: 0 }) return null;
                    var handler = Substitute.For(handlerTypes.ToArray(), new object[0]);
                    return new[] { handler, Activator.CreateInstance(typeof(Supports<>).MakeGenericType(type), true) };
                }
            );
        }


        [Theory]
        [MemberData(nameof(DisallowDynamicSupportsCapabilities))]
        public void Should_DisallowDynamicSupportedCapabilities(IJsonRpcHandler handler, object instance)
        {
            var textDocumentSyncHandler =
                TextDocumentSyncHandlerExtensions.With(TextDocumentSelector.ForPattern("**/*.cs"), "csharp");

            var collection =
                new SharedHandlerCollection(
                        SupportedCapabilitiesFixture.AlwaysTrue, new TextDocumentIdentifiers(), Substitute.For<IResolverContext>(),
                        new LspHandlerTypeDescriptorProvider(
                            new[]
                            {
                                typeof(FoundationTests).Assembly, typeof(LanguageServer).Assembly, typeof(LanguageClient).Assembly,
                                typeof(IRegistrationManager).Assembly,
                                typeof(LspRequestRouter).Assembly
                            }
                        )
                    )
                    { textDocumentSyncHandler, handler };
            var provider = new ClientCapabilityProvider(collection, true);

            HasHandler(provider, instance).Should().BeFalse();
        }

        public static IEnumerable<object?[]>? DisallowDynamicSupportsCapabilities()
        {
            return GetItems(
                Capabilities, type =>
                {
                    var handlerTypes = GetHandlerTypes(type);
                    if (handlerTypes is { Length: 0 }) return null;
                    var handler = Substitute.For(handlerTypes.ToArray(), new object[0]);
                    var capability = Activator.CreateInstance(type);
                    if (capability is DynamicCapability dyn) dyn.DynamicRegistration = true;
                    return new[]
                        { handler, Activator.CreateInstance(typeof(Supports<>).MakeGenericType(type), true, capability) };
                }
            );
        }

        [Fact]
        public void Should_Handle_Mixed_Capabilities()
        {
            var textDocumentSyncHandler =
                TextDocumentSyncHandlerExtensions.With(TextDocumentSelector.ForPattern("**/*.cs"), "csharp");

            var codeActionHandler = Substitute.For<ICodeActionHandler>();
            var definitionHandler = Substitute.For<IDefinitionHandler>();
            var typeDefinitionHandler = Substitute.For<ITypeDefinitionHandler>();

            var collection =
                new SharedHandlerCollection(
                        SupportedCapabilitiesFixture.AlwaysTrue, new TextDocumentIdentifiers(), Substitute.For<IResolverContext>(),
                        new LspHandlerTypeDescriptorProvider(
                            new[]
                            {
                                typeof(FoundationTests).Assembly, typeof(LanguageServer).Assembly, typeof(LanguageClient).Assembly,
                                typeof(IRegistrationManager).Assembly,
                                typeof(LspRequestRouter).Assembly
                            }
                        )
                    )
                    { textDocumentSyncHandler, codeActionHandler, definitionHandler, typeDefinitionHandler };
            var provider = new ClientCapabilityProvider(collection, true);
            var capabilities = new ClientCapabilities
            {
                TextDocument = new TextDocumentClientCapabilities
                {
                    CodeAction = new Supports<CodeActionCapability?>(
                        true, new CodeActionCapability
                        {
                            DynamicRegistration = false,
                        }
                    ),
                    TypeDefinition = new Supports<TypeDefinitionCapability?>(
                        true, new TypeDefinitionCapability
                        {
                            DynamicRegistration = true,
                        }
                    )
                }
            };

//            provider.GetStaticOptions(capabilities.TextDocument.CodeAction)
//                    .Get<ICodeActionOptions, CodeActionRegistrationOptions.StaticOptions>(CodeActionRegistrationOptions.StaticOptions.Of).Should().NotBeNull();
            provider.HasStaticHandler(capabilities.TextDocument.Definition).Should().BeTrue();
            provider.HasStaticHandler(capabilities.TextDocument.TypeDefinition).Should().BeFalse();
        }

        [Fact]
        public void GH162_TextDocumentSync_Should_Work_Without_WillSave_Or_WillSaveWaitUntil()
        {
            var textDocumentSyncHandler =
                TextDocumentSyncHandlerExtensions.With(TextDocumentSelector.ForPattern("**/*.cs"), "csharp");

            var collection =
                new SharedHandlerCollection(
                        SupportedCapabilitiesFixture.AlwaysTrue, new TextDocumentIdentifiers(), Substitute.For<IResolverContext>(),
                        new LspHandlerTypeDescriptorProvider(
                            new[]
                            {
                                typeof(FoundationTests).Assembly, typeof(LanguageServer).Assembly, typeof(LanguageClient).Assembly,
                                typeof(IRegistrationManager).Assembly,
                                typeof(LspRequestRouter).Assembly
                            }
                        )
                    )
                    { textDocumentSyncHandler };
            var provider = new ClientCapabilityProvider(collection, true);
            var capabilities = new ClientCapabilities
            {
                TextDocument = new TextDocumentClientCapabilities
                {
                    Synchronization = new TextSynchronizationCapability
                    {
                        DidSave = true,
                        DynamicRegistration = false,
                        WillSave = true,
                        WillSaveWaitUntil = true
                    },
                }
            };

            provider.HasStaticHandler(capabilities.TextDocument.Synchronization).Should().BeTrue();
        }

        [Fact]
        public void GH162_TextDocumentSync_Should_Work_With_WillSave_Or_WillSaveWaitUntil()
        {
            var textDocumentSyncHandler =
                TextDocumentSyncHandlerExtensions.With(TextDocumentSelector.ForPattern("**/*.cs"), "csharp");
            var willSaveTextDocumentHandler = Substitute.For<IWillSaveTextDocumentHandler>();
            var willSaveWaitUntilTextDocumentHandler = Substitute.For<IWillSaveWaitUntilTextDocumentHandler>();
            var didSaveTextDocumentHandler = Substitute.For<IDidSaveTextDocumentHandler>();

            var collection =
                new SharedHandlerCollection(
                    SupportedCapabilitiesFixture.AlwaysTrue, new TextDocumentIdentifiers(), Substitute.For<IResolverContext>(),
                    new LspHandlerTypeDescriptorProvider(
                        new[]
                        {
                            typeof(FoundationTests).Assembly, typeof(LanguageServer).Assembly, typeof(LanguageClient).Assembly,
                            typeof(IRegistrationManager).Assembly,
                            typeof(LspRequestRouter).Assembly
                        }
                    )
                )
                {
                    textDocumentSyncHandler, willSaveTextDocumentHandler, willSaveWaitUntilTextDocumentHandler,
                    didSaveTextDocumentHandler
                };
            var provider = new ClientCapabilityProvider(collection, true);
            var capabilities = new ClientCapabilities
            {
                TextDocument = new TextDocumentClientCapabilities
                {
                    Synchronization = new TextSynchronizationCapability
                    {
                        DidSave = true,
                        DynamicRegistration = false,
                        WillSave = true,
                        WillSaveWaitUntil = true
                    },
                }
            };

            provider.HasStaticHandler(capabilities.TextDocument.Synchronization).Should().BeTrue();
        }

        private static bool HasHandler(ClientCapabilityProvider provider, object instance)
        {
            return (bool)typeof(ClientCapabilityProviderTests).GetTypeInfo()
                                                              .GetMethod(nameof(GenericHasHandler), BindingFlags.Static | BindingFlags.NonPublic)!
                                                              .MakeGenericMethod(instance.GetType().GetTypeInfo().GetGenericArguments()[0])
                                                              .Invoke(null, new[] { provider, instance })!;
        }

        private static bool GenericHasHandler<T>(ClientCapabilityProvider provider, Supports<T> supports)
            where T : DynamicCapability
        {
            return provider.HasStaticHandler(supports);
        }

        private static IEnumerable<object?[]> GetItems<T>(IEnumerable<T> types, Func<T, IEnumerable<object?>?> func)
        {
            return types.Select(func).Where(z => z is not null).Select(z => z!.ToArray());
        }

        private static Type[] GetHandlerTypes(Type type)
        {
            return HandlerProvider.KnownHandlers
                                  .SelectMany(z => z.AsEnumerable())
                                  .Where(x => x.HasCapability && x.CapabilityType == type)
                                  .Select(z => z.HandlerType)
                                  .Where(z => z.IsInterface && !z.IsGenericType) // only testing our interfaces
                                  .ToArray();
        }

        private static readonly LspHandlerTypeDescriptorProvider HandlerProvider = new LspHandlerTypeDescriptorProvider(
            new[]
            {
                typeof(AssemblyScanningHandlerTypeDescriptorProvider).Assembly,
                typeof(LspHandlerTypeDescriptorProvider).Assembly,
                typeof(LanguageServer).Assembly,
                typeof(LanguageClient).Assembly,
                typeof(ISupports).Assembly,
                typeof(HandlerResolverTests).Assembly
            }
        );
    }
}
