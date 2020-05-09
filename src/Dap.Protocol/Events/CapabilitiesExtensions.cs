namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public static class CapabilitiesExtensions
    {
        public static void SendCapabilities(this IDebugClient mediator, CapabilitiesEvent @event)
        {
            mediator.SendNotification(EventNames.Capabilities, @event);
        }
    }
}
