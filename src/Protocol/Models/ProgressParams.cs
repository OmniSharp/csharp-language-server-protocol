using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [Method(GeneralNames.Progress)]
    public class ProgressParams : IRequest
    {
        public static ProgressParams Create<T>(ProgressToken token, T value)
        {
            return new ProgressParams()
            {
                Token = token,
                Value = value
            };
        }

        /// <summary>
        /// The progress token provided by the client or server.
        /// </summary>
        public ProgressToken Token { get; set; }

        /// <summary>
        /// The progress data.
        /// </summary>
        public object Value { get; set; }
    }
}
