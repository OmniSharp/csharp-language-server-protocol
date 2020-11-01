using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [Method(GeneralNames.LogTrace, Direction.ServerToClient)]
    public class LogTraceParams : IRequest
    {
        /// <summary>
        /// The message to be logged.
        /// </summary>
        public string Message { get; set; } = null!;

        /// <summary>
        /// Additional information that can be computed if the `trace` configuration is set to `'verbose'`
        /// </summary>
        [Optional]
        public string? Verbose { get; set; }
    }
}
