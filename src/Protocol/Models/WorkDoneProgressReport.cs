using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// Reporting progress is done using the following payload
    /// </summary>
    public class WorkDoneProgressReport : WorkDoneProgress
    {
        public WorkDoneProgressReport() : base("report") { }

        /// <summary>
        /// Controls enablement state of a cancel button. This property is only valid if a cancel
        /// button got requested in the `WorkDoneProgressStart` payload.
        ///
        /// Clients that don't support cancellation or don't support control the button's
        /// enablement state are allowed to ignore the setting.
        /// </summary>
        [Optional]
        public bool? Cancellable { get; set; }

        /// <summary>
        /// Optional, more detailed associated progress message. Contains
        /// complementary information to the `title`.
        ///
        /// Examples: "3/25 files", "project/src/module2", "node_modules/some_dep".
        /// If unset, the previous progress message (if any) is still valid.
        /// </summary>
        [Optional]
        public string Message { get; set; }

        /// <summary>
        /// Optional progress percentage to display (value 100 is considered 100%).
        /// If not provided infinite progress is assumed and clients are allowed
        /// to ignore the `percentage` value in subsequent in report notifications.
        ///
        /// The value should be steadily rising. Clients are free to ignore values
        /// that are not following this rule.
        /// </summary>
        [Optional]
        public double? Percentage { get; set; }
    }
}
