using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.Extensions.Configuration;

namespace OmniSharp.Extensions.LanguageServer.Server.Configuration
{
    class BaseWorkspaceConfigurationProvider : ConfigurationProvider
    {
        protected void ParseClientConfiguration(JsonElement settings, string prefix = null)
        {
            if (settings.ValueKind == JsonValueKind.Undefined || settings.ValueKind == JsonValueKind.Null) return;
            // The null request (appears) to always come second
            // this handler is set to use the SerialAttribute

            // TODO: Figure out the best way to plugin to handle additional configurations (toml, yaml?)
            try
            {

                foreach (var item in EnumerateValues(settings, prefix ?? ""))
                {
                    Data[item.key] = item.value;
                }
            }
            catch (JsonException)
            {
                // Might not have been json... try xml.
                foreach (var item in
                    XDocument.Parse(settings.GetString())
                        .Descendants()
                        .Where(p => !p.Descendants().Any())
                        .Select(item =>
                            new KeyValuePair<string, string>(GetKey(item, prefix), item.ToString())))
                {
                    Data[item.Key] = item.Value;
                }
            }
        }

        private IEnumerable<(string key, string value)> EnumerateValues(JsonElement element, string prefix)
        {
            if (element.ValueKind == JsonValueKind.Undefined || element.ValueKind == JsonValueKind.Null)
            {
                yield return (prefix, null);
            }

            if (element.ValueKind == JsonValueKind.Object)
            {
                foreach (var property in element.EnumerateObject())
                {
                    foreach (var item in EnumerateValues(property.Value, $"{prefix}:{property.Name}"))
                        yield return item;
                }
                yield break;
            }

            if (element.ValueKind == JsonValueKind.Array)
            {
                var idx = 0;
                foreach (var item in element.EnumerateArray())
                {
                    foreach (var value in EnumerateValues(item, $"{prefix}:{idx++})"))
                        yield return value;
                }
                yield break;
            }

            yield return element.ValueKind switch
            {
                JsonValueKind.False => (prefix, element.GetBoolean().ToString()),
                JsonValueKind.True => (prefix, element.GetBoolean().ToString()),
                JsonValueKind.String => (prefix, element.GetString()),
                JsonValueKind.Number => (prefix, element.GetInt64().ToString()),
                _ => (prefix, null)
            };
        }

        private string GetKey(XElement token, string prefix)
        {
            var items = new Stack<string>();

            while (token.Parent != null)
            {
                if (token.Parent.Elements().Count() > 1)
                {
                    items.Push(Array.IndexOf(token.Parent.Elements().ToArray(), token).ToString());
                }

                items.Push(token.Name.ToString());

                token = token.Parent;
            }

            if (!string.IsNullOrWhiteSpace(prefix))
            {
                items.Push(prefix);
            }

            return string.Join(":", items);
        }
    }
}
