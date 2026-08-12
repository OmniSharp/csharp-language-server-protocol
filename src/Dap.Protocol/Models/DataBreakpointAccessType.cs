using System.Text.Json.Serialization;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Models
{
    /// <summary>
    /// This enumeration defines all possible access types for data breakpoints.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum DataBreakpointAccessType
    {
        Read,
        Write,
        ReadWrite
    }
}
