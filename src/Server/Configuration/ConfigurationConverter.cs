using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;

namespace OmniSharp.Extensions.LanguageServer.Server.Configuration
{
    class ConfigurationConverter
    {
        public void ParseClientConfiguration(IDictionary<string, string?> data, JsonElement? settings, string? prefix = null)
        {
            if (settings is null || settings.Value.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined) return;
            // The null request (appears) to always come second
            // this handler is set to use the SerialAttribute

            // TODO: Figure out the best way to plugin to handle additional configurations (toml, yaml?)
            ParseElement(data, settings.Value, prefix ?? string.Empty);
        }

        private static void ParseElement(IDictionary<string, string?> data, JsonElement element, string key)
        {
            if (element.ValueKind == JsonValueKind.Object)
            {
                foreach (var property in element.EnumerateObject())
                {
                    ParseElement(data, property.Value, CombineKey(key, property.Name));
                }

                return;
            }

            if (element.ValueKind == JsonValueKind.Array)
            {
                var index = 0;
                foreach (var item in element.EnumerateArray())
                {
                    ParseElement(data, item, CombineKey(key, index.ToString(CultureInfo.InvariantCulture)));
                    index++;
                }

                return;
            }

            data[key] = element.ValueKind == JsonValueKind.String ? element.GetString() ?? string.Empty : element.ToString();
        }

        private static string CombineKey(string prefix, string key) => string.IsNullOrWhiteSpace(prefix) ? key : $"{prefix}:{key}";
    }
}
