using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public static class WorkDoneProgressExtensions
    {
        public static void Cancel(this ILanguageClientWindowProgress mediator, ProgressToken token)
        {
            mediator.SendNotification(WindowNames.WorkDoneProgressCancel, new WorkDoneProgressCancelParams()
            {
                Token = token
            });
        }
    }
}
