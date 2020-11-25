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
        [Method(RequestNames.GotoTargets, Direction.ClientToServer)]
        [
            GenerateHandler,
            GenerateHandlerMethods,
            GenerateRequestMethods
        ]
        public class GotoTargetsArguments : IRequest<GotoTargetsResponse>
        {
            /// <summary>
            /// The source location for which the goto targets are determined.
            /// </summary>
            public Source Source { get; set; } = null!;

            /// <summary>
            /// The line location for which the goto targets are determined.
            /// </summary>
            public long Line { get; set; }

            /// <summary>
            /// An optional column location for which the goto targets are determined.
            /// </summary>
            [Optional]
            public long? Column { get; set; }
        }

        public class GotoTargetsResponse
        {
            /// <summary>
            /// The possible goto targets of the specified location.
            /// </summary>
            public Container<GotoTarget> Targets { get; set; } = null!;
        }
    }

    namespace Models
    {
        /// <summary>
        /// A GotoTarget describes a code location that can be used as a target in the ‘goto’ request.
        /// The possible goto targets can be determined via the ‘gotoTargets’ request.
        /// </summary>
        public class GotoTarget
        {
            /// <summary>
            /// Unique identifier for a goto target. This is used in the goto request.
            /// </summary>
            public long Id { get; set; }

            /// <summary>
            /// The name of the goto target (shown in the UI).
            /// </summary>
            public string Label { get; set; } = null!;

            /// <summary>
            /// The line of the goto target.
            /// </summary>
            public int Line { get; set; }

            /// <summary>
            /// An optional column of the goto target.
            /// </summary>
            [Optional]
            public int? Column { get; set; }

            /// <summary>
            /// An optional end line of the range covered by the goto target.
            /// </summary>
            [Optional]
            public int? EndLine { get; set; }

            /// <summary>
            /// An optional end column of the range covered by the goto target.
            /// </summary>
            [Optional]
            public int? EndColumn { get; set; }

            /// <summary>
            /// Optional memory reference for the instruction pointer value represented by this target.
            /// </summary>
            [Optional]
            public string? InstructionPointerReference { get; set; }
        }
    }
}
