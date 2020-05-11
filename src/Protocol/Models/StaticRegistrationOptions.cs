using System.Text.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class StaticRegistrationOptions : IStaticRegistrationOptions
    {
        /// <summary>
        /// The id used to register the request. The id can be used to deregister the request again. See also Registration#id.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public string Id { get; set; }
    }
}
