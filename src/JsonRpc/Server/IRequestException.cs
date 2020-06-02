namespace OmniSharp.Extensions.JsonRpc.Server
{
    public interface IRequestException
    {
        /// <summary>
        ///     The LSP / JSON-RPC request Id (if known).
        /// </summary>
        public object RequestId { get; }

        /// <summary>
        ///     The LSP / JSON-RPC error code.
        /// </summary>
        public int ErrorCode { get; }
    }
}
