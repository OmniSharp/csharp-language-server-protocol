using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class ApplyWorkspaceEditResponse
    {
        /// <summary>
        /// Indicates whether the edit was applied or not.
        /// </summary>
        public bool Applied { get; set; }

        /// <summary>
        /// An optional textual description for why the edit was not applied.
        /// This may be used may be used by the server for diagnostic
        /// logging or to provide a suitable error for a request that
        /// triggered the edit.
        /// </summary>
        [Optional]
        public string? FailureReason { get; set; }

        /// <summary>
        /// Depending on the client's failure handling strategy `failedChange`
        /// might contain the index of the change that failed. This property is
        /// only available if the client signals a `failureHandlingStrategy`
        /// in its client capabilities.
        /// </summary>
        [Optional]
        public int? FailedChange { get; set; }
    }
}
