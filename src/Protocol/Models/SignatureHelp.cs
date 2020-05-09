using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// Signature help represents the signature of something
    /// callable. There can be multiple signature but only one
    /// active and only one active parameter.
    /// </summary>
    public class SignatureHelp
    {
        /// <summary>
        /// One or more signatures.
        /// </summary>
        public Container<SignatureInformation> Signatures { get; set; }

        /// <summary>
        /// The active signature.
        /// </summary>
        [Optional]
        public int? ActiveSignature { get; set; }

        /// <summary>
        /// The active parameter of the active signature.
        /// </summary>
        [Optional]
        public int? ActiveParameter { get; set; }
    }
}
