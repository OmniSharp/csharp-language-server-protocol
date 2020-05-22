using System.ComponentModel;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public class DynamicCapability : IDynamicCapability
    {
        /// <summary>
        /// Whether completion supports dynamic registration.
        /// </summary>
        [Optional, EditorBrowsable(EditorBrowsableState.Never)]
        public bool DynamicRegistration { get; set; }
    }
}
