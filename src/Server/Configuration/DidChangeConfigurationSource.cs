using Microsoft.Extensions.Configuration;

namespace OmniSharp.Extensions.LanguageServer.Server.Configuration
{
    internal class DidChangeConfigurationSource : IConfigurationSource
    {
        private readonly DidChangeConfigurationProvider _provider;

        public DidChangeConfigurationSource(DidChangeConfigurationProvider provider) => _provider = provider;

        public IConfigurationProvider Build(IConfigurationBuilder builder) => _provider;
    }
}
