using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer;
using OmniSharp.Extensions.LanguageServer.Capabilities.Client;
using OmniSharp.Extensions.LanguageServer.Models;
using Xunit;
using HandlerCollection = OmniSharp.Extensions.LanguageServer.HandlerCollection;

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
