using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
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
        [Method(RequestNames.SetExceptionBreakpoints, Direction.ClientToServer)]
        [GenerateHandler]
        [GenerateHandlerMethods]
        [GenerateRequestMethods]
        public record SetExceptionBreakpointsArguments : IRequest<SetExceptionBreakpointsResponse>
        {
            /// <summary>
            /// IDs of checked exception options.The set of IDs is returned via the 'exceptionBreakpointFilters' capability.
            /// </summary>
            public Container<string> Filters { get; init; } = null!;

            /// <summary>
            /// Set of exception filters and their options. The set of all possible
            /// exception filters is defined by the 'exceptionBreakpointFilters'
            /// capability. This attribute is only honored by a debug adapter if the
            /// capability 'supportsExceptionFilterOptions' is true. The 'filter' and
            /// 'filterOptions' sets are additive.
            /// </summary>
            [Optional]
            public Container<ExceptionFilterOptions>? FilterOptions { get; init; }

            /// <summary>
            /// Configuration options for selected exceptions.
            /// </summary>
            [Optional]
            public Container<ExceptionOptions>? ExceptionOptions { get; init; }
        }

        public record SetExceptionBreakpointsResponse;
    }

    namespace Models
    {
        /// <summary>
        /// ExceptionOptions
        /// An ExceptionOptions assigns configuration options to a set of exceptions.
        /// </summary>
        public record ExceptionOptions
        {
            /// <summary>
            /// A path that selects a single or multiple exceptions in a tree. If 'path' is missing, the whole tree is selected. By convention the first segment of the path is
            /// a category that is
            /// used to group exceptions in the UI.
            /// </summary>
            [Optional]
            public Container<ExceptionPathSegment>? Path { get; init; }

            /// <summary>
            /// Condition when a thrown exception should result in a break.
            /// </summary>
            public ExceptionBreakMode BreakMode { get; init; }
        }

        /// <summary>
        /// An ExceptionPathSegment represents a segment in a path that is used to match leafs or nodes in a tree of exceptions.If a segment consists of more than one
        /// name, it matches the
        /// names provided if ‘negate’ is false or missing or it matches anything except the names provided if ‘negate’ is true.
        /// </summary>
        public record ExceptionPathSegment
        {
            /// <summary>
            /// If false or missing this segment matches the names provided, otherwise it matches anything except the names provided.
            /// </summary>
            [Optional]
            public bool Negate { get; init; }

            /// <summary>
            /// Depending on the value of 'negate' the names that should match or not match.
            /// </summary>
            public Container<string> Names { get; init; } = null!;
        }

        /// <summary>
        /// This enumeration defines all possible conditions when a thrown exception should result in a break.
        /// never: never breaks,
        /// always: always breaks,
        /// unhandled: breaks when exception unhandled,
        /// userUnhandled: breaks if the exception is not handled by user code.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enum ExceptionBreakMode
        {
            Never, Always, Unhandled, UserUnhandled
        }
    }
}
