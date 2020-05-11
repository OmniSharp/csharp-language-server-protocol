using System.Collections.Generic;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace OmniSharp.Extensions.LanguageServer.Server.Configuration
{
    class WorkspaceConfigurationSource : IConfigurationSource
    {
        private WorkspaceConfigurationProvider _provider;

        public WorkspaceConfigurationSource(IEnumerable<(string key, JsonElement settings)> configuration)
        {
            _provider = new WorkspaceConfigurationProvider(configuration);
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder) => _provider;

        internal void Update(IEnumerable<(string key, JsonElement settings)> values) => _provider.Update(values);
    }
}
