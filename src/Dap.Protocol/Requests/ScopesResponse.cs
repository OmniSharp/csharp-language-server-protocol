namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public class ScopesResponse
    {
        /// <summary>
        /// The scopes of the stackframe.If the array has length zero, there are no scopes available.
        /// </summary>
        public Container<Scope> Scopes { get; set; }
    }

}
