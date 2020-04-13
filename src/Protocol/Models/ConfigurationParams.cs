using MediatR;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [Method(WorkspaceNames.WorkspaceConfiguration)]
    public class ConfigurationParams : IRequest<Container<JToken>>
    {
        public Container<ConfigurationItem> Items { get; set; }
    }
}
