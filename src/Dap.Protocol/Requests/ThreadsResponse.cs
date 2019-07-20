namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public class ThreadsResponse
    {
        /// <summary>
        /// All threads.
        /// </summary>
        public Container<Thread> Threads { get; set; }
    }

}
