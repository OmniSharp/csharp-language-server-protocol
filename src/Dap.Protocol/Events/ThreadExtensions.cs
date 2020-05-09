namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public static class ThreadExtensions
    {
        public static void SendThread(this IDebugClient mediator, ThreadEvent @event)
        {
            mediator.SendNotification(EventNames.Thread, @event);
        }
    }
}
