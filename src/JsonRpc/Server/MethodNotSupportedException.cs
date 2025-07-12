using System.Runtime.Serialization;

namespace OmniSharp.Extensions.JsonRpc.Server
{
    /// <summary>
    /// Exception raised when an LSP request is made for a method not supported by the remote process.
    /// </summary>
    [Serializable]
    public class MethodNotSupportedException
        : RequestException
    {
        /// <summary>
        /// Create a new <see cref="MethodNotSupportedException" />.
        /// </summary>
        /// <param name="requestId">
        /// The LSP / JSON-RPC request Id (if known).
        /// </param>
        /// <param name="method">
        /// The name of the target method.
        /// </param>
        public MethodNotSupportedException(object? requestId, string? method)
            : base(ErrorCodes.MethodNotSupported, requestId, $"Method not found: '{method}'.") =>
            Method = !string.IsNullOrWhiteSpace(method) ? method! : "(unknown)";

        /// <summary>
        /// Serialisation constructor.
        /// </summary>
        /// <param name="info">
        /// The serialisation data-store.
        /// </param>
        /// <param name="context">
        /// The serialisation streaming context.
        /// </param>
        protected MethodNotSupportedException(SerializationInfo info, StreamingContext context)
            : base(info, context) =>
            Method = info.GetString("Method");

        /// <summary>
        /// Get exception data for serialisation.
        /// </summary>
        /// <param name="info">
        /// The serialisation data-store.
        /// </param>
        /// <param name="context">
        /// The serialisation streaming context.
        /// </param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("Method", Method);
        }

        /// <summary>
        /// The name of the method that was not supported by the remote process.
        /// </summary>
        public string Method { get; }
    }
}
