namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// The result of an show document request.
    ///
    /// @since 3.16.0 - proposed state
    /// </summary>
    public class ShowDocumentResult
    {
        /// <summary>
        /// A boolean indicating if the show was successful.
        /// </summary>
        public bool Success { get; set; }
    }
}