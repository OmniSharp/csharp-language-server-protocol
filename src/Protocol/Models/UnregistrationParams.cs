using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class UnregistrationParams
    {
        public UnregistrationContainer Unregisterations { get; set; }
    }
}
