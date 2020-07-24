using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [Method(GeneralNames.SetTrace, Direction.ClientToServer)]
    public class SetTraceParams : IRequest
    {
        /// <summary>
        /// The new value that should be assigned to the trace setting.
        /// </summary>
        public InitializeTrace Value { get; set; }
    }
}
