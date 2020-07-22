using System.Diagnostics;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public struct CommandOrCodeAction
    {
        private CodeAction _codeAction;
        private Command _command;
        public CommandOrCodeAction(CodeAction value)
        {
            _codeAction = value;
            _command = default;
        }
        public CommandOrCodeAction(Command value)
        {
            _codeAction = default;
            _command = value;
        }

        public bool IsCommand => this._command != null;
        public Command Command
        {
            get { return this._command; }
            set {
                this._command = value;
                this._codeAction = null;
            }
        }

        public bool IsCodeAction => this._codeAction != null;
        public CodeAction CodeAction
        {
            get { return this._codeAction; }
            set {
                this._command = default;
                this._codeAction = value;
            }
        }
        public object RawValue
        {
            get {
                if (IsCommand) return Command;
                if (IsCodeAction) return CodeAction;
                return default;
            }
        }

        public static implicit operator CommandOrCodeAction(Command value)
        {
            return new CommandOrCodeAction(value);
        }

        public static implicit operator CommandOrCodeAction(CodeAction value)
        {
            return new CommandOrCodeAction(value);
        }

        private string DebuggerDisplay => $"{(IsCommand ? $"command: {Command}" : IsCodeAction ? $"code action: {CodeAction}" : "...")}";
        /// <inheritdoc />
        public override string ToString() => DebuggerDisplay;
    }
}
