using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OmniSharp.Extensions.LanguageServer.Server.Configuration
{
    class BaseWorkspaceConfigurationProvider : ConfigurationProvider
    {
        protected void ParseClientConfiguration(JToken settings, string prefix = null)
        {
            if (settings == null || settings.Type == JTokenType.Null || settings.Type == JTokenType.None) return;
            // The null request (appears) to always come second
            // this handler is set to use the SerialAttribute

            Data.Clear();

            // TODO: Figure out the best way to plugin to handle additional configurations (toml, yaml?)
            try
            {
                foreach (var item in
                    JObject.FromObject(settings)
                        .Descendants()
                        .Where(p => !p.Any())
                        .OfType<JValue>()
                        .Select(item =>
                            new KeyValuePair<string, string>(GetKey(item, prefix),
                                item.ToString(CultureInfo.InvariantCulture))))
                {
                    Data[item.Key] = item.Value;
                }
            }
            catch (JsonReaderException)
            {
                // Might not have been json... try xml.
                foreach (var item in
                    XDocument.Parse(settings.ToString())
                        .Descendants()
                        .Where(p => !p.Descendants().Any())
                        .Select(item =>
                            new KeyValuePair<string, string>(GetKey(item, prefix), item.ToString())))
                {
                    Data[item.Key] = item.Value;
                }
            }
        }

        private string GetKey(JToken token, string prefix)
        {
            var items = new Stack<string>();

            while (token.Parent != null)
            {
                if (token.Parent is JArray arr)
                {
                    items.Push(arr.IndexOf(token).ToString());
                }

                if (token is JProperty p)
                {
                    items.Push(p.Name);
                }

                token = token.Parent;
            }

            if (!string.IsNullOrWhiteSpace(prefix))
            {
                items.Push(prefix);
            }

            return string.Join(":", items);
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
