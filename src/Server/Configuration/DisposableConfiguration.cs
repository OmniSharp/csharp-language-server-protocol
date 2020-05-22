using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Server.Configuration
{
    class DisposableConfiguration : IScopedConfiguration
    {
        private readonly ConfigurationRoot _configuration;
        private readonly WorkspaceConfigurationSource _configurationSource;
        private readonly IDisposable _disposable;

        public DisposableConfiguration(IConfigurationBuilder configurationBuilder, WorkspaceConfigurationSource configurationSource, IDisposable disposable)
        {
            _configuration = configurationBuilder.Add(configurationSource).Build() as ConfigurationRoot;
            _configurationSource = configurationSource;
            _disposable = disposable;
        }

        public IConfigurationSection GetSection(string key) => _configuration.GetSection(key);

        public IEnumerable<IConfigurationSection> GetChildren() => _configuration.GetChildren();

        public IChangeToken GetReloadToken() => _configuration.GetReloadToken();

        internal void Update(IEnumerable<(string key, JToken settings)> data) => _configurationSource.Update(data);

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
