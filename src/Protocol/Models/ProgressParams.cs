using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [Method(GeneralNames.Progress, Direction.Bidirectional)]
    public class ProgressParams : IRequest
    {
        public static ProgressParams Create<T>(ProgressToken token, T value, JsonSerializer jsonSerializer)
        {
            return new ProgressParams()
            {
                Token = token,
                Value = JToken.FromObject(value, jsonSerializer)
            };
        }

        /// <summary>
        /// The progress token provided by the client or server.
        /// </summary>
        public ProgressToken Token { get; set; }

        /// <summary>
        /// The progress data.
        /// </summary>
        public JToken Value { get; set; }
    }
}
