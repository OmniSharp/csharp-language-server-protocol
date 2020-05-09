using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [Method(WindowNames.LogMessage)]
    public class LogMessageParams : IRequest
    {
        /// <summary>
        ///  The message type. See {@link MessageType}
        /// </summary>
        public MessageType Type { get; set; }

        /// <summary>
        ///  The actual message
        /// </summary>
        public string Message { get; set; }
    }
}
