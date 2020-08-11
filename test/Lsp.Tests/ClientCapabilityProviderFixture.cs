using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using OmniSharp.Extensions.LanguageServer.Server;
using OmniSharp.Extensions.LanguageServer.Shared;

namespace Lsp.Tests
{
    internal class ClientCapabilityProviderFixture
    {
        public ClientCapabilityProvider Provider { get; set; }

        public ClientCapabilityProviderFixture()
        {
            var handler = Substitute.For<IExecuteCommandHandler>();
            handler.GetRegistrationOptions().Returns(new ExecuteCommandRegistrationOptions());

            var handlerCollection = new SharedHandlerCollection(
                SupportedCapabilitiesFixture.AlwaysTrue, new TextDocumentIdentifiers(), new ServiceCollection().BuildServiceProvider()
            ) { handler };
            var capabilityProvider = new ClientCapabilityProvider(handlerCollection, true);

            Provider = capabilityProvider;
        }

        public ClientCapabilityProvider.IOptionsGetter GetStaticOptions() =>
            Provider.GetStaticOptions(new Supports<ExecuteCommandCapability>(true, new ExecuteCommandCapability { DynamicRegistration = false }));
    }
}
