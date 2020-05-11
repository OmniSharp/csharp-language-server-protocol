using System.Text.Json.Serialization;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Models
{
    /// <summary>
    /// Information about the capabilities of a debug adapter.
    /// </summary>
    public class Capabilities
    {
        /// <summary>
        /// The debug adapter supports the 'configurationDone' request.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public bool? SupportsConfigurationDoneRequest { get; set; }

        /// <summary>
        /// The debug adapter supports function breakpoints.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public bool? SupportsFunctionBreakpoints { get; set; }

        /// <summary>
        /// The debug adapter supports conditional breakpoints.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public bool? SupportsConditionalBreakpoints { get; set; }

        /// <summary>
        /// The debug adapter supports breakpoints that break execution after a specified long of hits.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public bool? SupportsHitConditionalBreakpoints { get; set; }

        /// <summary>
        /// The debug adapter supports a (side effect free) evaluate request for data hovers.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public bool? SupportsEvaluateForHovers { get; set; }

        /// <summary>
        /// Available filters or options for the setExceptionBreakpoints request.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public Container<ExceptionBreakpointsFilter> ExceptionBreakpointFilters { get; set; }

        /// <summary>
        /// The debug adapter supports stepping back via the 'stepBack' and 'reverseContinue' requests.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public bool? SupportsStepBack { get; set; }

        /// <summary>
        /// The debug adapter supports setting a variable to a value.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public bool? SupportsSetVariable { get; set; }

        /// <summary>
        /// The debug adapter supports restarting a frame.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public bool? SupportsRestartFrame { get; set; }

        /// <summary>
        /// The debug adapter supports the 'gotoTargets' request.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public bool? SupportsGotoTargetsRequest { get; set; }

        /// <summary>
        /// The debug adapter supports the 'stepInTargets' request.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public bool? SupportsStepInTargetsRequest { get; set; }

        /// <summary>
        /// The debug adapter supports the 'completions' request.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public bool? SupportsCompletionsRequest { get; set; }

        /// <summary>
        /// The debug adapter supports the 'modules' request.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public bool? SupportsModulesRequest { get; set; }

        /// <summary>
        /// The set of additional module information exposed by the debug adapter.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public Container<ColumnDescriptor> AdditionalModuleColumns { get; set; }

        /// <summary>
        /// Checksum algorithms supported by the debug adapter.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public Container<ChecksumAlgorithm> SupportedChecksumAlgorithms { get; set; }

        /// <summary>
        /// The debug adapter supports the 'restart' request. In this case a client should not implement 'restart' by terminating and relaunching the adapter but by calling the RestartRequest.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public bool? SupportsRestartRequest { get; set; }

        /// <summary>
        /// The debug adapter supports 'exceptionOptions' on the setExceptionBreakpoints request.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public bool? SupportsExceptionOptions { get; set; }

        /// <summary>
        /// The debug adapter supports a 'format' attribute on the stackTraceRequest, variablesRequest, and evaluateRequest.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public bool? SupportsValueFormattingOptions { get; set; }

        /// <summary>
        /// The debug adapter supports the 'exceptionInfo' request.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public bool? SupportsExceptionInfoRequest { get; set; }

        /// <summary>
        /// The debug adapter supports the 'terminateDebuggee' attribute on the 'disconnect' request.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public bool? SupportTerminateDebuggee { get; set; }

        /// <summary>
        /// The debug adapter supports the delayed loading of parts of the stack, which requires that both the 'startFrame' and 'levels' arguments and the 'totalFrames' result of the 'StackTrace' request are supported.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public bool? SupportsDelayedStackTraceLoading { get; set; }

        /// <summary>
        /// The debug adapter supports the 'loadedSources' request.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public bool? SupportsLoadedSourcesRequest { get; set; }

        /// <summary>
        /// The debug adapter supports logpoints by interpreting the 'logMessage' attribute of the SourceBreakpoint.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public bool? SupportsLogPoints { get; set; }

        /// <summary>
        /// The debug adapter supports the 'terminateThreads' request.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public bool? SupportsTerminateThreadsRequest { get; set; }

        /// <summary>
        /// The debug adapter supports the 'setExpression' request.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public bool? SupportsSetExpression { get; set; }

        /// <summary>
        /// The debug adapter supports the 'terminate' request.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public bool? SupportsTerminateRequest { get; set; }

        /// <summary>
        /// The debug adapter supports data breakpoints.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public bool? SupportsDataBreakpoints { get; set; }

        /// <summary>
        /// The debug adapter supports the 'readMemory' request.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public bool? SupportsReadMemoryRequest { get; set; }

        /// <summary>
        /// The debug adapter supports the 'disassemble' request.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public bool? SupportsDisassembleRequest { get; set; }
    }
}
