using System.Text.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public interface IWorkDoneProgressOptions
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        bool WorkDoneProgress { get; set; }
    }
}
