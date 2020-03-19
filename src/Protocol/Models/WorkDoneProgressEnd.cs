using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// Signaling the end of a progress reporting is done using the following payload
    /// </summary>
    public class WorkDoneProgressEnd : WorkDoneProgress
    {
        public WorkDoneProgressEnd() : base("end") { }

        /// <summary>
        /// Optional, a final message indicating to for example indicate the outcome
        /// of the operation.
        /// </summary>
        [Optional]
        public string Message { get; set; }
    }
}