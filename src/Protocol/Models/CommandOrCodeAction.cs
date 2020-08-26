using System.Diagnostics;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [JsonConverter(typeof(CommandOrCodeActionConverter))]
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public struct CommandOrCodeAction
    {
        private CodeAction? _codeAction;
        private Command? _command;

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

        public bool IsCommand => _command != null;

        public Command? Command
        {
            get => _command;
            set {
                _command = value;
                _codeAction = null;
            }
        }

        public bool IsCodeAction => _codeAction != null;

        public CodeAction? CodeAction
        {
            get => _codeAction;
            set {
                _command = default;
                _codeAction = value;
            }
        }

        public object? RawValue
        {
            get {
                if (IsCommand) return Command!;
                if (IsCodeAction) return CodeAction!;
                return default;
            }
        }

        public static implicit operator CommandOrCodeAction(Command value) => new CommandOrCodeAction(value);

        public static implicit operator CommandOrCodeAction(CodeAction value) => new CommandOrCodeAction(value);

        private string DebuggerDisplay => $"{( IsCommand ? $"command: {Command}" : IsCodeAction ? $"code action: {CodeAction}" : "..." )}";

        /// <inheritdoc />
        public override string ToString() => DebuggerDisplay;
    }
}
