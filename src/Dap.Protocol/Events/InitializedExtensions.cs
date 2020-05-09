namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public static class InitializedExtensions
    {
        public static void SendInitialized(this IDebugClient mediator, InitializedEvent @event)
        {
            mediator.SendNotification(EventNames.Initialized, @event);
        }
    }
}
