namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public static class EventNames
    {
        public const string Initialized = "initialized";
        public const string Stopped = "stopped";
        public const string Continued = "continued";
        public const string Exited = "exited";
        public const string Terminated = "terminated";
        public const string Thread = "thread";
        public const string Output = "output";
        public const string Breakpoint = "breakpoint";
        public const string Module = "module";
        public const string LoadedSource = "loadedSource";
        public const string Process = "process";
        public const string Capabilities = "capabilities";
    }
}
