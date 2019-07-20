namespace OmniSharp.Extensions.DebugAdapter.Protocol
{
    /// <summary>
    /// A Thread
    /// </summary>
    public class Thread
    {
        /// <summary>
        /// Unique identifier for the thread.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// A name of the thread.
        /// </summary>
        public string Name { get; set; }
    }
}