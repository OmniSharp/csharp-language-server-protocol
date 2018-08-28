using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    class CodeActionKindConverter : JsonConverter<CodeActionKind>
    {
        public override void WriteJson(JsonWriter writer, CodeActionKind value, JsonSerializer serializer)
        {
            new JValue(value.Kind).WriteTo(writer);
        }

        public override CodeActionKind ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.String:
                    return new CodeActionKind((string)reader.Value);
            }

            return new CodeActionKind(null);
        }

        public override bool CanRead => true;
    }
}
