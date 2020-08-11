using System.ComponentModel;
using Newtonsoft.Json;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public abstract class HandlerIdentity
    {
        [JsonProperty(Constants.PrivateHandlerId, DefaultValueHandling = DefaultValueHandling.Ignore)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string __identity { get; set; }
    }
}
