using System.Text.Json.Serialization;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    public class SourceResponse
    {
        /// <summary>
        /// Content of the source reference.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Optional content type(mime type) of the source.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public string MimeType { get; set; }
    }

}
