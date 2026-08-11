using System.Collections.Generic;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace OmniSharp.Extensions.LanguageServer.Server.Configuration
{
    internal class WorkspaceConfigurationSource : IConfigurationSource
    {
        private readonly WorkspaceConfigurationProvider _provider;

        public WorkspaceConfigurationSource(ConfigurationConverter configurationConverter, IEnumerable<(string key, JsonElement settings)> configuration)
        {
            _provider = new WorkspaceConfigurationProvider(configurationConverter, configuration);
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder) => _provider;

        internal void Update(IEnumerable<(string key, JsonElement settings)> values) => _provider.Update(values);
    }
}
