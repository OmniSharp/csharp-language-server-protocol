using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// Static registration options to be returned in the initialize request.
    /// </summary>
    public class StaticTextDocumentRegistrationOptions : TextDocumentRegistrationOptions, IStaticRegistrationOptions
    {
        /// <summary>
        /// The id used to register the request. The id can be used to deregister the request again. See also Registration#id.
        /// </summary>
        [Optional]
        public string Id { get; set; }
    }
}
