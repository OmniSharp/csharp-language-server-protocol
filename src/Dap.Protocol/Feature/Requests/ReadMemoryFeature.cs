using MediatR;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.DebugAdapter.Protocol
{
    namespace Requests
    {
        [Parallel]
        [Method(RequestNames.ReadMemory, Direction.ClientToServer)]
        [
            GenerateHandler,
            GenerateHandlerMethods,
            GenerateRequestMethods
        ]
        public record ReadMemoryArguments : IRequest<ReadMemoryResponse>
        {
            /// <summary>
            /// Memory reference to the base location from which data should be read.
            /// </summary>
            public string MemoryReference { get; init; }

            /// <summary>
            /// Optional offset(in bytes) to be applied to the reference location before reading data.Can be negative.
            /// </summary>

            [Optional]
            public long? Offset { get; init; }

            /// <summary>
            /// Number of bytes to read at the specified location and offset.
            /// </summary>
            public long Count { get; init; }
        }

        public record ReadMemoryResponse
        {
            /// <summary>
            /// The address of the first byte of data returned.Treated as a hex value if prefixed with '0x', or as a decimal value otherwise.
            /// </summary>
            public string Address { get; init; }

            /// <summary>
            /// The number of unreadable bytes encountered after the last successfully read byte. This can be used to determine the number of bytes that must be skipped before a subsequent
            /// 'readMemory' request will succeed.
            /// </summary>
            [Optional]
            public long? UnreadableBytes { get; init; }

            /// <summary>
            /// The bytes read from memory, encoded using base64.
            /// </summary>
            [Optional]
            public string? Data { get; init; }
        }
    }
}
