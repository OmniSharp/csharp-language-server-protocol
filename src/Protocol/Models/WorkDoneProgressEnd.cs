namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// Signaling the end of a progress reporting is done using the following payload
    /// </summary>
    public class WorkDoneProgressEnd : WorkDoneProgress
    {
        public WorkDoneProgressEnd() : base(WorkDoneProgressKind.End) { }
    }
}
