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
        /// <remarks>
        /// TODO: UPDATE THIS next version
        /// <see cref="uint"/> in the LSP spec
        /// </remarks>
        [JsonIgnore]
        public long TabSize
        {
            get => TryGetValue("tabSize", out var tabSize) && tabSize.IsLong ? tabSize.Long : -1;
            set => this["tabSize"] = value;
        }

        /// <summary>
        /// Prefer spaces over tabs.
        /// </summary>
        [JsonIgnore]
        public bool InsertSpaces
        {
            get => TryGetValue("insertSpaces", out var insertSpaces) && insertSpaces.IsBool && insertSpaces.Bool;
            set => this["insertSpaces"] = value;
        }

        /// <summary>
        /// Trim trailing whitespaces on a line.
        ///
        /// @since 3.15.0
        /// </summary>
        [JsonIgnore]
        public bool TrimTrailingWhitespace
        {
            get => TryGetValue("trimTrailingWhitespace", out var trimTrailingWhitespace) && trimTrailingWhitespace.IsBool && trimTrailingWhitespace.Bool;
            set => this["trimTrailingWhitespace"] = value;
        }

        /// <summary>
        /// Insert a newline character at the end of the file if one does not exist.
        ///
        /// @since 3.15.0
        /// </summary>
        [JsonIgnore]
        public bool InsertFinalNewline
        {
            get => TryGetValue("insertFinalNewline", out var insertFinalNewline) && insertFinalNewline.IsBool && insertFinalNewline.Bool;
            set => this["insertFinalNewline"] = value;
        }

        /// <summary>
        /// Trim all newlines after the final newline at the end of the file.
        ///
        /// @since 3.15.0
        /// </summary>
        [JsonIgnore]
        public bool TrimFinalNewlines
        {
            get => TryGetValue("trimFinalNewlines", out var trimFinalNewlines) && trimFinalNewlines.IsBool && trimFinalNewlines.Bool;
            set => this["trimFinalNewlines"] = value;
        }
    }
}
