using System.Threading.Tasks;
using JsonRpc;
using Lsp.Models;
// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    public static class ShowMessageExtensions
    {
        public static void ShowMessage(this ILanguageServer mediator, ShowMessageParams @params)
        {
            mediator.SendNotification("window/showMessage", @params);
        }
    }
}