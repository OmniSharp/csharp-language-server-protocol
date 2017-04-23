using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Lsp.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class UnregistrationParams
    {
        public UnregistrationContainer Unregisterations { get; set; }
    }
}