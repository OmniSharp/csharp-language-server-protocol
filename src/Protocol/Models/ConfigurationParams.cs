using System.Text.Json;
using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [Method(WorkspaceNames.WorkspaceConfiguration)]
    public class ConfigurationParams : IRequest<Container<JsonElement>>
    {
        public Container<ConfigurationItem> Items { get; set; }
    }
}
