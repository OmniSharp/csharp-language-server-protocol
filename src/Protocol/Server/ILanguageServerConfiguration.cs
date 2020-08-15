using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    public interface ILanguageServerConfiguration : IConfiguration
    {
        /// <summary>
        /// Adds a set of configuration items to be tracked by the server
        /// </summary>
        /// <param name="configurationItems"></param>
        /// <returns></returns>
        ILanguageServerConfiguration AddConfigurationItems(IEnumerable<ConfigurationItem> configurationItems);

        /// <summary>
        /// Stops tracking a given set of configuration items
        /// </summary>
        /// <param name="configurationItems"></param>
        /// <returns></returns>
        ILanguageServerConfiguration RemoveConfigurationItems(IEnumerable<ConfigurationItem> configurationItems);

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
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IScopedConfiguration> GetScopedConfiguration(DocumentUri scopeUri, CancellationToken cancellationToken);

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
