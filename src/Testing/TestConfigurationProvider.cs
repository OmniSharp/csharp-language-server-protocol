using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Client.Configuration;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

namespace OmniSharp.Extensions.LanguageProtocol.Testing
{
    public class TestConfigurationProvider : IConfigurationHandler
    {
        private readonly IWorkspaceLanguageClient _workspaceLanguageClient;

        private readonly ConcurrentDictionary<(string section, DocumentUri scope), IConfiguration> _scopedConfigurations =
            new ConcurrentDictionary<(string section, DocumentUri scope), IConfiguration>();

        public TestConfigurationProvider(IWorkspaceLanguageClient workspaceLanguageClient) => _workspaceLanguageClient = workspaceLanguageClient;

        public void Update(string section, IDictionary<string, string> configuration)
        {
            if (configuration == null) return;
            Update(section, new ConfigurationBuilder().AddInMemoryCollection(configuration).Build());
        }

        public void Update(string section, IConfiguration configuration)
        {
            if (configuration == null) return;
            Update(section, null, configuration);
        }

        public void Update(string section, DocumentUri documentUri, IDictionary<string, string> configuration)
        {
            if (configuration == null) return;
            Update(section, documentUri, new ConfigurationBuilder().AddInMemoryCollection(configuration).Build());
        }

        public void Update(string section, DocumentUri documentUri, IConfiguration configuration)
        {
            if (configuration == null) return;
            _scopedConfigurations.AddOrUpdate(( section, documentUri ), configuration, (a, _) => configuration);
            TriggerChange();
        }

        public void Reset(string section) => Reset(section, null);

        public void Reset(string section, DocumentUri documentUri)
        {
            _scopedConfigurations.TryRemove(( section, documentUri ), out _);
            _workspaceLanguageClient.DidChangeConfiguration(new DidChangeConfigurationParams());
            TriggerChange();
        }

        private IConfiguration Get(ConfigurationItem configurationItem)
        {
            if (_scopedConfigurations.TryGetValue(
                    ( configurationItem.Section, configurationItem.ScopeUri ),
                    out var configuration
                )
            )
            {
                return new ConfigurationBuilder()
                      .CustomAddConfiguration(configuration, false)
                      .Build();
            }

            return new ConfigurationBuilder().Build();
        }

        private void TriggerChange() => _workspaceLanguageClient.DidChangeConfiguration(new DidChangeConfigurationParams());

        Task<Container<JToken>> IRequestHandler<ConfigurationParams, Container<JToken>>.Handle(ConfigurationParams request, CancellationToken cancellationToken)
        {
            var results = new List<JToken>();
            foreach (var item in request.Items)
            {
                var config = Get(item);
                results.Add(Parse(config.AsEnumerable(true).Where(x => x.Value != null)));
            }

            return Task.FromResult<Container<JToken>>(results);
        }

        private JObject Parse(IEnumerable<KeyValuePair<string, string>> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            var result = new JObject();
            foreach (var item in values)
            {
                var keys = item.Key.Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                var prop = keys.Last();
                JToken root = result;

                // This produces a simple look ahead
                var zippedKeys = keys
                   .Zip(keys.Skip(1), (prev, current) => ( prev, current ));

                foreach (var (key, next) in zippedKeys)
                {
                    if (int.TryParse(next, out var value))
                    {
                        root = SetValueToToken(root, key, new JArray());
                    }
                    else
                    {
                        root = SetValueToToken(root, key, new JObject());
                    }
                }

                SetValueToToken(root, prop, new JValue(item.Value));
            }

            return result;
        }

        private T SetValueToToken<T>(JToken root, string key, T value)
            where T : JToken
        {
            var currentValue = GetValueFromToken(root, key);
            if (currentValue == null || currentValue.Type == JTokenType.Null)
            {
                if (root is JArray arr)
                {
                    if (int.TryParse(key, out var index))
                    {
                        if (arr.Count <= index)
                        {
                            while (arr.Count < index)
                                arr.Add(null!);
                            arr.Add(value);
                        }
                        else
                        {
                            arr[index] = value;
                        }

                        return value;
                    }
                }
                else
                {
                    root[key] = value;
                    return value;
                }
            }

            if (root is JArray arr2 && int.TryParse(key, out var i))
            {
                return (T) arr2[i];
            }

            return root[key] as T;
        }

        private static JToken GetValueFromToken(JToken root, string key)
        {
            if (root is JArray arr)
            {
                if (int.TryParse(key, out var index))
                {
                    if (arr.Count <= index) return null;
                    return arr[index];
                }

                throw new IndexOutOfRangeException(key);
            }

            return root[key];
        }
    }
}
