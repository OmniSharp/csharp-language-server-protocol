using OmniSharp.Extensions.Embedded.MediatR;
using Newtonsoft.Json.Linq;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class ConfigurationParams : IRequest<Container<JToken>>
    {
        public Container<ConfigurationItem> Items { get; set; }
    }
}
