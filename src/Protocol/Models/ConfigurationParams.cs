using MediatR;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [Method(WorkspaceNames.Configuration, Direction.ServerToClient)]
    public class ConfigurationParams : IRequest<Container<JToken>>
    {
        public Container<ConfigurationItem> Items { get; set; }
    }
}
