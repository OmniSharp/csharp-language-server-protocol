using OmniSharp.Extensions.DebugAdapter.Protocol.Models;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    public class ScopesResponse
    {
        /// <summary>
        /// The scopes of the stackframe.If the array has length zero, there are no scopes available.
        /// </summary>
        public Container<Scope> Scopes { get; set; }
    }

}
