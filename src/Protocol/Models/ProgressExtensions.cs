using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public static class ProgressExtensions
    {
        public static void SendProgress<T>(this IResponseRouter mediator, ProgressToken token, T value,
            JsonSerializerOptions options)
        {
            mediator.SendNotification(ProgressParams.Create(token, value));
        }

        public static void SendProgress(this IResponseRouter mediator, ProgressParams @params)
        {
            mediator.SendNotification(@params);
        }

        public static Task CreateProgress(this IResponseRouter mediator, ProgressToken token,
            CancellationToken cancellationToken)
        {
            return mediator.SendRequest(new WorkDoneProgressCreateParams() {Token = token}, cancellationToken);
        }
    }
}
