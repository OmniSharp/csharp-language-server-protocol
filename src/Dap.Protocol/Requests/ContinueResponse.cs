using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public class ContinueResponse
    {
        /// <summary>
        /// If true, the 'continue' request has ignored the specified thread and continued all threads instead.If this attribute is missing a value of 'true' is assumed for backward compatibility.
        /// </summary>
        [Optional] public bool? AllThreadsContinued { get; set; }
    }

}
