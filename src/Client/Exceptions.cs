using System;
using System.Runtime.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Client
{
    /// <summary>
    ///     Exception raised when a Language Server Protocol error is encountered.
    /// </summary>
    [Serializable]
    public class LspException
        : Exception
    {
        /// <summary>
        ///     Create a new <see cref="LspException"/>.
        /// </summary>
        /// <param name="message">
        ///     The exception message.
        /// </param>
        public LspException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///     Create a new <see cref="LspException"/>.
        /// </summary>
        /// <param name="message">
        ///     The exception message.
        /// </param>
        /// <param name="inner">
        ///     The exception that caused this exception to be raised.
        /// </param>
        public LspException(string message, Exception inner)
            : base(message, inner)
        {
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
        protected LspException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

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

    /// <summary>
    ///     Exception raised when an LSP request is made for a method not supported by the remote process.
    /// </summary>
    [Serializable]
    public class LspMethodNotSupportedException
        : LspRequestException
    {
        /// <summary>
        ///     Create a new <see cref="LspMethodNotSupportedException"/>.
        /// </summary>
        /// <param name="requestId">
        ///     The LSP / JSON-RPC request Id (if known).
        /// </param>
        /// <param name="method">
        ///     The name of the target method.
        /// </param>
        public LspMethodNotSupportedException(string method, string requestId)
            : base($"Method not found: '{method}'.", requestId, LspErrorCodes.MethodNotSupported)
        {
            Method = !string.IsNullOrWhiteSpace(method) ? method : "(unknown)";
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
        protected LspMethodNotSupportedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Method = info.GetString("Method");
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

            info.AddValue("Method", Method);
        }

        /// <summary>
        ///     The name of the method that was not supported by the remote process.
        /// </summary>
        public string Method { get; }
    }

    /// <summary>
    ///     Exception raised an LSP request is invalid.
    /// </summary>
    [Serializable]
    public class LspInvalidRequestException
        : LspRequestException
    {
        /// <summary>
        ///     Create a new <see cref="LspInvalidRequestException"/>.
        /// </summary>
        /// <param name="requestId">
        ///     The LSP / JSON-RPC request Id (if known).
        /// </param>
        public LspInvalidRequestException(string requestId)
            : base("Invalid request.", requestId, LspErrorCodes.InvalidRequest)
        {
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
        protected LspInvalidRequestException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    /// <summary>
    ///     Exception raised when request parameters are invalid according to the target method.
    /// </summary>
    [Serializable]
    public class LspInvalidParametersException
        : LspRequestException
    {
        /// <summary>
        ///     Create a new <see cref="LspInvalidParametersException"/>.
        /// </summary>
        /// <param name="requestId">
        ///     The LSP / JSON-RPC request Id (if known).
        /// </param>
        public LspInvalidParametersException(string requestId)
            : base("Invalid parameters.", requestId, LspErrorCodes.InvalidParameters)
        {
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
        protected LspInvalidParametersException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    /// <summary>
    ///     Exception raised when an internal error has occurred in the language server.
    /// </summary>
    [Serializable]
    public class LspInternalErrorException
        : LspRequestException
    {
        /// <summary>
        ///     Create a new <see cref="LspInternalErrorException"/>.
        /// </summary>
        /// <param name="requestId">
        ///     The LSP / JSON-RPC request Id (if known).
        /// </param>
        public LspInternalErrorException(string requestId)
            : base("Internal error.", requestId, LspErrorCodes.InternalError)
        {
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
        protected LspInternalErrorException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    /// <summary>
    ///     Exception raised when an LSP request could not be parsed.
    /// </summary>
    [Serializable]
    public class LspParseErrorException
        : LspRequestException
    {
        /// <summary>
        ///     Create a new <see cref="LspParseErrorException"/>.
        /// </summary>
        /// <param name="requestId">
        ///     The LSP / JSON-RPC request Id (if known).
        /// </param>
        public LspParseErrorException(string requestId)
            : base("Error parsing request.", requestId, LspErrorCodes.ParseError)
        {
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
        protected LspParseErrorException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    /// <summary>
    ///     Exception raised when an LSP request is cancelled.
    /// </summary>
    [Serializable]
    public class LspRequestCancelledException
        : LspRequestException
    {
        /// <summary>
        ///     Create a new <see cref="LspRequestCancelledException"/>.
        /// </summary>
        /// <param name="requestId">
        ///     The LSP / JSON-RPC request Id (if known).
        /// </param>
        public LspRequestCancelledException(string requestId)
            : base("Request was cancelled.", requestId, LspErrorCodes.RequestCancelled)
        {
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
        protected LspRequestCancelledException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
