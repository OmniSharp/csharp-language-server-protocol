using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    public interface ILanguageServerConfiguration : IConfiguration
    {
        /// <summary>
        /// Gets the current configuration values from the client
        /// This configuration object is stateless such that it won't change with any other configuration changes
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        Task<IConfiguration> GetConfiguration(params ConfigurationItem[] items);

        /// <summary>
        /// Gets the current configuration for a given document uri
        /// This re-uses all the sections from the <see cref="ConfigurationItem"/>s that
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
    }
}
