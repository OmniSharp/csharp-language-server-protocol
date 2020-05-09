using System;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    class DiagnosticCodeConverter : JsonConverter<DiagnosticCode>
    {
        public override void Write(Utf8JsonWriter writer, DiagnosticCode value, JsonSerializerOptions options)
        {
            if (value.IsLong)   JsonSerializer.Serialize(writer, value.Long, options);
            if (value.IsString)   JsonSerializer.Serialize(writer, value.String, options);
        }

        public override DiagnosticCode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonToken.String)
            {
                return new DiagnosticCode((string) reader.Value);
            }

            if (reader.TokenType == JsonToken.Integer)
            {
                return new DiagnosticCode((long) reader.Value);
            }

            return null;
        }


    }

    class NullableDiagnosticCodeConverter : JsonConverter<DiagnosticCode?>
    {
        public override void Write(Utf8JsonWriter writer, DiagnosticCode? value, JsonSerializerOptions options)
        {
            if (!value.HasValue)
            {
                writer.WriteNull();
            }
            else
            {
                if (value.Value.IsLong)   JsonSerializer.Serialize(writer, value.Value.Long, options);
                if (value.Value.IsString)   JsonSerializer.Serialize(writer, value.Value.String, options);
            }
        }

        public override DiagnosticCode? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonToken.String)
            {
                return new DiagnosticCode((string) reader.Value);
            }

            if (reader.TokenType == JsonToken.Integer)
            {
                return new DiagnosticCode((long) reader.Value);
            }

            return null;
        }


    }
}
