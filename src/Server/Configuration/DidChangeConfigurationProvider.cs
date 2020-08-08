using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Shared;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

namespace OmniSharp.Extensions.LanguageServer.Server.Configuration
{
    class DidChangeConfigurationProvider : BaseWorkspaceConfigurationProvider, IDidChangeConfigurationHandler, IOnLanguageServerStarted, ILanguageServerConfiguration
    {
        private readonly IEnumerable<ConfigurationItem> _configurationItems;
        private readonly ILogger<DidChangeConfigurationProvider> _logger;
        private readonly IWorkspaceLanguageServer _workspaceLanguageServer;
        private DidChangeConfigurationCapability _capability;
        private readonly ConfigurationRoot _configuration;

        private readonly ConcurrentDictionary<DocumentUri, DisposableConfiguration> _openScopes =
            new ConcurrentDictionary<DocumentUri, DisposableConfiguration>();

        public DidChangeConfigurationProvider(
            IEnumerable<ConfigurationItem> configurationItems,
            Action<IConfigurationBuilder> configurationBuilderAction,
            ILogger<DidChangeConfigurationProvider> logger,
            IWorkspaceLanguageServer workspaceLanguageServer)
        {
            _configurationItems = configurationItems;
            _logger = logger;
            _workspaceLanguageServer = workspaceLanguageServer;
            var builder = new ConfigurationBuilder()
                .Add(new DidChangeConfigurationSource(this));
            configurationBuilderAction(builder);
            _configuration = builder.Build() as ConfigurationRoot;
        }

        public async Task<Unit> Handle(DidChangeConfigurationParams request, CancellationToken cancellationToken)
        {
            if (_capability == null) return Unit.Value;
            // null means we need to re-read the configuration
            // https://github.com/Microsoft/vscode-languageserver-node/issues/380
            if (request.Settings == null || request.Settings.Type == JTokenType.Null)
            {
                await GetWorkspaceConfiguration();
                return Unit.Value;
            }

            ParseClientConfiguration(request.Settings);
            OnReload();
            return Unit.Value;
        }

        public object GetRegistrationOptions() => new object();

        public void SetCapability(DidChangeConfigurationCapability capability) => _capability = capability;
        public bool IsSupported => _capability != null;

        Task IOnLanguageServerStarted.OnStarted(ILanguageServer server, CancellationToken cancellationToken) => GetWorkspaceConfiguration();

        private async Task GetWorkspaceConfiguration()
        {
            var configurationItems = _configurationItems.ToArray();
            if (configurationItems.Length == 0) return;
            if (_capability == null) return;

            {
                var configurations = (await _workspaceLanguageServer.RequestConfiguration(new ConfigurationParams() {
                    Items = configurationItems
                })).ToArray();

                foreach (var (scope, settings) in configurationItems.Zip(configurations,
                    (scope, settings) => (scope, settings)))
                {
                    ParseClientConfiguration(settings, scope.Section);
                }

                OnReload();
            }

            {
                var scopedConfigurationItems = configurationItems
                    .SelectMany(scope =>
                        _openScopes.Keys.Select(scopeUri => new ConfigurationItem() { ScopeUri = scopeUri, Section = scope.Section })
                    ).ToArray();

                try
                {

                    var configurations = (await _workspaceLanguageServer.RequestConfiguration(new ConfigurationParams() {
                        Items = scopedConfigurationItems
                    })).ToArray();

                    var groups = scopedConfigurationItems
                        .Zip(configurations, (scope, settings) => (scope, settings))
                        .GroupBy(z => z.scope.ScopeUri);

                    foreach (var group in groups)
                    {
                        if (!_openScopes.TryGetValue(group.Key, out var source)) continue;
                        source.Update(group.Select(z => (z.scope.Section, z.settings)));
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Unable to get configuration from client!");
                }
            }
        }

        public IConfigurationSection GetSection(string key) => _configuration.GetSection(key);

        public IEnumerable<IConfigurationSection> GetChildren() => _configuration.GetChildren();

        public IChangeToken GetReloadToken() => _configuration.GetReloadToken();

        public string this[string key]
        {
            get => _configuration[key];
            set => _configuration[key] = value;
        }

        public async Task<IConfiguration> GetConfiguration(params ConfigurationItem[] items)
        {
            if (_capability == null || items.Length == 0)
            {
                return new ConfigurationBuilder().AddInMemoryCollection(_configuration.AsEnumerable()).Build();
            }

            var configurations = await _workspaceLanguageServer.RequestConfiguration(new ConfigurationParams() {
                Items = items
            });
            var data = items.Zip(configurations,
                (scope, settings) => (scope.Section, settings));
            return new ConfigurationBuilder()
                // this avoids chaining the configurations
                // so that the returned configuration object
                // is stateless.
                // scoped configuration should be a snapshot of the current state.
                .AddInMemoryCollection(_configuration.AsEnumerable())
                .Add(new WorkspaceConfigurationSource(data))
                .Build();
        }

        public async Task<IScopedConfiguration> GetScopedConfiguration(DocumentUri scopeUri)
        {
            var scopes = _configurationItems.ToArray();
            if (scopes.Length == 0)
                return EmptyDisposableConfiguration.Instance;

            var configurations = await _workspaceLanguageServer .RequestConfiguration(new ConfigurationParams() {
                Items = scopes.Select(z => new ConfigurationItem() { Section = z.Section, ScopeUri = scopeUri }).ToArray()
            });

            var data = scopes.Zip(configurations,
                (scope, settings) => (scope.Section, settings));

            var config = new DisposableConfiguration(
                new ConfigurationBuilder()
                    .AddConfiguration(_configuration),
                new WorkspaceConfigurationSource(data),
                Disposable.Create(
                    () => _openScopes.TryRemove(scopeUri, out _))
            );

            _openScopes.TryAdd(scopeUri, config);
            return config;
        }

        public bool TryGetScopedConfiguration(DocumentUri scopeUri, out IScopedConfiguration disposable)
        {
            var result = _openScopes.TryGetValue(scopeUri, out var c);
            if (result)
            {
                disposable = c;
                return true;
            }

            disposable = EmptyDisposableConfiguration.Instance;
            return false;
        }
    }
}
