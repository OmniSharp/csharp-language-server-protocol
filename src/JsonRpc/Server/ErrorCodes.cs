namespace OmniSharp.Extensions.JsonRpc.Server
{
    /// <summary>
    /// Well-known LSP error codes.
    /// </summary>
    public static class ErrorCodes
    {
        /// <summary>
        /// No error code was supplied.
        /// </summary>
        public const int UnknownErrorCode = -32001;

        /// <summary>
        /// An exception was thrown by a .net server / client
        /// </summary>
        public const int Exception = -32050;

        /// <summary>
        /// Server has not been initialised.
        /// </summary>
        public const int ServerNotInitialized = -32002;

        /// <summary>
        /// Method not found.
        /// </summary>
        public const int MethodNotSupported = -32601;

        /// <summary>
        /// Invalid request.
        /// </summary>
        public const int InvalidRequest = -32600;

        /// <summary>
        /// Invalid request parameters.
        /// </summary>
        public const int InvalidParameters = -32602;

        /// <summary>
        /// Internal error.
        /// </summary>
        public const int InternalError = -32603;

        /// <summary>
        /// Unable to parse request.
        /// </summary>
        public const int ParseError = -32700;

        /// <summary>
        /// Request was cancelled.
        /// </summary>
        public const int RequestCancelled = -32800;

        /// <summary>
        /// Content was modified
        /// </summary>
        public const int ContentModified = -32801;

        /// <summary>
        /// A request failed but it was syntactically correct, e.g the
        /// method name was known and the parameters were valid. The error
        /// message should contain human readable information about why
        /// the request failed.
        /// @since 3.17.0
        /// </summary>
        public const int RequestFailed = -32803;

        /// <summary>
        /// The server cancelled the request. This error code should
        /// only be used for requests that explicitly support being
        /// server cancellable.
        /// @since 3.17.0
        /// </summary>
        public const int ServerCancelled = -32802;
    }
}
