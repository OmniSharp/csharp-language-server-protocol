using System.Threading.Tasks;
using JsonRpc;
using Lsp.Models;
// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    public static class ShowMessageExtensions
    {
        public static Task ShowMessage(this IClient mediator, ShowMessageParams @params)
        {
            return mediator.SendNotification("window/showMessage", @params);
        }
    }
}