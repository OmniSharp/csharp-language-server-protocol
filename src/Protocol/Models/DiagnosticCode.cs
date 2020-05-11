using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [JsonConverter(typeof(JsonConverter))]
    public struct DiagnosticCode
    {
        public DiagnosticCode(long value)
        {
            Long = value;
            String = null;
        }

        public DiagnosticCode(string value)
        {
            Long = 0;
            String = value;
        }

        public bool IsLong => this.String == null;
        public long Long { get; set; }
        public bool IsString => this.String != null;
        public string String { get; set; }

        public static implicit operator DiagnosticCode(long value)
        {
            return new DiagnosticCode(value);
        }

        public static implicit operator DiagnosticCode(string value)
        {
            return new DiagnosticCode(value);
        }

        public static implicit operator long(DiagnosticCode value)
        {
            return value.IsLong ? value.Long : 0;
        }

        public static implicit operator string(DiagnosticCode value)
        {
            return value.IsString ? value.String : null;
        }

        class Converter : JsonConverter<DiagnosticCode>
        {
            public override void Write(Utf8JsonWriter writer, DiagnosticCode value, JsonSerializerOptions options)
            {
                if (value.IsLong) JsonSerializer.Serialize(writer, value.Long, options);
                if (value.IsString) JsonSerializer.Serialize(writer, value.String, options);
            }

            public override DiagnosticCode Read(ref Utf8JsonReader reader, Type typeToConvert,
                JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.String)
                {
                    return new DiagnosticCode(reader.GetString());
                }

                if (reader.TokenType == JsonTokenType.Number)
                {
                    return new DiagnosticCode(reader.GetInt64());
                }

                return null;
            }
        }
    }
}
