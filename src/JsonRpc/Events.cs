using Microsoft.Extensions.Logging;

namespace OmniSharp.Extensions.JsonRpc
{
    public static class Events
    {
        public static EventId UnhandledException = new EventId(1337_100);
        public static EventId UnhandledRequest = new EventId(1337_101);
        public static EventId UnhandledNotification = new EventId(1337_102);
    }
}