using MediatR;

namespace OmniSharp.Extensions.JsonRpc
{
    [Method(JsonRpcNames.CancelRequest)]
    public partial class CancelParams : IRequest<Unit>
    {
        /// <summary>
        /// The request id to cancel.
        /// </summary>
        public object Id { get; set; } = null!;
    }
}
