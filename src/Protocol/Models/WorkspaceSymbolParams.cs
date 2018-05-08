using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// The parameters of a Workspace Symbol Request.
    /// </summary>
    public class WorkspaceSymbolParams : IRequest<WorkspaceSymbolInformationContainer>
    {
        /// <summary>
        /// A non-empty query string
        /// </summary>
        public string Query { get; set; }
    }
}
