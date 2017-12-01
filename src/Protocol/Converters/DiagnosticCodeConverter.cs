using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Converters
{
    class DiagnosticCodeConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
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

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
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

        public override bool CanConvert(Type objectType) => objectType == typeof(DiagnosticCode);
    }
}