using System;

namespace OmniSharp.Extensions.JsonRpc
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class ProcessAttribute : Attribute
    {
        public ProcessAttribute(RequestProcessType type)
        {
            Type = type;
        }

        public RequestProcessType Type { get; }
    }
}