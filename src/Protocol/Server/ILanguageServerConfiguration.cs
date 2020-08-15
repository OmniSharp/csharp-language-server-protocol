using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    public static class LanguageServerConfigurationExtensions
    {
        /// <summary>
        /// Adds a set of configuration items to be tracked by the server
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="configurationItem"></param>
        /// <param name="configurationItems"></param>
        /// <returns></returns>
        public static ILanguageServerConfiguration AddConfigurationItem(this ILanguageServerConfiguration configuration, ConfigurationItem configurationItem, params ConfigurationItem[] configurationItems)
        {
            return configuration.AddConfigurationItem(new[] { configurationItem }.Concat(configurationItems));
        }

        /// <summary>
        /// Stops tracking a given set of configuration items
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="configurationItem"></param>
        /// <param name="configurationItems"></param>
        /// <returns></returns>
        public static ILanguageServerConfiguration RemoveConfigurationItem(this ILanguageServerConfiguration configuration, ConfigurationItem configurationItem, params ConfigurationItem[] configurationItems)
        {
            return configuration.RemoveConfigurationItem(new[] { configurationItem }.Concat(configurationItems));
        }

        /// <summary>
        /// Adds a set of configuration items to be tracked by the server
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="section"></param>
        /// <param name="sections"></param>
        /// <returns></returns>
        public static ILanguageServerConfiguration AddSection(this ILanguageServerConfiguration configuration, string section, params string[] sections)
        {
            return configuration.AddConfigurationItem(new[] { section }.Concat(sections).Select(z => new ConfigurationItem() { Section = z}));
        }

        /// <summary>
        /// Stops tracking a given set of configuration items
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="section"></param>
        /// <param name="sections"></param>
        /// <returns></returns>
        public static ILanguageServerConfiguration RemoveSection(this ILanguageServerConfiguration configuration, string section, params string[] sections)
        {
            return configuration.RemoveConfigurationItem(new[] { section }.Concat(sections).Select(z => new ConfigurationItem() { Section = z}));
        }

        /// <summary>
        /// Adds a set of configuration items to be tracked by the server
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="sections"></param>
        /// <returns></returns>
        public static ILanguageServerConfiguration AddSections(this ILanguageServerConfiguration configuration, IEnumerable<string> sections)
        {
            return configuration.AddConfigurationItem(sections.Select(z => new ConfigurationItem() { Section = z}));
        }

        /// <summary>
        /// Stops tracking a given set of configuration items
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="sections"></param>
        /// <returns></returns>
        public static ILanguageServerConfiguration RemoveSections(this ILanguageServerConfiguration configuration, IEnumerable<string> sections)
        {
            return configuration.RemoveConfigurationItem(sections.Select(z => new ConfigurationItem() { Section = z}));
        }
    }

    public interface ILanguageServerConfiguration : IConfiguration
    {
        /// <summary>
        /// Adds a set of configuration items to be tracked by the server
        /// </summary>
        /// <param name="configurationItems"></param>
        /// <returns></returns>
        ILanguageServerConfiguration AddConfigurationItem(IEnumerable<ConfigurationItem> configurationItems);

        /// <summary>
        /// Stops tracking a given set of configuration items
        /// </summary>
        /// <param name="configurationItems"></param>
        /// <returns></returns>
        ILanguageServerConfiguration RemoveConfigurationItem(IEnumerable<ConfigurationItem> configurationItems);

        /// <summary>
        /// Gets the current configuration values from the client
        /// This configuration object is stateless such that it won't change with any other configuration changes
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        Task<IConfiguration> GetConfiguration(params ConfigurationItem[] items);

        /// <summary>
        /// Gets the current configuration for a given document uri
        /// This re-uses all the sections from the <see cref="ConfigurationItem" />s that
        /// the root configuration uses.
        ///
        /// This will watch for changes of the scoped documents and update the configuration.
        /// </summary>
        /// <param name="scopeUri"></param>
        /// <returns></returns>
        Task<IScopedConfiguration> GetScopedConfiguration(DocumentUri scopeUri);

        /// <summary>
        /// Attempt to get an existing scoped configuration so that it can be disposed
        /// </summary>
        /// <param name="scopeUri"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        bool TryGetScopedConfiguration(DocumentUri scopeUri, out IScopedConfiguration configuration);

        /// <summary>
        /// Is configuration supported by the client
        /// </summary>
        bool IsSupported { get; }
    }
}
