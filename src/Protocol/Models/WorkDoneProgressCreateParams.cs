using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [Method(WindowNames.WorkDoneProgressCreate, Direction.ServerToClient)]
    public class WorkDoneProgressCreateParams : IRequest
    {
        /// <summary>
        /// The token to be used to report progress.
        /// </summary>
        public ProgressToken Token { get; set; }
    }
}
