using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace OmniSharp.Extensions.LanguageServer.Server.Configuration
{
    internal class WorkspaceConfigurationProvider : ConfigurationProvider
    {
        private readonly ConfigurationConverter _configurationConverter;

        public WorkspaceConfigurationProvider(
            ConfigurationConverter configurationConverter,
            IEnumerable<(string key, JToken settings)> configuration)
        {
            _configurationConverter = configurationConverter;
            Update(configuration);
        }

        internal void Update(IEnumerable<(string key, JToken settings)> values)
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
