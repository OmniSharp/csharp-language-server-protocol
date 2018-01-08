using NSubstitute;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Server;

namespace Lsp.Tests
{
    internal class ClientCapabilityProviderFixture
    {
        private IExecuteCommandHandler handler;

        public ClientCapabilityProvider Provider { get; set; }

        public ClientCapabilityProviderFixture()
        {
            handler = Substitute.For<IExecuteCommandHandler>();
            handler.GetRegistrationOptions().Returns(new ExecuteCommandRegistrationOptions());

            var handlerCollection = new HandlerCollection { handler };
            var capabilityProvider = new ClientCapabilityProvider(handlerCollection);

            Provider = capabilityProvider;
        }

        public ClientCapabilityProvider.IOptionsGetter GetStaticOptions()
        {
            return Provider.GetStaticOptions(new Supports<ExecuteCommandCapability>(true, new ExecuteCommandCapability { DynamicRegistration = false }));
        }
    }
}