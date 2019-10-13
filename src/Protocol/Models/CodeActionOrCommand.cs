namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public struct CodeActionOrCommand
    {

        public CodeActionOrCommand(CodeAction codeAction)
        {
            CodeAction = codeAction;
            Command = null;
        }

        public CodeActionOrCommand(Command command)
        {
            CodeAction = null;
            Command = command;
        }

        public bool IsCodeAction => CodeAction != null;
        public CodeAction CodeAction { get; }

        public bool IsCommand => Command != null;
        public Command Command { get; }

        public static implicit operator CodeActionOrCommand(CodeAction codeAction)
        {
            return new CodeActionOrCommand(codeAction);
        }

        public static implicit operator CodeActionOrCommand(Command command)
        {
            return new CodeActionOrCommand(command);
        }
    }
}