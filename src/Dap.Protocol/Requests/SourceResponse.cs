using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public class SourceResponse
    {
        /// <summary>
        /// Content of the source reference.
        /// </summary>
        public string content { get; set; }

        /// <summary>
        /// Optional content type(mime type) of the source.
        /// </summary>
        [Optional] public string mimeType { get; set; }
    }

}
