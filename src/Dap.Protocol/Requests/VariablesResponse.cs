namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public class VariablesResponse
    {
        /// <summary>
        /// All(or a range) of variables for the given variable reference.
        /// </summary>
        public Container<Variable> variables { get; set; }
    }

}
