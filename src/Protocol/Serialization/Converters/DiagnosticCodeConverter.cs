using System;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    internal class DiagnosticCodeConverter : JsonConverter<DiagnosticCode>
    {
        public override void WriteJson(JsonWriter writer, DiagnosticCode value, JsonSerializer serializer)
        {
            if (value.IsLong) serializer.Serialize(writer, value.Long);
            if (value.IsString) serializer.Serialize(writer, value.String);
        }

        public override DiagnosticCode ReadJson(
            JsonReader reader, Type objectType, DiagnosticCode existingValue,
            bool hasExistingValue, JsonSerializer serializer
        )
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

        public override bool CanRead => true;
    }
}
