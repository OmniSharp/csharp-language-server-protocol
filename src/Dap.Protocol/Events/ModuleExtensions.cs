namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public static class ModuleExtensions
    {
        public static void SendModule(this IDebugClient mediator, ModuleEvent @event)
        {
            mediator.SendNotification(EventNames.Module, @event);
        }
    }
}
