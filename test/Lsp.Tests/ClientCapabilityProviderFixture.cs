using System;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Server;
using OmniSharp.Extensions.LanguageServer.Server.Abstractions;

namespace Lsp.Tests
{
    internal class ClientCapabilityProviderFixture
    {
        public ClientCapabilityProvider Provider { get; set; }

        public ClientCapabilityProviderFixture()
        {
            var handler = Substitute.For<IExecuteCommandHandler>();
            handler.GetRegistrationOptions().Returns(new ExecuteCommandRegistrationOptions());

            var handlerCollection = new OmniSharp.Extensions.LanguageServer.Server.HandlerCollection(SupportedCapabilitiesFixture.AlwaysTrue, new TextDocumentIdentifiers()) { handler };
            var capabilityProvider = new ClientCapabilityProvider(handlerCollection, true);

            Provider = capabilityProvider;
        }

        public ClientCapabilityProvider.IOptionsGetter GetStaticOptions()
        {
            return Provider.GetStaticOptions(new Supports<ExecuteCommandCapability>(true, new ExecuteCommandCapability { DynamicRegistration = false }));
        }
    }
}
