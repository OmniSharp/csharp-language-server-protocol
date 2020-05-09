namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public static class ExitedExtensions
    {
        public static void SendExited(this IDebugClient mediator, ExitedEvent @event)
        {
            mediator.SendNotification(EventNames.Exited, @event);
        }
    }
}
