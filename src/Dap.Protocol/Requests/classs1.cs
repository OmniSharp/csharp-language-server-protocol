using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using OmniSharp.Extensions.Embedded.MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public static class RequestNames
    {
        public static string Initialize = "initialize";
        public static string ConfigurationDone = "configurationDone";
        public static string Launch = "launch";
        public static string Attach = "attach";
        public static string Restart = "restart";
        public static string Disconnect = "disconnect";
        public static string Terminate = "terminate";
        public static string SetBreakpoints = "setBreakpoints";
        public static string setFunctionBreakpoints = "setFunctionBreakpoints";
        public static string SetExceptionBreakpoints = "setExceptionBreakpoints";
        public static string DataBreakpointInfo = "dataBreakpointInfo";
        public static string SetDataBreakpoints = "setDataBreakpoints";
        public static string Continue = "continue";
        public static string Next = "next";
        public static string StepIn = "stepIn";
        public static string StepOut = "stepOut";
        public static string StepBack = "stepBack";
        public static string ReverseContinue = "reverseContinue";
        public static string RestartFrame = "restartFrame";
        public static string Goto = "goto";
        public static string Pause = "pause";
        public static string StackTrace = "stackTrace";
        public static string Scopes = "scopes";
        public static string Variables = "variables";
        public static string SetVariable = "setVariable";
        public static string Source = "source";

        public static string Threads = "threads";
        public static string TerminateThreads = "terminateThreads";
        public static string Modules = "modules";
        public static string LoadedSources = "loadedSources";
        public static string Evaluate = "evaluate";
        public static string SetExpression = "setExpression";
        public static string StepInTargets = "stepInTargets";
        public static string GotoTargets = "gotoTargets";
        public static string Completions = "completions";
        public static string ExceptionInfo = "exceptionInfo";
        public static string ReadMemory = "readMemory";
        public static string Disassemble = "disassemble";
    }


    [Parallel, Method(RequestNames.Initialize)]
    public interface IInitializeHandler : IJsonRpcNotificationHandler<InitializeRequestArguments> { }

    public abstract class InitializeHandler : IInitializeHandler
    {
        public abstract Task<Unit> Handle(InitializeRequestArguments request, CancellationToken cancellationToken);
    }

    public static class InitializeHandlerExtensions
    {
        public static IDisposable OnPublishDiagnostics(this IDebugAdapterRegistry registry, Func<InitializeRequestArguments, Task<Unit>> handler)
        {
            return registry.AddHandlers(new DelegatingHandler(handler));
        }

        class DelegatingHandler : InitializeHandler
        {
            private readonly Func<InitializeRequestArguments, Task<Unit>> _handler;

            public DelegatingHandler(Func<InitializeRequestArguments, Task<Unit>> handler)
            {
                _handler = handler;
            }

            public override Task<Unit> Handle(InitializeRequestArguments request, CancellationToken cancellationToken) => _handler.Invoke(request);
        }
    }

    Requests
:leftwards_arrow_with_hook: Initialize Request
The ‘initialize’ request is sent as the first request from the client to the debug adapter in order to configure it with client capabilities and to retrieve capabilities from the debug adapter.
Until the debug adapter has responded to with an ‘initialize’ response, the client must not send any additional requests or events to the debug adapter.In addition the debug adapter is not allowed to send any requests or events to the client until it has responded with an ‘initialize’ response.
The ‘initialize’ request may only be sent once.

interface InitializeRequest extends Request
    {
public static string initialize = "initialize";

    arguments: InitializeRequestArguments;
}
Arguments for ‘initialize’ request.


interface InitializeRequestArguments
{
    /**
    * The ID of the (frontend) client using this adapter.
*/
    clientID?: string;

/**
* The human readable name of the (frontend) client using this adapter.
*/
clientName?: string;

/**
* The ID of the debug adapter.
*/
adapterID: string;

/**
* The ISO-639 locale of the (frontend) client using this adapter, e.g. en-US or de-CH.
*/
locale?: string;

/**
* If true all line numbers are 1-based (default).
*/
linesStartAt1?: boolean;

/**
* If true all column numbers are 1-based (default).
*/
columnsStartAt1?: boolean;

/**
* Determines in what format paths are specified. The default is 'path', which is the native format.
* Values: 'path', 'uri', etc.
*/
pathFormat?: string;

/**
* Client supports the optional type attribute for variables.
*/
supportsVariableType?: boolean;

/**
* Client supports the paging of variables.
*/
supportsVariablePaging?: boolean;

/**
* Client supports the runInTerminal request.
*/
supportsRunInTerminalRequest?: boolean;

/**
* Client supports memory references.
*/
supportsMemoryReferences?: boolean;
}
Response to ‘initialize’ request.


interface InitializeResponse extends Response
{
    /**
    * The capabilities of this debug adapter.
*/
    body?: Capabilities;
}
:leftwards_arrow_with_hook: ConfigurationDone Request
The client of the debug protocol must send this request at the end of the sequence of configuration requests (which was started by the ‘initialized’ event).

interface ConfigurationDoneRequest extends Request
{

    arguments?: ConfigurationDoneArguments;
}
Arguments for ‘configurationDone’ request.


interface ConfigurationDoneArguments
{
}
Response to ‘configurationDone’ request.This is just an acknowledgement, so no body field is required.


interface ConfigurationDoneResponse extends Response
{
}
:leftwards_arrow_with_hook: Launch Request
The launch request is sent from the client to the debug adapter to start the debuggee with or without debugging(if ‘noDebug’ is true). Since launching is debugger/runtime specific, the arguments for this request are not part of this specification.

interface LaunchRequest extends Request
{

    arguments: LaunchRequestArguments;
}
Arguments for ‘launch’ request.Additional attributes are implementation specific.


interface LaunchRequestArguments
{
    /**
    * If noDebug is true the launch request should launch the program without enabling debugging.
*/
    noDebug?: boolean;

/**
* Optional data from the previous, restarted session.
* The data is sent as the 'restart' attribute of the 'terminated' event.
* The client should leave the data intact.
*/
__restart?: any;
}
Response to ‘launch’ request.This is just an acknowledgement, so no body field is required.


interface LaunchResponse extends Response
{
}
:leftwards_arrow_with_hook: Attach Request
The attach request is sent from the client to the debug adapter to attach to a debuggee that is already running.Since attaching is debugger/runtime specific, the arguments for this request are not part of this specification.

interface AttachRequest extends Request
{

    arguments: AttachRequestArguments;
}
Arguments for ‘attach’ request.Additional attributes are implementation specific.


interface AttachRequestArguments
{
    /**
    * Optional data from the previous, restarted session.
    * The data is sent as the 'restart' attribute of the 'terminated' event.
    * The client should leave the data intact.
*/
    __restart?: any;
}
Response to ‘attach’ request.This is just an acknowledgement, so no body field is required.


interface AttachResponse extends Response
{
}
:leftwards_arrow_with_hook: Restart Request
Restarts a debug session.If the capability ‘supportsRestartRequest’ is missing or has the value false,

the client will implement ‘restart’ by terminating the debug adapter first and then launching it anew.

A debug adapter can override this default behaviour by implementing a restart request

and setting the capability ‘supportsRestartRequest’ to true.

interface RestartRequest extends Request
{

    arguments?: RestartArguments;
}
Arguments for ‘restart’ request.


interface RestartArguments
{
}
Response to ‘restart’ request.This is just an acknowledgement, so no body field is required.


interface RestartResponse extends Response
{
}
:leftwards_arrow_with_hook: Disconnect Request
The ‘disconnect’ request is sent from the client to the debug adapter in order to stop debugging.It asks the debug adapter to disconnect from the debuggee and to terminate the debug adapter.If the debuggee has been started with the ‘launch’ request, the ‘disconnect’ request terminates the debuggee. If the ‘attach’ request was used to connect to the debuggee, ‘disconnect’ does not terminate the debuggee.This behavior can be controlled with the ‘terminateDebuggee’ argument (if supported by the debug adapter).

interface DisconnectRequest extends Request
{

    arguments?: DisconnectArguments;
}
Arguments for ‘disconnect’ request.


interface DisconnectArguments
{
    /**
    * A value of true indicates that this 'disconnect' request is part of a restart sequence.
*/
    restart?: boolean;

/**
* Indicates whether the debuggee should be terminated when the debugger is disconnected.
* If unspecified, the debug adapter is free to do whatever it thinks is best.
* A client can only rely on this attribute being properly honored if a debug adapter returns true for the 'supportTerminateDebuggee' capability.
*/
terminateDebuggee?: boolean;
}
Response to ‘disconnect’ request.This is just an acknowledgement, so no body field is required.


interface DisconnectResponse extends Response
{
}
:leftwards_arrow_with_hook: Terminate Request
The ‘terminate’ request is sent from the client to the debug adapter in order to give the debuggee a chance for terminating itself.

interface TerminateRequest extends Request
{

    arguments?: TerminateArguments;
}
Arguments for ‘terminate’ request.


interface TerminateArguments
{
    /**
    * A value of true indicates that this 'terminate' request is part of a restart sequence.
*/
    restart?: boolean;
}
Response to ‘terminate’ request.This is just an acknowledgement, so no body field is required.


interface TerminateResponse extends Response
{
}
:leftwards_arrow_with_hook: SetBreakpoints Request
Sets multiple breakpoints for a single source and clears all previous breakpoints in that source.

To clear all breakpoint for a source, specify an empty array.

When a breakpoint is hit, a ‘stopped’ event (with reason ‘breakpoint’) is generated.

interface SetBreakpointsRequest extends Request
{

    arguments: SetBreakpointsArguments;
}
Arguments for ‘setBreakpoints’ request.


interface SetBreakpointsArguments
{
    /**
    * The source location of the breakpoints; either 'source.path' or 'source.reference' must be specified.
*/
    source: Source;

/**
* The code locations of the breakpoints.
*/
breakpoints?: SourceBreakpoint[];

/**
* Deprecated: The code locations of the breakpoints.
*/
lines?: number[];

/**
* A value of true indicates that the underlying source has been modified which results in new breakpoint locations.
*/
sourceModified?: boolean;
}
Response to ‘setBreakpoints’ request.

Returned is information about each breakpoint created by this request.

This includes the actual code location and whether the breakpoint could be verified.

The breakpoints returned are in the same order as the elements of the ‘breakpoints’


(or the deprecated ‘lines’) array in the arguments.


interface SetBreakpointsResponse extends Response
{
    body: {
    /**
    * Information about the breakpoints. The array elements are in the same order as the elements of the 'breakpoints' (or the deprecated 'lines') array in the arguments.
*/
    breakpoints: Breakpoint[];
    };
}
:leftwards_arrow_with_hook: SetFunctionBreakpoints Request
Replaces all existing function breakpoints with new function breakpoints.

To clear all function breakpoints, specify an empty array.

When a function breakpoint is hit, a ‘stopped’ event (with reason ‘function breakpoint’) is generated.

interface SetFunctionBreakpointsRequest extends Request
{

    arguments: SetFunctionBreakpointsArguments;
}
Arguments for ‘setFunctionBreakpoints’ request.


interface SetFunctionBreakpointsArguments
{
    /**
    * The function names of the breakpoints.
*/
    breakpoints: FunctionBreakpoint[];
}
Response to ‘setFunctionBreakpoints’ request.

Returned is information about each breakpoint created by this request.


interface SetFunctionBreakpointsResponse extends Response
{
    body: {
    /**
    * Information about the breakpoints. The array elements correspond to the elements of the 'breakpoints' array.
*/
    breakpoints: Breakpoint[];
    };
}
:leftwards_arrow_with_hook: SetExceptionBreakpoints Request
The request configures the debuggers response to thrown exceptions.If an exception is configured to break, a ‘stopped’ event is fired (with reason ‘exception’).

interface SetExceptionBreakpointsRequest extends Request
{

    arguments: SetExceptionBreakpointsArguments;
}
Arguments for ‘setExceptionBreakpoints’ request.


interface SetExceptionBreakpointsArguments
{
    /**
    * IDs of checked exception options. The set of IDs is returned via the 'exceptionBreakpointFilters' capability.
*/
    filters: string[];

/**
* Configuration options for selected exceptions.
*/
exceptionOptions?: ExceptionOptions[];
}
Response to ‘setExceptionBreakpoints’ request.This is just an acknowledgement, so no body field is required.


interface SetExceptionBreakpointsResponse extends Response
{
}
:leftwards_arrow_with_hook: DataBreakpointInfo Request
Obtains information on a possible data breakpoint that could be set on an expression or variable.

interface DataBreakpointInfoRequest extends Request
{

    arguments: DataBreakpointInfoArguments;
}
Arguments for ‘dataBreakpointInfo’ request.


interface DataBreakpointInfoArguments
{
    /**
    * Reference to the Variable container if the data breakpoint is requested for a child of the container.
*/
    variablesReference?: number;

/**
* The name of the Variable's child to obtain data breakpoint information for. If variableReference isn’t provided, this can be an expression.
*/
name: string;
}
Response to ‘dataBreakpointInfo’ request.


interface DataBreakpointInfoResponse extends Response
{
    body: {
    /**
    * An identifier for the data on which a data breakpoint can be registered with the setDataBreakpoints request or null if no data breakpoint is available.
*/
    dataId: string | null;

    /**
    * UI string that describes on what data the breakpoint is set on or why a data breakpoint is not available.
*/
    description: string;

        /**
        * Optional attribute listing the available access types for a potential data breakpoint. A UI frontend could surface this information.
*/
        accessTypes ?: DataBreakpointAccessType[];

        /**
        * Optional attribute indicating that a potential data breakpoint could be persisted across sessions.
*/
        canPersist ?: boolean;
    };
}
:leftwards_arrow_with_hook: SetDataBreakpoints Request
Replaces all existing data breakpoints with new data breakpoints.

To clear all data breakpoints, specify an empty array.

When a data breakpoint is hit, a ‘stopped’ event (with reason ‘data breakpoint’) is generated.

interface SetDataBreakpointsRequest extends Request
{

    arguments: SetDataBreakpointsArguments;
}
Arguments for ‘setDataBreakpoints’ request.


interface SetDataBreakpointsArguments
{
    /**
    * The contents of this array replaces all existing data breakpoints. An empty array clears all data breakpoints.
*/
    breakpoints: DataBreakpoint[];
}
Response to ‘setDataBreakpoints’ request.

Returned is information about each breakpoint created by this request.


interface SetDataBreakpointsResponse extends Response
{
    body: {
    /**
    * Information about the data breakpoints. The array elements correspond to the elements of the input argument 'breakpoints' array.
*/
    breakpoints: Breakpoint[];
    };
}
:leftwards_arrow_with_hook: Continue Request
The request starts the debuggee to run again.

interface ContinueRequest extends Request
{

    arguments: ContinueArguments;
}
Arguments for ‘continue’ request.


interface ContinueArguments
{
    /**
    * Continue execution for the specified thread (if possible). If the backend cannot continue on a single thread but will continue on all threads, it should set the 'allThreadsContinued' attribute in the response to true.
*/
    threadId: number;
}
Response to ‘continue’ request.


interface ContinueResponse extends Response
{
    body: {
        /**
        * If true, the 'continue' request has ignored the specified thread and continued all threads instead. If this attribute is missing a value of 'true' is assumed for backward compatibility.
*/
        allThreadsContinued ?: boolean;
    };
}
:leftwards_arrow_with_hook: Next Request
The request starts the debuggee to run again for one step.

The debug adapter first sends the response and then a ‘stopped’ event (with reason ‘step’) after the step has completed.

interface NextRequest extends Request
{

    arguments: NextArguments;
}
Arguments for ‘next’ request.


interface NextArguments
{
    /**
    * Execute 'next' for this thread.
*/
    threadId: number;
}
Response to ‘next’ request.This is just an acknowledgement, so no body field is required.


interface NextResponse extends Response
{
}
:leftwards_arrow_with_hook: StepIn Request
The request starts the debuggee to step into a function/method if possible.

If it cannot step into a target, ‘stepIn’ behaves like ‘next’.

The debug adapter first sends the response and then a ‘stopped’ event (with reason ‘step’) after the step has completed.

If there are multiple function/method calls (or other targets) on the source line,

the optional argument ‘targetId’ can be used to control into which target the ‘stepIn’ should occur.

The list of possible targets for a given source line can be retrieved via the ‘stepInTargets’ request.

interface StepInRequest extends Request
{

    arguments: StepInArguments;
}
Arguments for ‘stepIn’ request.


interface StepInArguments {
/**
* Execute 'stepIn' for this thread.
*/
threadId: number;

/**
* Optional id of the target to step into.
*/
targetId?: number;
}
Response to ‘stepIn’ request. This is just an acknowledgement, so no body field is required.


interface StepInResponse extends Response {
}
:leftwards_arrow_with_hook: StepOut Request
The request starts the debuggee to run again for one step.

The debug adapter first sends the response and then a ‘stopped’ event (with reason ‘step’) after the step has completed.

interface StepOutRequest extends Request {

arguments: StepOutArguments;
}
Arguments for ‘stepOut’ request.


interface StepOutArguments {
/**
* Execute 'stepOut' for this thread.
*/
threadId: number;
}
Response to ‘stepOut’ request. This is just an acknowledgement, so no body field is required.


interface StepOutResponse extends Response {
}
:leftwards_arrow_with_hook: StepBack Request
The request starts the debuggee to run one step backwards.

The debug adapter first sends the response and then a ‘stopped’ event (with reason ‘step’) after the step has completed. Clients should only call this request if the capability ‘supportsStepBack’ is true.

interface StepBackRequest extends Request {

arguments: StepBackArguments;
}
Arguments for ‘stepBack’ request.


interface StepBackArguments {
/**
* Execute 'stepBack' for this thread.
*/
threadId: number;
}
Response to ‘stepBack’ request. This is just an acknowledgement, so no body field is required.


interface StepBackResponse extends Response {
}
:leftwards_arrow_with_hook: ReverseContinue Request
The request starts the debuggee to run backward. Clients should only call this request if the capability ‘supportsStepBack’ is true.

interface ReverseContinueRequest extends Request {

arguments: ReverseContinueArguments;
}
Arguments for ‘reverseContinue’ request.


interface ReverseContinueArguments {
/**
* Execute 'reverseContinue' for this thread.
*/
threadId: number;
}
Response to ‘reverseContinue’ request. This is just an acknowledgement, so no body field is required.


interface ReverseContinueResponse extends Response {
}
:leftwards_arrow_with_hook: RestartFrame Request
The request restarts execution of the specified stackframe.

The debug adapter first sends the response and then a ‘stopped’ event (with reason ‘restart’) after the restart has completed.

interface RestartFrameRequest extends Request {

arguments: RestartFrameArguments;
}
Arguments for ‘restartFrame’ request.


interface RestartFrameArguments {
/**
* Restart this stackframe.
*/
frameId: number;
}
Response to ‘restartFrame’ request. This is just an acknowledgement, so no body field is required.


interface RestartFrameResponse extends Response {
}
:leftwards_arrow_with_hook: Goto Request
The request sets the location where the debuggee will continue to run.

This makes it possible to skip the execution of code or to executed code again.

The code between the current location and the goto target is not executed but skipped.

The debug adapter first sends the response and then a ‘stopped’ event with reason ‘goto’.

interface GotoRequest extends Request {

arguments: GotoArguments;
}
Arguments for ‘goto’ request.


interface GotoArguments {
/**
* Set the goto target for this thread.
*/
threadId: number;

/**
* The location where the debuggee will continue to run.
*/
targetId: number;
}
Response to ‘goto’ request. This is just an acknowledgement, so no body field is required.


interface GotoResponse extends Response {
}
:leftwards_arrow_with_hook: Pause Request
The request suspenses the debuggee.

The debug adapter first sends the response and then a ‘stopped’ event (with reason ‘pause’) after the thread has been paused successfully.

interface PauseRequest extends Request {

arguments: PauseArguments;
}
Arguments for ‘pause’ request.


interface PauseArguments {
/**
* Pause execution for this thread.
*/
threadId: number;
}
Response to ‘pause’ request. This is just an acknowledgement, so no body field is required.


interface PauseResponse extends Response {
}
:leftwards_arrow_with_hook: StackTrace Request
The request returns a stacktrace from the current execution state.

interface StackTraceRequest extends Request {

arguments: StackTraceArguments;
}
Arguments for ‘stackTrace’ request.


interface StackTraceArguments {
/**
* Retrieve the stacktrace for this thread.
*/
threadId: number;

/**
* The index of the first frame to return; if omitted frames start at 0.
*/
startFrame?: number;

/**
* The maximum number of frames to return. If levels is not specified or 0, all frames are returned.
*/
levels?: number;

/**
* Specifies details on how to format the stack frames.
*/
format?: StackFrameFormat;
}
Response to ‘stackTrace’ request.


interface StackTraceResponse extends Response {
body: {
/**
* The frames of the stackframe. If the array has length zero, there are no stackframes available.
* This means that there is no location information available.
*/
stackFrames: StackFrame[];

/**
* The total number of frames available.
*/
totalFrames?: number;
};
}
:leftwards_arrow_with_hook: Scopes Request
The request returns the variable scopes for a given stackframe ID.

interface ScopesRequest extends Request {

arguments: ScopesArguments;
}
Arguments for ‘scopes’ request.


interface ScopesArguments {
/**
* Retrieve the scopes for this stackframe.
*/
frameId: number;
}
Response to ‘scopes’ request.


interface ScopesResponse extends Response {
body: {
/**
* The scopes of the stackframe. If the array has length zero, there are no scopes available.
*/
scopes: Scope[];
};
}
:leftwards_arrow_with_hook: Variables Request
Retrieves all child variables for the given variable reference.

An optional filter can be used to limit the fetched children to either named or indexed children.

interface VariablesRequest extends Request {

arguments: VariablesArguments;
}
Arguments for ‘variables’ request.


interface VariablesArguments {
/**
* The Variable reference.
*/
variablesReference: number;

/**
* Optional filter to limit the child variables to either named or indexed. If ommited, both types are fetched.
*/
filter?: 'indexed' | 'named';

/**
* The index of the first variable to return; if omitted children start at 0.
*/
start?: number;

/**
* The number of variables to return. If count is missing or 0, all variables are returned.
*/
count?: number;

/**
* Specifies details on how to format the Variable values.
*/
format?: ValueFormat;
}
Response to ‘variables’ request.


interface VariablesResponse extends Response {
body: {
/**
* All (or a range) of variables for the given variable reference.
*/
variables: Variable[];
};
}
:leftwards_arrow_with_hook: SetVariable Request
Set the variable with the given name in the variable container to a new value.

interface SetVariableRequest extends Request {

arguments: SetVariableArguments;
}
Arguments for ‘setVariable’ request.


interface SetVariableArguments {
/**
* The reference of the variable container.
*/
variablesReference: number;

/**
* The name of the variable in the container.
*/
name: string;

/**
* The value of the variable.
*/
value: string;

/**
* Specifies details on how to format the response value.
*/
format?: ValueFormat;
}
Response to ‘setVariable’ request.


interface SetVariableResponse extends Response {
body: {
/**
* The new value of the variable.
*/
value: string;

/**
* The type of the new value. Typically shown in the UI when hovering over the value.
*/
type?: string;

/**
* If variablesReference is > 0, the new value is structured and its children can be retrieved by passing variablesReference to the VariablesRequest.
*/
variablesReference?: number;

/**
* The number of named child variables.
* The client can use this optional information to present the variables in a paged UI and fetch them in chunks.
*/
namedVariables?: number;

/**
* The number of indexed child variables.
* The client can use this optional information to present the variables in a paged UI and fetch them in chunks.
*/
indexedVariables?: number;
};
}
:leftwards_arrow_with_hook: Source Request
The request retrieves the source code for a given source reference.

interface SourceRequest extends Request {
arguments: SourceArguments;
}
Arguments for ‘source’ request.


interface SourceArguments {
/**
* Specifies the source content to load. Either source.path or source.sourceReference must be specified.
*/
source?: Source;

/**
* The reference to the source. This is the same as source.sourceReference. This is provided for backward compatibility since old backends do not understand the 'source' attribute.
*/
sourceReference: number;
}
Response to ‘source’ request.


interface SourceResponse extends Response {
body: {
/**
* Content of the source reference.
*/
content: string;

/**
* Optional content type (mime type) of the source.
*/
mimeType?: string;
};
}
:leftwards_arrow_with_hook: Threads Request
The request retrieves a list of all threads.

interface ThreadsRequest extends Request {
}
Response to ‘threads’ request.


interface ThreadsResponse extends Response {
body: {
/**
* All threads.
*/
threads: Thread[];
};
}
:leftwards_arrow_with_hook: TerminateThreads Request
The request terminates the threads with the given ids.

interface TerminateThreadsRequest extends Request {

arguments: TerminateThreadsArguments;
}
Arguments for ‘terminateThreads’ request.


interface TerminateThreadsArguments {
/**
* Ids of threads to be terminated.
*/
threadIds?: number[];
}
Response to ‘terminateThreads’ request. This is just an acknowledgement, so no body field is required.


interface TerminateThreadsResponse extends Response {
}
:leftwards_arrow_with_hook: Modules Request
Modules can be retrieved from the debug adapter with the ModulesRequest which can either return all modules or a range of modules to support paging.

interface ModulesRequest extends Request {

arguments: ModulesArguments;
}
Arguments for ‘modules’ request.


interface ModulesArguments {
/**
* The index of the first module to return; if omitted modules start at 0.
*/
startModule?: number;

/**
* The number of modules to return. If moduleCount is not specified or 0, all modules are returned.
*/
moduleCount?: number;
}
Response to ‘modules’ request.


interface ModulesResponse extends Response {
body: {
/**
* All modules or range of modules.
*/
modules: Module[];

/**
* The total number of modules available.
*/
totalModules?: number;
};
}
:leftwards_arrow_with_hook: LoadedSources Request
Retrieves the set of all sources currently loaded by the debugged process.

interface LoadedSourcesRequest extends Request {

arguments?: LoadedSourcesArguments;
}
Arguments for ‘loadedSources’ request.


interface LoadedSourcesArguments {
}
Response to ‘loadedSources’ request.


interface LoadedSourcesResponse extends Response {
body: {
/**
* Set of loaded sources.
*/
sources: Source[];
};
}
:leftwards_arrow_with_hook: Evaluate Request
Evaluates the given expression in the context of the top most stack frame.

The expression has access to any variables and arguments that are in scope.

interface EvaluateRequest extends Request {

arguments: EvaluateArguments;
}
Arguments for ‘evaluate’ request.


interface EvaluateArguments {
/**
* The expression to evaluate.
*/
expression: string;

/**
* Evaluate the expression in the scope of this stack frame. If not specified, the expression is evaluated in the global scope.
*/
frameId?: number;

/**
* The context in which the evaluate request is run.
* Values:
* 'watch': evaluate is run in a watch.
* 'repl': evaluate is run from REPL console.
* 'hover': evaluate is run from a data hover.
* etc.
*/
context?: string;

/**
* Specifies details on how to format the Evaluate result.
*/
format?: ValueFormat;
}
Response to ‘evaluate’ request.


interface EvaluateResponse extends Response {
body: {
/**
* The result of the evaluate request.
*/
result: string;

/**
* The optional type of the evaluate result.
*/
type?: string;

/**
* Properties of a evaluate result that can be used to determine how to render the result in the UI.
*/
presentationHint?: VariablePresentationHint;

/**
* If variablesReference is > 0, the evaluate result is structured and its children can be retrieved by passing variablesReference to the VariablesRequest.
*/
variablesReference: number;

/**
* The number of named child variables.
* The client can use this optional information to present the variables in a paged UI and fetch them in chunks.
*/
namedVariables?: number;

/**
* The number of indexed child variables.
* The client can use this optional information to present the variables in a paged UI and fetch them in chunks.
*/
indexedVariables?: number;

/**
* Memory reference to a location appropriate for this result. For pointer type eval results, this is generally a reference to the memory address contained in the pointer.
*/
memoryReference?: string;
};
}
:leftwards_arrow_with_hook: SetExpression Request
Evaluates the given ‘value’ expression and assigns it to the ‘expression’ which must be a modifiable l-value.

The expressions have access to any variables and arguments that are in scope of the specified frame.

interface SetExpressionRequest extends Request {

arguments: SetExpressionArguments;
}
Arguments for ‘setExpression’ request.


interface SetExpressionArguments {
/**
* The l-value expression to assign to.
*/
expression: string;

/**
* The value expression to assign to the l-value expression.
*/
value: string;

/**
* Evaluate the expressions in the scope of this stack frame. If not specified, the expressions are evaluated in the global scope.
*/
frameId?: number;

/**
* Specifies how the resulting value should be formatted.
*/
format?: ValueFormat;
}
Response to ‘setExpression’ request.


interface SetExpressionResponse extends Response {
body: {
/**
* The new value of the expression.
*/
value: string;

/**
* The optional type of the value.
*/
type?: string;

/**
* Properties of a value that can be used to determine how to render the result in the UI.
*/
presentationHint?: VariablePresentationHint;

/**
* If variablesReference is > 0, the value is structured and its children can be retrieved by passing variablesReference to the VariablesRequest.
*/
variablesReference?: number;

/**
* The number of named child variables.
* The client can use this optional information to present the variables in a paged UI and fetch them in chunks.
*/
namedVariables?: number;

/**
* The number of indexed child variables.
* The client can use this optional information to present the variables in a paged UI and fetch them in chunks.
*/
indexedVariables?: number;
};
}
:leftwards_arrow_with_hook: StepInTargets Request
This request retrieves the possible stepIn targets for the specified stack frame.

These targets can be used in the ‘stepIn’ request.

The StepInTargets may only be called if the ‘supportsStepInTargetsRequest’ capability exists and is true.

interface StepInTargetsRequest extends Request {

arguments: StepInTargetsArguments;
}
Arguments for ‘stepInTargets’ request.


interface StepInTargetsArguments {
/**
* The stack frame for which to retrieve the possible stepIn targets.
*/
frameId: number;
}
Response to ‘stepInTargets’ request.


interface StepInTargetsResponse extends Response {
body: {
/**
* The possible stepIn targets of the specified source location.
*/
targets: StepInTarget[];
};
}
:leftwards_arrow_with_hook: GotoTargets Request
This request retrieves the possible goto targets for the specified source location.

These targets can be used in the ‘goto’ request.

The GotoTargets request may only be called if the ‘supportsGotoTargetsRequest’ capability exists and is true.

interface GotoTargetsRequest extends Request {

arguments: GotoTargetsArguments;
}
Arguments for ‘gotoTargets’ request.


interface GotoTargetsArguments {
/**
* The source location for which the goto targets are determined.
*/
source: Source;

/**
* The line location for which the goto targets are determined.
*/
line: number;

/**
* An optional column location for which the goto targets are determined.
*/
column?: number;
}
Response to ‘gotoTargets’ request.


interface GotoTargetsResponse extends Response {
body: {
/**
* The possible goto targets of the specified location.
*/
targets: GotoTarget[];
};
}
:leftwards_arrow_with_hook: Completions Request
Returns a list of possible completions for a given caret position and text.

The CompletionsRequest may only be called if the ‘supportsCompletionsRequest’ capability exists and is true.

interface CompletionsRequest extends Request {

arguments: CompletionsArguments;
}
Arguments for ‘completions’ request.


interface CompletionsArguments {
/**
* Returns completions in the scope of this stack frame. If not specified, the completions are returned for the global scope.
*/
frameId?: number;

/**
* One or more source lines. Typically this is the text a user has typed into the debug console before he asked for completion.
*/
text: string;

/**
* The character position for which to determine the completion proposals.
*/
column: number;

/**
* An optional line for which to determine the completion proposals. If missing the first line of the text is assumed.
*/
line?: number;
}
Response to ‘completions’ request.


interface CompletionsResponse extends Response {
body: {
/**
* The possible completions for .
*/
targets: CompletionItem[];
};
}
:leftwards_arrow_with_hook: ExceptionInfo Request
Retrieves the details of the exception that caused this event to be raised.

interface ExceptionInfoRequest extends Request {

arguments: ExceptionInfoArguments;
}
Arguments for ‘exceptionInfo’ request.


interface ExceptionInfoArguments {
/**
* Thread for which exception information should be retrieved.
*/
threadId: number;
}
Response to ‘exceptionInfo’ request.


interface ExceptionInfoResponse extends Response {
body: {
/**
* ID of the exception that was thrown.
*/
exceptionId: string;

/**
* Descriptive text for the exception provided by the debug adapter.
*/
description?: string;

/**
* Mode that caused the exception notification to be raised.
*/
breakMode: ExceptionBreakMode;

/**
* Detailed information about the exception.
*/
details?: ExceptionDetails;
};
}
:leftwards_arrow_with_hook: ReadMemory Request
Reads bytes from memory at the provided location.

interface ReadMemoryRequest extends Request {

arguments: ReadMemoryArguments;
}
Arguments for ‘readMemory’ request.


interface ReadMemoryArguments {
/**
* Memory reference to the base location from which data should be read.
*/
memoryReference: string;

/**
* Optional offset (in bytes) to be applied to the reference location before reading data. Can be negative.
*/
offset?: number;

/**
* Number of bytes to read at the specified location and offset.
*/
count: number;
}
Response to ‘readMemory’ request.


interface ReadMemoryResponse extends Response {
body?: {
/**
* The address of the first byte of data returned. Treated as a hex value if prefixed with '0x', or as a decimal value otherwise.
*/
address: string;

/**
* The number of unreadable bytes encountered after the last successfully read byte. This can be used to determine the number of bytes that must be skipped before a subsequent 'readMemory' request will succeed.
*/
unreadableBytes?: number;

/**
* The bytes read from memory, encoded using base64.
*/
data?: string;
};
}
:leftwards_arrow_with_hook: Disassemble Request
Disassembles code stored at the provided location.

interface DisassembleRequest extends Request {

arguments: DisassembleArguments;
}
Arguments for ‘disassemble’ request.


interface DisassembleArguments {
/**
* Memory reference to the base location containing the instructions to disassemble.
*/
memoryReference: string;

/**
* Optional offset (in bytes) to be applied to the reference location before disassembling. Can be negative.
*/
offset?: number;

/**
* Optional offset (in instructions) to be applied after the byte offset (if any) before disassembling. Can be negative.
*/
instructionOffset?: number;

/**
* Number of instructions to disassemble starting at the specified location and offset. An adapter must return exactly this number of instructions - any unavailable instructions should be replaced with an implementation-defined 'invalid instruction' value.
*/
instructionCount: number;

/**
* If true, the adapter should attempt to resolve memory addresses and other values to symbolic names.
*/
resolveSymbols?: boolean;
}
Response to ‘disassemble’ request.


interface DisassembleResponse extends Response {
body?: {
/**
* The list of disassembled instructions.
*/
instructions: DisassembledInstruction[];
};
}

}
