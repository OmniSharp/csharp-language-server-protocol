using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class WorkDoneProgressOptions : IWorkDoneProgressOptions
    {
        [Optional]
        public bool WorkDoneProgress { get; set; }
    }
}
