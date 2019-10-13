using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class StaticRegistrationOptions : IStaticRegistrationOptions
    {
        /// <summary>
        /// The id used to register the request. The id can be used to deregister the request again. See also Registration#id.
        /// </summary>
        [Optional]
        public string Id { get; set; }
    }
}
