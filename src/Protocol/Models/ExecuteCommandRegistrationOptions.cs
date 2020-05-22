namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// Execute command registration options.
    /// </summary>
    public class ExecuteCommandRegistrationOptions : WorkDoneTextDocumentRegistrationOptions, IExecuteCommandOptions
    {
        /// <summary>
        /// The commands to be executed on the server
        /// </summary>
        public Container<string> Commands { get; set; }
    }
}
