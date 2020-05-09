namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ProcessEventStartMethod
    {
        Launch, Attach, AttachForSuspendedLaunch
    }

}
