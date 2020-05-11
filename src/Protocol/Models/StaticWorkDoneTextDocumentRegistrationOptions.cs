using System.Text.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// Static registration options to be returned in the initialize request.
    /// </summary>
    public class StaticWorkDoneTextDocumentRegistrationOptions : StaticTextDocumentRegistrationOptions, IWorkDoneProgressOptions
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        /// <inheritdoc />
        public bool WorkDoneProgress { get; set; }
    }
}
