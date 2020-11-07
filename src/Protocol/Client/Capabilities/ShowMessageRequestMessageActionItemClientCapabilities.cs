using System;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    [Obsolete(Constants.Proposal)]
    public class ShowMessageRequestMessageActionItemClientCapabilities
    {
        /// <summary>
        /// Whether the client supports additional attribues which
        /// are preserved and send back to the server in the
        /// request's response.
        /// </summary>
        [Optional]
        public bool AdditionalPropertiesSupport { get; set; }
    }
}