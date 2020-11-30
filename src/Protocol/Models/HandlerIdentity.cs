using System.ComponentModel;
using Newtonsoft.Json;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public interface IHandlerIdentity
    {
        [JsonProperty(Constants.PrivateHandlerId, DefaultValueHandling = DefaultValueHandling.Ignore)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        // ReSharper disable once InconsistentNaming
        string __identity { get; init; }
    }
}
