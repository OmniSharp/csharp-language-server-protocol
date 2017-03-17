using System.Threading.Tasks;
using JsonRPC;
using Lsp.Models;

namespace Lsp.Protocol
{
    public static partial class ClientNotificationExtensions
    {
        public static Task ShowMessage(this IClient mediator, ShowMessageParams @params)
        {
            return mediator.SendNotification("window/showMessage", @params);
        }
    }
}