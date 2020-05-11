using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [JsonConverter(typeof(Converter))]
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

        class Converter : JsonConverter<CommandOrCodeAction>
        {
            public override void Write(Utf8JsonWriter writer, CommandOrCodeAction value, JsonSerializerOptions options)
            {
                if (value.IsCodeAction)
                {
                    JsonSerializer.Serialize(writer, value.CodeAction, options);
                }
                else if (value.IsCommand)
                {
                    JsonSerializer.Serialize(writer, value.Command, options);
                }
                else
                {
                    writer.WriteNullValue();
                }
            }

            public override CommandOrCodeAction Read(ref Utf8JsonReader reader, Type typeToConvert,
                JsonSerializerOptions options)
            {
                if (reader.TokenType != JsonTokenType.StartObject)
                    throw new JsonException("Expected an object");

                // TODO: Is there a better way to do this?
                // Perhaps a way to peek into the reader and see if there is a property name, and come back to start here?
                var isCommand = false;
                string title = null;
                string command = null;
                Command innerCommand = null;
                JsonElement? arguments = null;
                bool isPreferred = false;
                CodeActionKind kind = null;
                Container<Diagnostic> diagnostics = null;
                WorkspaceEdit edit = null;
                string propertyName = null;

                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndObject)
                    {
                        reader.Read();
                        break;
                    }

                    if (reader.TokenType == JsonTokenType.PropertyName)
                    {
                        propertyName = reader.GetString();
                        continue;
                    }

                    switch (propertyName)
                    {
                        case nameof(command):
                            if (reader.TokenType == JsonTokenType.String) command = reader.GetString();
                            else if (reader.TokenType == JsonTokenType.StartObject)
                                innerCommand = JsonSerializer.Deserialize<Command>(ref reader, options);
                            break;
                        case nameof(title):
                            title = reader.GetString();
                            break;
                        case nameof(arguments):
                            arguments = JsonSerializer.Deserialize<JsonElement>(ref reader, options);
                            break;
                        case nameof(isPreferred):
                            isPreferred = reader.GetBoolean();
                            break;
                        case nameof(kind):
                            kind = new CodeActionKind(reader.GetString());
                            break;
                        case nameof(diagnostics):
                            diagnostics = JsonSerializer.Deserialize<Container<Diagnostic>>(ref reader, options);
                            break;
                        case nameof(edit):
                            edit = JsonSerializer.Deserialize<WorkspaceEdit>(ref reader, options);
                            break;
                        default:
                            throw new JsonException($"Unsupported property found {propertyName}");
                    }
                }

                if (command != null)
                {
                    return new Command() {
                        Arguments = arguments,
                        Name = command,
                        Title = title
                    };
                }

                return new CodeAction() {
                    Command = innerCommand,
                    Diagnostics = diagnostics,
                    Edit = edit,
                    Kind = kind,
                    Title = title,
                    IsPreferred = isPreferred
                };
            }
        }
    }
}
