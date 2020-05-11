using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities
{
    [JsonConverter(typeof(Converter))]
    public class TextDocumentSync
    {
        public TextDocumentSync(TextDocumentSyncKind kind)
        {
            Kind = kind;
        }
        public TextDocumentSync(TextDocumentSyncOptions value)
        {
            Options = value;
        }
        public bool HasKind => this.Options == null;
        public TextDocumentSyncKind Kind { get; set; }
        public bool HasOptions => this.Options != null;
        public TextDocumentSyncOptions Options { get; set; }
        public object Value => Options ?? (object)Kind;

        public static implicit operator TextDocumentSync(TextDocumentSyncKind value)
        {
            return new TextDocumentSync(value);
        }

        public static implicit operator TextDocumentSync(TextDocumentSyncOptions value)
        {
            return new TextDocumentSync(value);
        }

        class Converter : JsonConverter<TextDocumentSync>
        {
            public override void Write(Utf8JsonWriter writer, TextDocumentSync value, JsonSerializerOptions options)
            {
                if (value.HasOptions)
                {
                    JsonSerializer.Serialize(writer, value.Options, options);
                }
                else if (value.HasKind)
                {
                    writer.WriteNumberValue((int) value.Kind);
                }
            }

            public override TextDocumentSync Read(ref Utf8JsonReader reader, Type typeToConvert,
                JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.Number)
                {
                    return new TextDocumentSync((TextDocumentSyncKind) reader.GetInt32());
                }

                return new TextDocumentSync(JsonSerializer.Deserialize<TextDocumentSyncOptions>(ref reader, options));
            }
        }
    }
}
