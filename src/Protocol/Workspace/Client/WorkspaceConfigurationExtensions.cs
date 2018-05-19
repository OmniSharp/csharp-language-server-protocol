using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public static class WorkspaceConfigurationExtensions
    {
        public static Task<Container<JToken>> WorkspaceConfiguration(this ILanguageClientWorkspace router, ConfigurationParams @params)
        {
            return router.SendRequest<ConfigurationParams, Container<JToken>>(WorkspaceNames.WorkspaceConfiguration, @params);
        }
    }
}
