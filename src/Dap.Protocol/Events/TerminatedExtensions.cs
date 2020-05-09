namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public static class TerminatedExtensions
    {
        public static void SendTerminated(this IDebugClient mediator, TerminatedEvent @event)
        {
            mediator.SendNotification(EventNames.Terminated, @event);
        }
    }
}
