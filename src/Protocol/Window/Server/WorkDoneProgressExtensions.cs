using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    public static class WorkDoneProgressExtensions
    {
        public static async Task Create(this ILanguageServerWindowProgress mediator, WorkDoneProgressCreateParams @params, CancellationToken cancellationToken = default)
        {
            await mediator.SendRequest(@params, cancellationToken);
        }

        public static void Cancel(this ILanguageServerWindowProgress mediator, WorkDoneProgressCancelParams @params)
        {
            mediator.SendNotification(@params);
        }
    }
}
