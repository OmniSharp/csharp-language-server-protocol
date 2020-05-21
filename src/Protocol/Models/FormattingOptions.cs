using System.Collections.Generic;
using Newtonsoft.Json;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// Value-object describing what options formatting should use.
    /// </summary>
    [JsonDictionary]
    public class FormattingOptions : Dictionary<string, BooleanNumberString>
    {
        /// <summary>
        /// Size of a tab in spaces.
        /// </summary>
        [JsonIgnore]
        public long TabSize
        {
            get
            {
                return this["tabSize"].IsLong ? this["tabSize"].Long : -1;
            }
            set
            {
                this["tabSize"] = value;
            }
        }

        /// <summary>
        /// Prefer spaces over tabs.
        /// </summary>
        [JsonIgnore]
        public bool InsertSpaces
        {
            get
            {
                return this["insertSpaces"].IsBool ? this["insertSpaces"].Bool : false;
            }
            set
            {
                this["insertSpaces"] = value;
            }
        }

        /// <summary>
        /// Trim trailing whitespaces on a line.
        ///
        /// @since 3.15.0
        /// </summary>
        [JsonIgnore]
        public bool TrimTrailingWhitespace
        {
            get
            {
                return this["trimTrailingWhitespace"].IsBool ? this["trimTrailingWhitespace"].Bool : false;
            }
            set
            {
                this["trimTrailingWhitespace"] = value;
            }
        }

        /// <summary>
        /// Insert a newline character at the end of the file if one does not exist.
        ///
        /// @since 3.15.0
        /// </summary>
        [JsonIgnore]
        public bool InsertFinalNewline
        {
            get
            {
                return this["insertFinalNewline"].IsBool ? this["insertFinalNewline"].Bool : false;
            }
            set
            {
                this["insertFinalNewline"] = value;
            }
        }

        /// <summary>
        /// Trim all newlines after the final newline at the end of the file.
        ///
        /// @since 3.15.0
        /// </summary>
        [JsonIgnore]
        public bool TrimFinalNewlines
        {
            get
            {
                return this["trimFinalNewlines"].IsBool ? this["trimFinalNewlines"].Bool : false;
            }
            set
            {
                this["trimFinalNewlines"] = value;
            }
        }
    }
}
