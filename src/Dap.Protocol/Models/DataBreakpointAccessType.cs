using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Models
{
    /// <summary>
    /// This enumeration defines all possible access types for data breakpoints.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum DataBreakpointAccessType
    {
        Read,
        Write,
        ReadWrite
    }
}
