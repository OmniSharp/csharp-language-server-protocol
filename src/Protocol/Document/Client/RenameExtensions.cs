using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public static class RenameExtensions
    {
        public static Task<WorkspaceEdit> Rename(this ILanguageClientDocument mediator, RenameParams @params)
        {
            return mediator.SendRequest<RenameParams, WorkspaceEdit>(DocumentNames.Rename, @params);
        }
    }
}
