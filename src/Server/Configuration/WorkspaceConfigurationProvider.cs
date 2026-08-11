using System.Collections.Generic;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace OmniSharp.Extensions.LanguageServer.Server.Configuration
{
    internal class WorkspaceConfigurationProvider : ConfigurationProvider
    {
        private readonly ConfigurationConverter _configurationConverter;

        public WorkspaceConfigurationProvider(
            ConfigurationConverter configurationConverter,
            IEnumerable<(string key, JsonElement settings)> configuration)
        {
            _configurationConverter = configurationConverter;
            Update(configuration);
        }

        internal void Update(IEnumerable<(string key, JsonElement settings)> values)
        {
            Data.Clear();
            foreach (var (key, settings) in values)
            {
                _configurationConverter.ParseClientConfiguration(Data, settings, key);
            }

            OnReload();
        }
    }
}
