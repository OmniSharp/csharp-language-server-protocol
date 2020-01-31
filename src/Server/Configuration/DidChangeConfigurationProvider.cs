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
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Server.Configuration
{
    class DidChangeConfigurationProvider : BaseWorkspaceConfigurationProvider, IDidChangeConfigurationHandler,
        IOnStarted, ILanguageServerConfiguration
    {
        private readonly ILanguageServer _server;
        private DidChangeConfigurationCapability _capability;
        private readonly ConfigurationRoot _configuration;

        private readonly ConcurrentDictionary<Uri, WorkspaceConfigurationSource> _openScopes =
            new ConcurrentDictionary<Uri, WorkspaceConfigurationSource>();

        public DidChangeConfigurationProvider(ILanguageServer server, Action<IConfigurationBuilder> configurationBuilderAction)
        {
            _server = server;
            var builder = new ConfigurationBuilder()
                .Add(new DidChangeConfigurationSource(this));
            configurationBuilderAction(builder);
            _configuration = builder.Build() as ConfigurationRoot;
        }

        public async Task<Unit> Handle(DidChangeConfigurationParams request, CancellationToken cancellationToken)
        {
            // null means we need to re-read the configuration
            // https://github.com/Microsoft/vscode-languageserver-node/issues/380
            if (request.Settings == null || request.Settings.Type == JTokenType.Null)
            {
                _server.Services.GetRequiredService<ILogger<DidChangeConfigurationProvider>>().LogInformation("Reloading workspace configuration!");
                await GetWorkspaceConfiguration();
                return Unit.Value;
            }

            _server.Services.GetRequiredService<ILogger<DidChangeConfigurationProvider>>().LogInformation("parsing provided configuration!");
            ParseClientConfiguration(request.Settings);
            OnReload();
            return Unit.Value;
        }

        public object GetRegistrationOptions() => new object();

        public void SetCapability(DidChangeConfigurationCapability capability) => _capability = capability;

        Task IOnStarted.OnStarted(ILanguageServer server, InitializeResult result) => GetWorkspaceConfiguration();

        private async Task GetWorkspaceConfiguration()
        {
            var configurationItems = _server.Services.GetServices<ConfigurationItem>().ToArray();
            if (configurationItems.Length == 0) return;

            {
                var configurations = (await _server.Workspace.WorkspaceConfiguration(new ConfigurationParams() {
                    Items = configurationItems
                })).ToArray();

                foreach (var (scope, settings) in configurationItems.Zip(configurations,
                    (scope, settings) => (scope, settings)))
                {
                    ParseClientConfiguration(settings, scope.Section);

                    _server.Services.GetRequiredService<ILogger<DidChangeConfigurationProvider>>().LogInformation("Config: {Config}", settings);
                }

                OnReload();
            }

            {
                var scopedConfigurationItems = configurationItems
                    .SelectMany(scope =>
                        _openScopes.Keys.Select(scopeUri => new ConfigurationItem()
                            {ScopeUri = scopeUri, Section = scope.Section})
                    ).ToArray();

                var configurations = (await _server.Workspace.WorkspaceConfiguration(new ConfigurationParams() {
                    Items = scopedConfigurationItems
                })).ToArray();

                var groups = scopedConfigurationItems
                    .Zip(configurations, (scope, settings) => (scope, settings))
                    .GroupBy(z => z.scope.ScopeUri);

                foreach (var group in groups)
                {
                    if (!_openScopes.TryGetValue(group.Key, out var source)) continue;
                    source.Update(group.Select(z => (z.scope.Section, z.settings)));
                    _server.Services.GetRequiredService<ILogger<DidChangeConfigurationProvider>>().LogInformation("Group: {Config}", group);
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
            if (items.Length == 0)
            {
                return new ConfigurationBuilder().AddInMemoryCollection(_configuration.AsEnumerable()).Build();
            }

            var configurations = await _server.Workspace.WorkspaceConfiguration(new ConfigurationParams() {
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

        public async Task<IDisposableConfiguration> GetScopedConfiguration(Uri scopeUri)
        {
            var scopes = _server.Services.GetServices<ConfigurationItem>().ToArray();
            if (scopes.Length == 0)
                return new DisposableConfiguration(
                    new ConfigurationBuilder().AddConfiguration(_configuration).Build() as ConfigurationRoot,
                    Disposable.Empty);

            var configurations = await _server.Workspace.WorkspaceConfiguration(new ConfigurationParams() {
                Items = scopes.Select(z => new ConfigurationItem() {Section = z.Section, ScopeUri = scopeUri}).ToArray()
            });

            var data = scopes.Zip(configurations,
                (scope, settings) => (scope.Section, settings));

            var source = new WorkspaceConfigurationSource(data);
            _openScopes.TryAdd(scopeUri, source);
            var configurationBuilder = new ConfigurationBuilder()
                // this avoids chaining the configurations
                // so that the returned configuration object
                // is stateless.
                // scoped configuration should be a snapshot of the current state.
                .AddConfiguration(_configuration)
                .Add(source);
            return new DisposableConfiguration(
                configurationBuilder.Build() as ConfigurationRoot,
                Disposable.Create(
                    () => _openScopes.TryRemove(scopeUri, out _))
            );
        }

        public bool TryGetScopedConfiguration(Uri scopeUri, out IDisposable disposable)
        {
            var result = _openScopes.TryGetValue(scopeUri, out var c);
            if (result)
            {
                disposable = Disposable.Create(() => _openScopes.TryRemove(scopeUri, out _));
                return true;
            }

            disposable = Disposable.Empty;
            return false;
        }
    }

    class DisposableConfiguration : IDisposableConfiguration
    {
        private ConfigurationRoot _configuration;
        private readonly IDisposable _disposable;

        public DisposableConfiguration(ConfigurationRoot configuration, IDisposable disposable)
        {
            _configuration = configuration;
            _disposable = disposable;
        }

        public IConfigurationSection GetSection(string key) => _configuration.GetSection(key);

        public IEnumerable<IConfigurationSection> GetChildren() => _configuration.GetChildren();

        public IChangeToken GetReloadToken() => _configuration.GetReloadToken();

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
