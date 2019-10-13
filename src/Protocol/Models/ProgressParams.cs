using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class ProgressParams : IRequest
    {
        public static ProgressParams Create<T>(ProgressToken token, T value, JsonSerializer jsonSerializer)
        {
            return new ProgressParams()
            {
                Token = token,
                Value = JObject.FromObject(value, jsonSerializer)
            };
        }

        /// <summary>
        /// The progress token provided by the client or server.
        /// </summary>
        public ProgressToken Token { get; set; }

        /// <summary>
        /// The progress data.
        /// </summary>
        public JObject Value { get; set; }
    }
}