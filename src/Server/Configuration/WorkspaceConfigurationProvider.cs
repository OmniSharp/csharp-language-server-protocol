using System.Collections.Generic;
using System.Text.Json;

namespace OmniSharp.Extensions.LanguageServer.Server.Configuration
{
    class WorkspaceConfigurationProvider : BaseWorkspaceConfigurationProvider
    {
        public WorkspaceConfigurationProvider(IEnumerable<(string key, JsonElement settings)> configuration)
        {
            Update(configuration);
        }

        internal void Update(IEnumerable<(string key, JsonElement settings)> values)
        {
            foreach (var (key, settings) in values)
            {
                ParseClientConfiguration(settings, key);
            }
            OnReload();
        }
    }
}
