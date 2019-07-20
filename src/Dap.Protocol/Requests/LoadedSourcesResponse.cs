namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public class LoadedSourcesResponse
    {
        /// <summary>
        /// Set of loaded sources.
        /// </summary>
        public Container<Source> Sources { get; set; }
    }

}
