using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
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
