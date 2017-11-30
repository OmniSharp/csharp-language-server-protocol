using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// Value-object describing what options formatting should use.
    /// </summary>
    [JsonDictionary(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
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
    }
}