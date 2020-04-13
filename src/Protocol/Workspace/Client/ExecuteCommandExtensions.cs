using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public static class ExecuteCommandExtensions
    {
        public static Task ExecuteCommand(this ILanguageClientWorkspace router, ExecuteCommandParams @params, CancellationToken cancellationToken = default)
        {
            return router.SendRequest(WorkspaceNames.ExecuteCommand, @params, cancellationToken);
        }
    }
}
