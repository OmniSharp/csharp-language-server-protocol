namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public static class ProcessExtensions
    {
        public static void SendProcess(this IDebugClient mediator, ProcessEvent @event)
        {
            mediator.SendNotification(EventNames.Process, @event);
        }
    }
}
