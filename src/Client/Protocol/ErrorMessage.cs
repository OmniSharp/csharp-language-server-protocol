using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Client.Protocol
{
    /// <summary>
    ///     A JSON-RPC error message.
    /// </summary>
    public class ErrorMessage
    {
        /// <summary>
        ///     The error code.
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        ///     The error message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        ///     Optional data associated with the message.
        /// </summary>
        [Optional]
        public JToken Data { get; set; }
    }
}
