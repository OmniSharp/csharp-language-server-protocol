using System.Text.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// To start progress reporting a `$/progress` notification with the following payload must be sent
    /// </summary>
    public class WorkDoneProgressBegin : WorkDoneProgress
    {
        public WorkDoneProgressBegin() : base("begin") { }

        /// <summary>
        /// Mandatory title of the progress operation. Used to briefly inform about
        /// the kind of operation being performed.
        ///
        /// Examples: "Indexing" or "Linking dependencies".
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Controls if a cancel button should show to allow the user to cancel the
        /// long running operation. Clients that don't support cancellation are allowed
        /// to ignore the setting.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public bool? Cancellable { get; set; }

        /// <summary>
        /// Optional, more detailed associated progress message. Contains
        /// complementary information to the `title`.
        ///
        /// Examples: "3/25 files", "project/src/module2", "node_modules/some_dep".
        /// If unset, the previous progress message (if any) is still valid.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public string Message { get; set; }

        /// <summary>
        /// Optional progress percentage to display (value 100 is considered 100%).
        /// If not provided infinite progress is assumed and clients are allowed
        /// to ignore the `percentage` value in subsequent in report notifications.
        ///
        /// The value should be steadily rising. Clients are free to ignore values
        /// that are not following this rule.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public double? Percentage { get; set; }
    }
}
