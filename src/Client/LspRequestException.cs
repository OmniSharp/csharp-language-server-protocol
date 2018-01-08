using System;
using System.Runtime.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Client
{
    /// <summary>
    ///     Exception raised when a Language Server Protocol error is encountered while processing a request.
    /// </summary>
    [Serializable]
    public class LspRequestException
        : LspException
    {
        /// <summary>
        ///     The request Id used when no valid request Id was supplied.
        /// </summary>
        public const string UnknownRequestId = "(unknown)";

        /// <summary>
        ///     Create a new <see cref="LspRequestException"/> without an error code (<see cref="LspErrorCodes.None"/>).
        /// </summary>
        /// <param name="message">
        ///     The exception message.
        /// </param>
        /// <param name="requestId">
        ///     The LSP / JSON-RPC request Id (if known).
        /// </param>
        public LspRequestException(string message, string requestId)
            : this(message, requestId, LspErrorCodes.None)
        {
        }

        /// <summary>
        ///     Create a new <see cref="LspRequestException"/>.
        /// </summary>
        /// <param name="message">
        ///     The exception message.
        /// </param>
        /// <param name="requestId">
        ///     The LSP / JSON-RPC request Id (if known).
        /// </param>
        /// <param name="errorCode">
        ///     The LSP / JSON-RPC error code.
        /// </param>
        public LspRequestException(string message, string requestId, int errorCode)
            : base(message)
        {
            RequestId = !string.IsNullOrWhiteSpace(requestId) ? requestId : UnknownRequestId;
            ErrorCode = errorCode;
        }

        /// <summary>
        ///     Create a new <see cref="LspRequestException"/>.
        /// </summary>
        /// <param name="message">
        ///     The exception message.
        /// </param>
        /// <param name="requestId">
        ///     The LSP / JSON-RPC request Id (if known).
        /// </param>
        /// <param name="inner">
        ///     The exception that caused this exception to be raised.
        /// </param>
        public LspRequestException(string message, string requestId, Exception inner)
            : this(message, requestId, LspErrorCodes.None, inner)
        {
        }

        /// <summary>
        ///     Create a new <see cref="LspRequestException"/>.
        /// </summary>
        /// <param name="message">
        ///     The exception message.
        /// </param>
        /// <param name="requestId">
        ///     The LSP / JSON-RPC request Id (if known).
        /// </param>
        /// <param name="errorCode">
        ///     The LSP / JSON-RPC error code.
        /// </param>
        /// <param name="inner">
        ///     The exception that caused this exception to be raised.
        /// </param>
        public LspRequestException(string message, string requestId, int errorCode, Exception inner)
            : base(message, inner)
        {
            RequestId = !string.IsNullOrWhiteSpace(requestId) ? requestId : UnknownRequestId;
            ErrorCode = errorCode;
        }

        /// <summary>
        ///     Serialisation constructor.
        /// </summary>
        /// <param name="info">
        ///     The serialisation data-store.
        /// </param>
        /// <param name="context">
        ///     The serialisation streaming context.
        /// </param>
        protected LspRequestException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            RequestId = info.GetString(nameof(RequestId));
            ErrorCode = info.GetInt32(nameof(ErrorCode));
        }

        /// <summary>
        ///     Get exception data for serialisation.
        /// </summary>
        /// <param name="info">
        ///     The serialisation data-store.
        /// </param>
        /// <param name="context">
        ///     The serialisation streaming context.
        /// </param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(nameof(RequestId), RequestId);
            info.AddValue(nameof(ErrorCode), ErrorCode);
        }

        /// <summary>
        ///     The LSP / JSON-RPC request Id (if known).
        /// </summary>
        public string RequestId { get; }

        /// <summary>
        ///     The LSP / JSON-RPC error code.
        /// </summary>
        public int ErrorCode { get; }

        /// <summary>
        ///     Does the <see cref="LspRequestException"/> represent an LSP / JSON-RPC protocol error?
        /// </summary>
        public bool IsProtocolError => ErrorCode != LspErrorCodes.None;
    }
}