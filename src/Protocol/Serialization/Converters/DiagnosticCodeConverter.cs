using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    class DiagnosticCodeConverter : JsonConverter<DiagnosticCode>
    {
        public override void WriteJson(JsonWriter writer, DiagnosticCode value, JsonSerializer serializer)
        {
            var v = value as DiagnosticCode?;

            if (v.HasValue)
            {
                new JValue(v.Value.Value).WriteTo(writer);
            }
            else
            {
                writer.WriteNull();
            }
        }

        public override DiagnosticCode ReadJson(JsonReader reader, Type objectType, DiagnosticCode existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                return new DiagnosticCode((string)reader.Value);
            }
            if (reader.TokenType == JsonToken.Integer)
            {
                return new DiagnosticCode((long)reader.Value);
            }
            return null;
        }

        public override bool CanRead => true;
    }
}
