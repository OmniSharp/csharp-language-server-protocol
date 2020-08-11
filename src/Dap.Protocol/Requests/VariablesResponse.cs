using OmniSharp.Extensions.DebugAdapter.Protocol.Models;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    public class VariablesResponse
    {
        /// <summary>
        /// All(or a range) of variables for the given variable reference.
        /// </summary>
        public Container<Variable> Variables { get; set; }
    }
}
