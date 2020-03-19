namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public abstract class WorkDoneProgress
    {
        public WorkDoneProgress(string kind)
        {
            Kind = kind;
        }

        public string Kind { get; }
    }
}