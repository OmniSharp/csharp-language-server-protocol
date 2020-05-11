using System.Text.Json.Serialization;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using MediatR;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    public class ReadMemoryArguments : IRequest<ReadMemoryResponse>
    {
        /// <summary>
        /// Memory reference to the base location from which data should be read.
        /// </summary>
        public string MemoryReference { get; set; }

        /// <summary>
        /// Optional offset(in bytes) to be applied to the reference location before reading data.Can be negative.
        /// </summary>

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public long? Offset { get; set; }

        /// <summary>
        /// Number of bytes to read at the specified location and offset.
        /// </summary>
        public long Count { get; set; }
    }

}
