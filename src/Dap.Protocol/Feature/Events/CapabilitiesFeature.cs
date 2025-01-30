using MediatR;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.DebugAdapter.Protocol
{
    namespace Models
    {
        /// <summary>
        /// Information about the capabilities of a debug adapter.
        /// </summary>
        public record Capabilities
        {
            /// <summary>
            /// The debug adapter supports the 'configurationDone' request.
            /// </summary>
            [Optional]
            public bool SupportsConfigurationDoneRequest { get; set; }

            /// <summary>
            /// The debug adapter supports function breakpoints.
            /// </summary>
            [Optional]
            public bool SupportsFunctionBreakpoints { get; set; }

            /// <summary>
            /// The debug adapter supports conditional breakpoints.
            /// </summary>
            [Optional]
            public bool SupportsConditionalBreakpoints { get; set; }

            /// <summary>
            /// The debug adapter supports breakpoints that break execution after a specified long of hits.
            /// </summary>
            [Optional]
            public bool SupportsHitConditionalBreakpoints { get; set; }

            /// <summary>
            /// The debug adapter supports a (side effect free) evaluate request for data hovers.
            /// </summary>
            [Optional]
            public bool SupportsEvaluateForHovers { get; set; }

            /// <summary>
            /// Available filters or options for the setExceptionBreakpoints request.
            /// </summary>
            [Optional]
            public Container<ExceptionBreakpointsFilter>? ExceptionBreakpointFilters { get; set; }

            /// <summary>
            /// The debug adapter supports stepping back via the 'stepBack' and 'reverseContinue' requests.
            /// </summary>
            [Optional]
            public bool SupportsStepBack { get; set; }

            /// <summary>
            /// The debug adapter supports setting a variable to a value.
            /// </summary>
            [Optional]
            public bool SupportsSetVariable { get; set; }

            /// <summary>
            /// The debug adapter supports restarting a frame.
            /// </summary>
            [Optional]
            public bool SupportsRestartFrame { get; set; }

            /// <summary>
            /// The debug adapter supports the 'gotoTargets' request.
            /// </summary>
            [Optional]
            public bool SupportsGotoTargetsRequest { get; set; }

            /// <summary>
            /// The debug adapter supports the 'stepInTargets' request.
            /// </summary>
            [Optional]
            public bool SupportsStepInTargetsRequest { get; set; }

            /// <summary>
            /// The debug adapter supports the 'completions' request.
            /// </summary>
            [Optional]
            public bool SupportsCompletionsRequest { get; set; }

            /// <summary>
            /// The debug adapter supports the 'modules' request.
            /// </summary>
            [Optional]
            public bool SupportsModulesRequest { get; set; }

            /// <summary>
            /// The set of additional module information exposed by the debug adapter.
            /// </summary>
            [Optional]
            public Container<ColumnDescriptor>? AdditionalModuleColumns { get; set; }

            /// <summary>
            /// Checksum algorithms supported by the debug adapter.
            /// </summary>
            [Optional]
            public Container<ChecksumAlgorithm>? SupportedChecksumAlgorithms { get; set; }

            /// <summary>
            /// The debug adapter supports the 'restart' request. In this case a client should not implement 'restart' by terminating and relaunching the adapter but by
            /// calling the
            /// RestartRequest.
            /// </summary>
            [Optional]
            public bool SupportsRestartRequest { get; set; }

            /// <summary>
            /// The debug adapter supports 'exceptionOptions' on the setExceptionBreakpoints request.
            /// </summary>
            [Optional]
            public bool SupportsExceptionOptions { get; set; }

            /// <summary>
            /// The debug adapter supports a 'format' attribute on the stackTraceRequest, variablesRequest, and evaluateRequest.
            /// </summary>
            [Optional]
            public bool SupportsValueFormattingOptions { get; set; }

            /// <summary>
            /// The debug adapter supports the 'exceptionInfo' request.
            /// </summary>
            [Optional]
            public bool SupportsExceptionInfoRequest { get; set; }

            /// <summary>
            /// The debug adapter supports the 'terminateDebuggee' attribute on the 'disconnect' request.
            /// </summary>
            [Optional]
            public bool SupportTerminateDebuggee { get; set; }

            /// <summary>
            /// The debug adapter supports the delayed loading of parts of the stack, which requires that both the 'startFrame' and 'levels' arguments and the 'totalFrames'
            /// result of the
            /// 'StackTrace' request are supported.
            /// </summary>
            [Optional]
            public bool SupportsDelayedStackTraceLoading { get; set; }

            /// <summary>
            /// The debug adapter supports the 'loadedSources' request.
            /// </summary>
            [Optional]
            public bool SupportsLoadedSourcesRequest { get; set; }

            /// <summary>
            /// The debug adapter supports logpoints by interpreting the 'logMessage' attribute of the SourceBreakpoint.
            /// </summary>
            [Optional]
            public bool SupportsLogPoints { get; set; }

            /// <summary>
            /// The debug adapter supports the 'terminateThreads' request.
            /// </summary>
            [Optional]
            public bool SupportsTerminateThreadsRequest { get; set; }

            /// <summary>
            /// The debug adapter supports the 'setExpression' request.
            /// </summary>
            [Optional]
            public bool SupportsSetExpression { get; set; }

            /// <summary>
            /// The debug adapter supports the 'terminate' request.
            /// </summary>
            [Optional]
            public bool SupportsTerminateRequest { get; set; }

            /// <summary>
            /// The debug adapter supports data breakpoints.
            /// </summary>
            [Optional]
            public bool SupportsDataBreakpoints { get; set; }

            /// <summary>
            /// The debug adapter supports the 'readMemory' request.
            /// </summary>
            [Optional]
            public bool SupportsReadMemoryRequest { get; set; }

            /// <summary>
            /// The debug adapter supports the 'disassemble' request.
            /// </summary>
            [Optional]
            public bool SupportsDisassembleRequest { get; set; }

            /// <summary>
            /// The debug adapter supports the 'cancel' request.
            /// </summary>
            [Optional]
            public bool SupportsCancelRequest { get; set; }

            /// <summary>
            /// The debug adapter supports the 'breakpointLocations' request.
            /// </summary>
            [Optional]
            public bool SupportsBreakpointLocationsRequest { get; set; }

            /// <summary>
            /// The debug adapter supports the 'clipboard' context value in the 'evaluate' request.
            /// </summary>
            [Optional]
            public bool SupportsClipboardContext { get; set; }

            /// <summary>
            /// The debug adapter supports stepping granularities (argument 'granularity') for the stepping requests.
            /// </summary>
            [Optional]
            public bool SupportsSteppingGranularity { get; set; }

            /// <summary>
            /// The debug adapter supports adding breakpoints based on instruction references.
            /// </summary>
            [Optional]
            public bool SupportsInstructionBreakpoints { get; set; }

            /// <summary>
            /// The debug adapter supports 'filterOptions' as an argument on the
            /// 'setExceptionBreakpoints' request.
            /// </summary>
            [Optional]
            public bool SupportsExceptionFilterOptions { get; set; }
        }
    }

    namespace Events
    {
        [Parallel]
        [Method(EventNames.Capabilities, Direction.ServerToClient)]
        [GenerateHandler]
        [GenerateHandlerMethods]
        [GenerateRequestMethods]
        public record CapabilitiesEvent : IRequest<Unit>
        {
            /// <summary>
            /// The set of updated capabilities.
            /// </summary>
            public Capabilities Capabilities { get; init; } = null!;
        }
    }
}
