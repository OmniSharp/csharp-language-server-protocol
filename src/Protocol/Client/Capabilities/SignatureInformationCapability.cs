using System.Text.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public class SignatureInformationCapability
    {
        /// <summary>
        /// Client supports the follow content formats for the content property. The order describes the preferred format of the client.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public Container<MarkupKind> DocumentationFormat { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public SignatureParameterInformationCapability ParameterInformation { get; set; }
    }
}
