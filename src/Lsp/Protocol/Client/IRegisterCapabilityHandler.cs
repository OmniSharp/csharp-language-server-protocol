using System.Threading.Tasks;
using JsonRPC;
using Lsp.Models;

namespace Lsp.Protocol
{
    public static partial class ResponseHandlerExtensions
    {
        public static Task RegisterCapability(this IClient mediator,  RegistrationParams @params)
        {
            return mediator.SendRequest("client/registerCapability", @params);
        }
    }
}