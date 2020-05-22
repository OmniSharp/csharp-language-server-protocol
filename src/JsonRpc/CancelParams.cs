using MediatR;

namespace OmniSharp.Extensions.JsonRpc
{
    [Method(JsonRpcNames.CancelRequest)]
    public class CancelParams : IRequest
    {
        /// <summary>
        /// The request id to cancel.
        /// </summary>
        public object Id { get; set; }
    }
}
