namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public static class LoadedSourceExtensions
    {
        public static void SendLoadedSource(this IDebugClient mediator, LoadedSourceEvent @event)
        {
            mediator.SendNotification(EventNames.LoadedSource, @event);
        }
    }
}
