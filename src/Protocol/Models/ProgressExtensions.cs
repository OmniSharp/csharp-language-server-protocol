using Newtonsoft.Json;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public static class ProgressExtensions
    {
        public static void SendProgress<T>(this IResponseRouter mediator, ProgressToken token, T value, JsonSerializer jsonSerializer)
        {
            mediator.SendNotification(GeneralNames.Progress, ProgressParams.Create<T>(token, value, jsonSerializer));
        }

        public static void SendProgress(this IResponseRouter mediator, ProgressParams @params)
        {
            mediator.SendNotification(GeneralNames.Progress, @params);
        }
    }
}