using System.Text.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class WorkDoneProgressOptions : IWorkDoneProgressOptions
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public bool WorkDoneProgress { get; set; }
    }
}
