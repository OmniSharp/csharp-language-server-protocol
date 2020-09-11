using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using static System.Reactive.Linq.Observable;
using Unit = MediatR.Unit;

namespace OmniSharp.Extensions.LanguageServer.Server.Configuration
{
    internal class DidChangeConfigurationProvider : ConfigurationProvider, IDidChangeConfigurationHandler, IOnLanguageServerStarted, ILanguageServerConfiguration, IDisposable
    {
        private readonly HashSet<ConfigurationItem> _configurationItems = new HashSet<ConfigurationItem>();
        private readonly ILogger<DidChangeConfigurationProvider> _logger;
        private readonly IWorkspaceLanguageServer _workspaceLanguageServer;
        private readonly ConfigurationConverter _configurationConverter;
        private DidChangeConfigurationCapability _capability;
        private readonly ConfigurationRoot _configuration;
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        private readonly ConcurrentDictionary<DocumentUri, ScopedConfiguration> _openScopes =
            new ConcurrentDictionary<DocumentUri, ScopedConfiguration>();

        private readonly IObserver<System.Reactive.Unit> _triggerChange;

        public DidChangeConfigurationProvider(
            Action<IConfigurationBuilder> configurationBuilderAction,
            ILogger<DidChangeConfigurationProvider> logger,
            IWorkspaceLanguageServer workspaceLanguageServer,
            ConfigurationConverter configurationConverter
        )
        {
            _logger = logger;
            _workspaceLanguageServer = workspaceLanguageServer;
            _configurationConverter = configurationConverter;
            var builder = new ConfigurationBuilder()
               .Add(new DidChangeConfigurationSource(this));
            configurationBuilderAction(builder);
            _configuration = builder.Build() as ConfigurationRoot;

            var triggerChange = new Subject<System.Reactive.Unit>();
            _compositeDisposable.Add(triggerChange);
            _triggerChange = triggerChange;
            if (_configuration is IDisposable disposableConfiguration) _compositeDisposable.Add(disposableConfiguration);
            _compositeDisposable.Add(triggerChange.Throttle(TimeSpan.FromMilliseconds(50)).Select(_ => GetWorkspaceConfiguration()).Switch().Subscribe());
        }

        public Task<Unit> Handle(DidChangeConfigurationParams request, CancellationToken cancellationToken)
        {
            if (_capability == null) return Unit.Task;
            // null means we need to re-read the configuration
            // https://github.com/Microsoft/vscode-languageserver-node/issues/380
            if (request.Settings == null || request.Settings.Type == JTokenType.Null)
            {
                _triggerChange.OnNext(System.Reactive.Unit.Default);
                return Unit.Task;
            }

            Data.Clear();
            _configurationConverter.ParseClientConfiguration(Data, request.Settings);
            OnReload();
            return Unit.Task;
        }

        public object GetRegistrationOptions() => new object();

        public void SetCapability(DidChangeConfigurationCapability capability) => _capability = capability;
        public bool IsSupported => _capability != null;

        Task IOnLanguageServerStarted.OnStarted(ILanguageServer server, CancellationToken cancellationToken) => GetWorkspaceConfigurationAsync(cancellationToken);

        private Task GetWorkspaceConfigurationAsync(CancellationToken cancellationToken) => GetWorkspaceConfiguration().LastOrDefaultAsync().ToTask(cancellationToken);

        private IObservable<System.Reactive.Unit> GetWorkspaceConfiguration()
        {
            if (_capability == null || _configurationItems.Count == 0)
            {
                _logger.LogWarning("No ConfigurationItems have been defined, configuration won't surface any configuration from the client!");
                OnReload();
                return Empty<System.Reactive.Unit>();
            }

            return Concat(
                Create<System.Reactive.Unit>(
                    observer => {
                        var newData = new Dictionary<string, string>();
                        return GetConfigurationFromClient(_configurationItems)
                              .Select(
                                   x => {
                                       var (dataItem, settings) = x;
                                       var key = dataItem.ScopeUri != null ? $"{dataItem.ScopeUri}:{dataItem.Section}" : dataItem.Section;
                                       _configurationConverter.ParseClientConfiguration(newData, settings, key);
                                       return System.Reactive.Unit.Default;
                                   }
                               )
                              .Catch<System.Reactive.Unit, Exception>(
                                   e => {
                                       _logger.LogError(e, "Unable to get configuration from client!");
                                       return Empty<System.Reactive.Unit>();
                                   }
                               )
                              .Do(
                                   _ => { }, () => {
                                       Data = newData;
                                   }
                               )
                              .Subscribe(observer);
                    }
                ),
                Create<System.Reactive.Unit>(
                    observer => {
                        var scopedConfigurationItems = _configurationItems
                                                      .Where(z => z.ScopeUri == null)
                                                      .SelectMany(
                                                           scope =>
                                                               _openScopes.Keys.Select(
                                                                   scopeUri => new ConfigurationItem { ScopeUri = scopeUri, Section = scope.Section }
                                                               )
                                                       ).ToArray();
                        return GetConfigurationFromClient(scopedConfigurationItems)
                              .GroupBy(z => z.scope.ScopeUri, z => ( z.scope.Section, z.settings ))
                              .Select(z => z.ToArray().Select(items => ( key: z.Key, items )))
                              .Concat()
                              .Do(
                                   group => {
                                       if (!_openScopes.TryGetValue(group.key, out var source)) return;
                                       source.Update(group.items);
                                   }
                               )
                              .Select(x => System.Reactive.Unit.Default)
                              .Subscribe(observer);
                    }
                ),
                // Ensure we don't trigger reload until scoped configurations are loaded
                Create<System.Reactive.Unit>(
                    o => {
                        OnReload();
                        o.OnCompleted();
                        return Disposable.Empty;
                    })
            );
        }

        public IConfigurationSection GetSection(string key) => _configuration.GetSection(key);

        public IEnumerable<IConfigurationSection> GetChildren() => _configuration.GetChildren();

        public IChangeToken GetReloadToken() => _configuration.GetReloadToken();

        public string this[string key]
        {
            get => _configuration[key];
            set => _configuration[key] = value;
        }

        public ILanguageServerConfiguration AddConfigurationItems(IEnumerable<ConfigurationItem> configurationItems)
        {
            foreach (var item in configurationItems)
                _configurationItems.Add(item);

            _triggerChange.OnNext(System.Reactive.Unit.Default);

            return this;
        }

        public ILanguageServerConfiguration RemoveConfigurationItems(IEnumerable<ConfigurationItem> configurationItems)
        {
            foreach (var item in configurationItems)
                _configurationItems.Remove(item);

            _triggerChange.OnNext(System.Reactive.Unit.Default);

            return this;
        }

        public async Task<IConfiguration> GetConfiguration(params ConfigurationItem[] items)
        {
            if (_capability == null || items.Length == 0)
            {
                return new ConfigurationBuilder().AddInMemoryCollection(_configuration.AsEnumerable()).Build();
            }

            var configurations = await _workspaceLanguageServer.RequestConfiguration(
                new ConfigurationParams {
                    Items = items
                }
            ).ConfigureAwait(false);
            var data = items.Zip(
                configurations,
                (scope, settings) => ( scope.Section, settings )
            );
            return new ConfigurationBuilder()
                   // this avoids chaining the configurations
                   // so that the returned configuration object
                   // is stateless.
                   // scoped configuration should be a snapshot of the current state.
                  .AddInMemoryCollection(_configuration.AsEnumerable())
                  .Add(new WorkspaceConfigurationSource(_configurationConverter, data))
                  .Build();
        }

        public async Task<IScopedConfiguration> GetScopedConfiguration(DocumentUri scopeUri, CancellationToken cancellationToken)
        {
            var scopes = _configurationItems.ToArray();
            if (scopes.Length == 0)
                return EmptyDisposableConfiguration.Instance;

            var data = await GetConfigurationFromClient(scopes.Select(z => new ConfigurationItem { Section = z.Section, ScopeUri = scopeUri }))
                            .Select(z => (z.scope.Section, z.settings))
                            .ToArray()
                            .ToTask(cancellationToken)
                            .ConfigureAwait(false);

            var config = new ScopedConfiguration(
                _configuration,
                _configurationConverter,
                data,
                Disposable.Create(
                    () => _openScopes.TryRemove(scopeUri, out _)
                )
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

        public void Dispose()
        {
            _compositeDisposable?.Dispose();
        }

        private IObservable<(ConfigurationItem scope, JToken settings)> GetConfigurationFromClient(
            IEnumerable<ConfigurationItem> configurationItems
        )
        {
            return FromAsync(
                    ct => _workspaceLanguageServer.RequestConfiguration(
                        new ConfigurationParams {
                            Items = configurationItems.ToArray()
                        }, cancellationToken: ct
                    )
                ).SelectMany(a => a.ToArray())
                 .Zip(configurationItems, (settings, scope) => ( scope, settings ))
                 .Catch<(ConfigurationItem scope, JToken settings), Exception>(
                      e => {
                          _logger.LogError(e, "Unable to get configuration from client!");
                          return Empty<(ConfigurationItem scope, JToken settings)>();
                      }
                  );
        }
    }
}
