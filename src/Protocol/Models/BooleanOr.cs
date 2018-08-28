namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public struct BooleanOr<T>
    {
        private T _value;
        private bool? _bool;
        public BooleanOr(T value)
        {
            _value = value;
            _bool = null;
        }
        public BooleanOr(bool value)
        {
            _value = default;
            _bool = value;
        }

        public bool IsValue => this._value != null;
        public T Value
        {
            get { return this._value; }
            set
            {
                this._value = value;
                this._bool = null;
            }
        }

        public bool IsBool => this._bool.HasValue;
        public bool Bool
        {
            get { return this._bool.HasValue && this._bool.Value; }
            set
            {
                this.Value = default;
                this._bool = value;
            }
        }
        public object RawValue
        {
            get
            {
                if (IsBool) return Bool;
                if (IsValue) return Value;
                return null;
            }
        }

        public static implicit operator BooleanOr<T>(T value)
        {
            return new BooleanOr<T>(value);
        }

        public static implicit operator BooleanOr<T>(bool value)
        {
            return new BooleanOr<T>(value);
        }
    }

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
        public T Command
        {
            get { return this._command; }
            set
            {
                this._command = value;
                this._codeAction = null;
            }
        }

        public bool IsCodeAction => this._codeAction != null;
        public bool CodeAction
        {
            get { return this._codeAction; }
            set
            {
                this._command = default;
                this._codeAction = value;
            }
        }
        public object RawValue
        {
            get
            {
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
    }

    public class CodeAction
    {
        /// <summary>
        /// A short, human-readable, title for this code action.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The kind of the code action.
        ///
        /// Used to filter code actions.
        /// </summary>
        [Optional]
        public CodeActionKind Kind { get; set; }

        /// <summary>
        /// The diagnostics that this code action resolves.
        /// </summary>
        [Optional]
        public Container<Diagnostic> Diagnostics { get; set; }

        /// <summary>
        /// The workspace edit this code action performs.
        /// </summary>
        [Optional]
        public WorkspaceEdit Edit { get; set; }

        /// <summary>
        /// A command this code action executes. If a code action
        /// provides an edit and a command, first the edit is
        /// executed and then the command.
        /// </summary>
        [Optional]
        public Command Command { get; set; }
    }
}
