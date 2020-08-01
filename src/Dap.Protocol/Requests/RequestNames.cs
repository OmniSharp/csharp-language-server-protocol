namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    public static class RequestNames
    {
        public const string Initialize = "initialize";
        public const string ConfigurationDone = "configurationDone";
        public const string Launch = "launch";
        public const string Attach = "attach";
        public const string Restart = "restart";
        public const string Disconnect = "disconnect";
        public const string Terminate = "terminate";
        public const string BreakpointLocations = "breakpointLocations";
        public const string SetBreakpoints = "setBreakpoints";
        public const string SetFunctionBreakpoints = "setFunctionBreakpoints";
        public const string SetExceptionBreakpoints = "setExceptionBreakpoints";
        public const string DataBreakpointInfo = "dataBreakpointInfo";
        public const string SetDataBreakpoints = "setDataBreakpoints";
        public const string SetInstructionBreakpoints = "setInstructionBreakpoints";
        public const string Continue = "continue";
        public const string Next = "next";
        public const string StepIn = "stepIn";
        public const string StepOut = "stepOut";
        public const string StepBack = "stepBack";
        public const string ReverseContinue = "reverseContinue";
        public const string RestartFrame = "restartFrame";
        public const string Goto = "goto";
        public const string Pause = "pause";
        public const string StackTrace = "stackTrace";
        public const string Scopes = "scopes";
        public const string Variables = "variables";
        public const string SetVariable = "setVariable";
        public const string Source = "source";

        public const string Threads = "threads";
        public const string TerminateThreads = "terminateThreads";
        public const string Modules = "modules";
        public const string LoadedSources = "loadedSources";
        public const string Evaluate = "evaluate";
        public const string SetExpression = "setExpression";
        public const string StepInTargets = "stepInTargets";
        public const string GotoTargets = "gotoTargets";
        public const string Completions = "completions";
        public const string ExceptionInfo = "exceptionInfo";
        public const string ReadMemory = "readMemory";
        public const string Disassemble = "disassemble";
        public const string RunInTerminal = "runInTerminal";
        public const string Cancel = "cancel";
    }

}
