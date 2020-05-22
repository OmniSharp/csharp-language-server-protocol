using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public interface IWorkDoneProgressOptions
    {
        [Optional]
        bool WorkDoneProgress { get; set; }
    }
}
