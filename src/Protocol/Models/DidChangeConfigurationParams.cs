using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [Method(WorkspaceNames.DidChangeConfiguration)]
    public class DidChangeConfigurationParams : IRequest
    {
        /// <summary>
        ///  The actual changed settings
        /// </summary>
        public JToken Settings { get; set; }
    }
}
