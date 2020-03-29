using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    public static class WorkspaceConfigurationExtensions
    {
        public static Task<Container<JToken>> WorkspaceConfiguration(this ILanguageServerWorkspace router, ConfigurationParams @params, CancellationToken cancellationToken = default)
        {
            return router.SendRequest<ConfigurationParams, Container<JToken>>(WorkspaceNames.WorkspaceConfiguration, @params, cancellationToken);
        }
    }
}
