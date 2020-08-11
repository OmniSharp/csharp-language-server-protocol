using OmniSharp.Extensions.DebugAdapter.Protocol.Models;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    public class ThreadsResponse
    {
        /// <summary>
        /// All threads.
        /// </summary>
        public Container<Thread> Threads { get; set; }
    }
}
