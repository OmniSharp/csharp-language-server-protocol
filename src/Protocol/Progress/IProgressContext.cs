using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Progress
{
    public interface IProgressContext
    {
        ProgressToken ProgressToken { get; }
    }
}
