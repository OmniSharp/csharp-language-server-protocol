using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    public static class WorkspaceConfigurationExtensions
    {
        public static Task<Container<JsonElement>> WorkspaceConfiguration(this ILanguageServerWorkspace router, ConfigurationParams @params, CancellationToken cancellationToken = default)
        {
            return router.SendRequest(@params, cancellationToken);
        }
    }
}
