namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class WorkspaceFolderRegistrationOptions : IWorkspaceFolderOptions
    {
        public bool Supported { get; set; }
        public BooleanString ChangeNotifications { get; set; }
    }
}
