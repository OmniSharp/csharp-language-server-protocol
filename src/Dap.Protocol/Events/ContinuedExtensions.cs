namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public static class ContinuedExtensions
    {
        public static void SendContinued(this IDebugClient mediator, ContinuedEvent @event)
        {
            mediator.SendNotification(EventNames.Continued, @event);
        }
    }
}
