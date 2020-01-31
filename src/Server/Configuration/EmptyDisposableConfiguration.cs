using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace OmniSharp.Extensions.LanguageServer.Server.Configuration
{
    class EmptyDisposableConfiguration : IDisposableConfiguration
    {
        public static EmptyDisposableConfiguration Instance { get; } = new EmptyDisposableConfiguration();
        private ConfigurationRoot _configuration;

        private EmptyDisposableConfiguration()
        {
            _configuration = new ConfigurationBuilder().Build() as ConfigurationRoot;
        }
        void IDisposable.Dispose() {}

        IConfigurationSection IConfiguration.GetSection(string key) => _configuration.GetSection(key);

        IEnumerable<IConfigurationSection> IConfiguration.GetChildren() => _configuration.GetChildren();

        IChangeToken IConfiguration.GetReloadToken() => _configuration.GetReloadToken();

        string IConfiguration.this[string key]
        {
            get => _configuration[key];
            set => _configuration[key] = value;
        }
    }
}