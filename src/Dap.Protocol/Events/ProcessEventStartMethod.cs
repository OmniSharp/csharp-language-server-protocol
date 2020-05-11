using System.Text.Json.Serialization;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ProcessEventStartMethod
    {
        Launch, Attach, AttachForSuspendedLaunch
    }

}
