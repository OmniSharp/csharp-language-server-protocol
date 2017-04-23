using System.Threading.Tasks;
using JsonRpc;
using Lsp.Models;
// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    public static class RegisterCapabilityExtensions
    {
        public static Task RegisterCapability(this IResponseRouter mediator,  RegistrationParams @params)
        {
            return mediator.SendRequest("client/registerCapability", @params);
        }
    }
}