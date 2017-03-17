using System.Threading.Tasks;
using JsonRPC;
using Lsp.Models;
// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    public static class UnregisterCapabilityExtensions
    {
        public static Task UnregisterCapability(this IClient mediator, UnregistrationParams @params)
        {
            return mediator.SendRequest("client/unregisterCapability", @params);
        }
    }
}