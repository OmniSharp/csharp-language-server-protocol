using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using OmniSharp.Extensions.LanguageServer.Server;
using Xunit;
using HandlerCollection = OmniSharp.Extensions.LanguageServer.Server.HandlerCollection;

namespace Lsp.Tests
{
    public class ClientCapabilityProviderTests
    {
        private static readonly Type[] Capabilities = {
            typeof(CompletionCapability),
            typeof(HoverCapability),
            typeof(SignatureHelpCapability),
            typeof(ReferencesCapability),
            typeof(DocumentHighlightCapability),
            typeof(DocumentSymbolCapability),
            typeof(DocumentFormattingCapability),
            typeof(DocumentRangeFormattingCapability),
            typeof(DocumentOnTypeFormattingCapability),
            typeof(DefinitionCapability),
            typeof(CodeActionCapability),
            typeof(CodeLensCapability),
            typeof(DocumentLinkCapability),
            typeof(RenameCapability),
            typeof(WorkspaceSymbolCapability),
            typeof(ExecuteCommandCapability),
        };

        [Theory, MemberData(nameof(AllowSupportedCapabilities))]
        public void Should_AllowSupportedCapabilities(IJsonRpcHandler handler, object instance)
        {
            var textDocumentSyncHandler = TextDocumentSyncHandlerExtensions.With(DocumentSelector.ForPattern("**/*.cs"));

            var collection = new HandlerCollection { textDocumentSyncHandler, handler };
            var provider = new ClientCapabilityProvider(collection);

            HasHandler(provider, instance).Should().BeTrue();
        }

        public static IEnumerable<object[]> AllowSupportedCapabilities()
        {
            return GetItems(Capabilities, type => {
                var handlerType = GetHandlerType(type);
                var handler = Substitute.For(new Type[] { handlerType }, new object[0]);
                return new[] { handler, Activator.CreateInstance(typeof(Supports<>).MakeGenericType(type), true, Activator.CreateInstance(type)) };
            });
        }

        [Theory, MemberData(nameof(DisallowUnsupportedCapabilities))]
        public void Should_DisallowUnsupportedCapabilities(IJsonRpcHandler handler, object instance)
        {
            var textDocumentSyncHandler = TextDocumentSyncHandlerExtensions.With(DocumentSelector.ForPattern("**/*.cs"));

            var collection = new HandlerCollection { textDocumentSyncHandler, handler };
            var provider = new ClientCapabilityProvider(collection);

            HasHandler(provider, instance).Should().BeFalse();
        }

        [Fact]
        public void Should_Invoke_Get_Delegate()
        {
            // Given
            var stub = Substitute.For<Func<IExecuteCommandOptions, ExecuteCommandOptions>>();
            var provider = new ClientCapabilityProviderFixture().GetStaticOptions();

            // When
            provider.Get(stub);

            // Then
            stub.Received().Invoke(Arg.Any<IExecuteCommandOptions>());
        }

        [Fact]
        public void Should_Invoke_Reduce_Delegate()
        {
            // Given
            var stub = Substitute.For<Func<IEnumerable<IExecuteCommandOptions>, ExecuteCommandOptions>>();
            var provider = new ClientCapabilityProviderFixture().GetStaticOptions();

            // When
            provider.Reduce(stub);

            // Then
            stub.Received().Invoke(Arg.Any<IEnumerable<IExecuteCommandOptions>>());
        }

        public static IEnumerable<object[]> DisallowUnsupportedCapabilities()
        {
            return GetItems(Capabilities, type => {
                var handlerType = GetHandlerType(type);
                var handler = Substitute.For(new Type[] { handlerType }, new object[0]);
                return new[] { handler, Activator.CreateInstance(typeof(Supports<>).MakeGenericType(type), false) };
            });
        }

        [Theory, MemberData(nameof(DisallowNullSupportsCapabilities))]
        public void Should_DisallowNullSupportedCapabilities(IJsonRpcHandler handler, object instance)
        {
            var textDocumentSyncHandler = TextDocumentSyncHandlerExtensions.With(DocumentSelector.ForPattern("**/*.cs"));

            var collection = new HandlerCollection { textDocumentSyncHandler, handler };
            var provider = new ClientCapabilityProvider(collection);

            HasHandler(provider, instance).Should().BeFalse();
        }

        public static IEnumerable<object[]> DisallowNullSupportsCapabilities()
        {
            return GetItems(Capabilities, type => {
                var handlerType = GetHandlerType(type);
                var handler = Substitute.For(new Type[] { handlerType }, new object[0]);
                return new[] { handler, Activator.CreateInstance(typeof(Supports<>).MakeGenericType(type), true) };
            });
        }

        private static bool HasHandler(ClientCapabilityProvider provider, object instance)
        {
            return (bool)typeof(ClientCapabilityProviderTests).GetTypeInfo()
                .GetMethod(nameof(GenericHasHandler), BindingFlags.Static | BindingFlags.NonPublic)
                .MakeGenericMethod(instance.GetType().GetTypeInfo().GetGenericArguments()[0]).Invoke(null, new[] { provider, instance });
        }

        private static bool HasHandler(ClientCapabilityProvider provider, Type type)
        {
            return (bool)typeof(ClientCapabilityProviderTests).GetTypeInfo()
                .GetMethod(nameof(GenericHasHandler), BindingFlags.Static | BindingFlags.NonPublic)
                .MakeGenericMethod(type).Invoke(null, new object[] { provider, null });
        }

        private static bool GenericHasHandler<T>(ClientCapabilityProvider provider, Supports<T> supports)
            where T : DynamicCapability, ConnectedCapability<IJsonRpcHandler>
        {
            return provider.HasStaticHandler(supports);
        }

        private static IEnumerable<object[]> GetItems<T>(IEnumerable<T> types, Func<T, IEnumerable<object>> func)
        {
            return types.Select(x => func(x).ToArray());
        }

        private static Type GetHandlerType(Type type)
        {
            return type.GetTypeInfo().ImplementedInterfaces
                .Single(x => x.GetTypeInfo().IsGenericType && x.GetTypeInfo().GetGenericTypeDefinition() == typeof(ConnectedCapability<>))
                .GetTypeInfo().GetGenericArguments()[0];
        }
    }
}
