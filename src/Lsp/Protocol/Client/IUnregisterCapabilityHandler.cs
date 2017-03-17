using System.Threading.Tasks;
using JsonRPC;
using Lsp.Models;

namespace Lsp.Protocol
{
    public static partial class ResponseHandlerExtensions
    {
        public static Task UnregisterCapability(this IClient mediator, UnregistrationParams @params)
        {
            return mediator.SendRequest("client/unregisterCapability", @params);
        }
    }
}