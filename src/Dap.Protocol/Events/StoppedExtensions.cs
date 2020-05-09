namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public static class StoppedExtensions
    {
        public static void SendStopped(this IDebugClient mediator, StoppedEvent @event)
        {
            mediator.SendNotification(EventNames.Stopped, @event);
        }
    }
}
