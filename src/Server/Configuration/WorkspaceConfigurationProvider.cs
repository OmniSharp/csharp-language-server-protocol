using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace OmniSharp.Extensions.LanguageServer.Server.Configuration
{
    internal class WorkspaceConfigurationProvider : BaseWorkspaceConfigurationProvider
    {
        public WorkspaceConfigurationProvider(IEnumerable<(string key, JToken settings)> configuration) => Update(configuration);

        internal void Update(IEnumerable<(string key, JToken settings)> values)
        {
            foreach (var (key, settings) in values)
            {
                ParseClientConfiguration(settings, key);
            }

            OnReload();
        }
    }
}
