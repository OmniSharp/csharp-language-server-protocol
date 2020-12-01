using Microsoft.Extensions.Logging;

namespace OmniSharp.Extensions.JsonRpc
{
    public static class Events
    {
        public static EventId UnhandledException { get; } = new EventId(1337_100);
        public static EventId UnhandledRequest { get; } = new EventId(1337_101);
        public static EventId UnhandledNotification { get; } = new EventId(1337_102);
    }
}
