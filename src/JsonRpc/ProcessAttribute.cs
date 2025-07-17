namespace OmniSharp.Extensions.JsonRpc
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class ProcessAttribute : Attribute
    {
        public ProcessAttribute(RequestProcessType type) => Type = type;

        public RequestProcessType Type { get; }
    }

    /// <summary>
    /// Used by source generation to known handlers to the assembly metadata
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class AssemblyJsonRpcHandlersAttribute : Attribute
    {
        public Type[] Types { get; }

        public AssemblyJsonRpcHandlersAttribute(params Type[] handlerTypes)
        {
            Types = handlerTypes;
        }
    }
}
