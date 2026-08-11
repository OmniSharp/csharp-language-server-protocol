using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using Microsoft.Extensions.Configuration;
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

        private readonly ConcurrentDictionary<(string section, DocumentUri? scope), IConfiguration> _scopedConfigurations =
            new ConcurrentDictionary<(string section, DocumentUri? scope), IConfiguration>();

        public TestConfigurationProvider(IWorkspaceLanguageClient workspaceLanguageClient) => _workspaceLanguageClient = workspaceLanguageClient;

        public void Update(string section, IDictionary<string, string>? configuration)
        {
            if (configuration == null) return;
            Update(section, new ConfigurationBuilder().AddInMemoryCollection(configuration).Build());
        }

        public void Update(string section, IConfiguration? configuration)
        {
            if (configuration == null) return;
            Update(section, null, configuration);
        }

        public void Update(string section, DocumentUri documentUri, IDictionary<string, string>? configuration)
        {
            if (configuration == null) return;
            Update(section, documentUri, new ConfigurationBuilder().AddInMemoryCollection(configuration).Build());
        }

        public void Update(string section, DocumentUri? documentUri, IConfiguration? configuration)
        {
            if (configuration == null) return;
            _scopedConfigurations.AddOrUpdate(( section, documentUri ), configuration, (_, _) => configuration);
            TriggerChange();
        }

        public void Reset(string section) => Reset(section, null);

        public void Reset(string section, DocumentUri? documentUri)
        {
            _scopedConfigurations.TryRemove(( section, documentUri ), out _);
            _workspaceLanguageClient.DidChangeConfiguration(new DidChangeConfigurationParams());
            TriggerChange();
        }

        private IConfiguration Get(ConfigurationItem configurationItem)
        {
            if (_scopedConfigurations.TryGetValue(
                    ( configurationItem.Section!, configurationItem.ScopeUri ),
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

        Task<Container<JsonElement>> IRequestHandler<ConfigurationParams, Container<JsonElement>>.Handle(ConfigurationParams request, CancellationToken cancellationToken)
        {
            var results = new List<JsonElement>();
            foreach (var item in request.Items)
            {
                var config = Get(item);
                results.Add(Parse(config.AsEnumerable(true).Where(x => x.Value != null)));
            }

            return Task.FromResult<Container<JsonElement>>(results);
        }

        private JsonElement Parse(IEnumerable<KeyValuePair<string, string?>> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            var result = new JsonObject();
            foreach (var item in values)
            {
                var keys = item.Key.Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                var prop = keys.Last();
                JsonNode root = result;

                // This produces a simple look ahead
                var zippedKeys = keys
                   .Zip(keys.Skip(1), (prev, current) => ( prev, current ));

                foreach (var (key, next) in zippedKeys)
                {
                    if (int.TryParse(next, out _))
                    {
                        root = SetValueToNode(root, key, new JsonArray());
                    }
                    else
                    {
                        root = SetValueToNode(root, key, new JsonObject());
                    }
                }

                SetValueToNode(root, prop, JsonValue.Create(item.Value!)!);
            }

            return JsonSerializer.SerializeToElement(result);
        }

        private T SetValueToNode<T>(JsonNode root, string key, T value)
            where T : JsonNode
        {
            var currentValue = GetValueFromNode(root, key);
            if (currentValue == null)
            {
                if (root is JsonArray arr)
                {
                    if (int.TryParse(key, out var index))
                    {
                        if (arr.Count <= index)
                        {
                            while (arr.Count < index)
                                arr.Add(null);
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

            if (root is JsonArray arr2 && int.TryParse(key, out var i))
            {
                return (T) arr2[i]!;
            }

            return ( root[key] as T )!;
        }

        private static JsonNode? GetValueFromNode(JsonNode root, string key)
        {
            if (root is JsonArray arr)
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
