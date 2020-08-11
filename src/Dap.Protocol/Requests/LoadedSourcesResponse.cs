using OmniSharp.Extensions.DebugAdapter.Protocol.Models;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    public class LoadedSourcesResponse
    {
        /// <summary>
        /// Set of loaded sources.
        /// </summary>
        public Container<Source> Sources { get; set; }
    }
}
