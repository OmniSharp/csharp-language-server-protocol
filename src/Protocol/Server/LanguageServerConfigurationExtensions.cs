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
            return configuration.AddConfigurationItems(new[] { configurationItem }.Concat(configurationItems));
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
            return configuration.RemoveConfigurationItems(new[] { configurationItem }.Concat(configurationItems));
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
            return configuration.AddConfigurationItems(new[] { section }.Concat(sections).Select(z => new ConfigurationItem() { Section = z}));
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
            return configuration.RemoveConfigurationItems(new[] { section }.Concat(sections).Select(z => new ConfigurationItem() { Section = z}));
        }

        /// <summary>
        /// Adds a set of configuration items to be tracked by the server
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="sections"></param>
        /// <returns></returns>
        public static ILanguageServerConfiguration AddSections(this ILanguageServerConfiguration configuration, IEnumerable<string> sections)
        {
            return configuration.AddConfigurationItems(sections.Select(z => new ConfigurationItem() { Section = z}));
        }

        /// <summary>
        /// Stops tracking a given set of configuration items
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="sections"></param>
        /// <returns></returns>
        public static ILanguageServerConfiguration RemoveSections(this ILanguageServerConfiguration configuration, IEnumerable<string> sections)
        {
            return configuration.RemoveConfigurationItems(sections.Select(z => new ConfigurationItem() { Section = z}));
        }
    }
}