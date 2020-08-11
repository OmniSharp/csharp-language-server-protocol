using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace OmniSharp.Extensions.LanguageServer.Server.Configuration
{
    internal class WorkspaceConfigurationSource : IConfigurationSource
    {
        private readonly WorkspaceConfigurationProvider _provider;

        public WorkspaceConfigurationSource(IEnumerable<(string key, JToken settings)> configuration) => _provider = new WorkspaceConfigurationProvider(configuration);

        public IConfigurationProvider Build(IConfigurationBuilder builder) => _provider;

        internal void Update(IEnumerable<(string key, JToken settings)> values) => _provider.Update(values);
    }
}
