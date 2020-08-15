using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Server.Configuration
{
    internal class ScopedConfiguration : IScopedConfiguration
    {
        private ConfigurationRoot _configuration;
        private readonly IConfiguration _rootConfiguration;
        private readonly WorkspaceConfigurationSource _configurationSource;
        private readonly IDisposable _disposable;

        public ScopedConfiguration(
            IConfiguration rootConfiguration,
            ConfigurationConverter configurationConverter,
            IEnumerable<(string key, JToken settings)> configuration,
            IDisposable disposable)
        {
            _configurationSource = new WorkspaceConfigurationSource(configurationConverter, configuration);
            _configuration = new ConfigurationBuilder()
                            .AddConfiguration(rootConfiguration)
                            .Add(_configurationSource)
                            .Build() as ConfigurationRoot;
            _rootConfiguration = rootConfiguration;
            _disposable = disposable;
        }

        public IConfigurationSection GetSection(string key) => _configuration.GetSection(key);

        public IEnumerable<IConfigurationSection> GetChildren() => _configuration.GetChildren();

        public IChangeToken GetReloadToken() => _configuration.GetReloadToken();

        internal void Update(IEnumerable<(string key, JToken settings)> data)
        {
            _configurationSource.Update(data);
        }

        public string this[string key]
        {
            get => _configuration[key];
            set => _configuration[key] = value;
        }

        public void Dispose()
        {
            _configuration.Dispose();
            _disposable.Dispose();
        }
    }
}
