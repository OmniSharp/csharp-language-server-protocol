using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    ///  General paramters to to regsiter for a capability.
    /// </summary>
    public class Registration
    {
        /// <summary>
        ///  The id used to register the request. The id can be used to deregister
        ///  the request again.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///  The method / capability to register for.
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        ///  Options necessary for the registration.
        /// </summary>
        [Optional]
        public object RegisterOptions { get; set; }
    }
}
