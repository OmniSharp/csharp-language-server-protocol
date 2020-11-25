using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.DebugAdapter.Protocol
{
    namespace Requests
    {
        [Parallel]
        [Method(RequestNames.Source, Direction.ClientToServer)]
        [
            GenerateHandler,
            GenerateHandlerMethods,
            GenerateRequestMethods
        ]
        public class SourceArguments : IRequest<SourceResponse>
        {
            /// <summary>
            /// Specifies the source content to load.Either source.path or source.sourceReference must be specified.
            /// </summary>
            [Optional]
            public Source? Source { get; set; }

            /// <summary>
            /// The reference to the source.This is the same as source.sourceReference.This is provided for backward compatibility since old backends do not understand the 'source' attribute.
            /// </summary>
            public long SourceReference { get; set; }
        }

        public class SourceResponse
        {
            /// <summary>
            /// Content of the source reference.
            /// </summary>
            public string Content { get; set; } = null!;

            /// <summary>
            /// Optional content type(mime type) of the source.
            /// </summary>
            [Optional]
            public string? MimeType { get; set; }
        }
    }

    namespace Models
    {
    }
}
