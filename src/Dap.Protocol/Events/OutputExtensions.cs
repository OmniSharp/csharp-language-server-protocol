namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public static class OutputExtensions
    {
        public static void SendOutput(this IDebugClient mediator, OutputEvent @event)
        {
            mediator.SendNotification(EventNames.Output, @event);
        }
    }
}
