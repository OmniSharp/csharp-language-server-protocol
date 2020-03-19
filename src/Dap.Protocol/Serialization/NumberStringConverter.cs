using System;
using Newtonsoft.Json;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Serialization
{
    class NumberStringConverter : JsonConverter<NumberString>
    {
        public override void WriteJson(JsonWriter writer, NumberString value, JsonSerializer serializer)
        {
            if (value.IsLong) serializer.Serialize(writer, value.Long);
            else if (value.IsString) serializer.Serialize(writer, value.String);
            else writer.WriteNull();
        }

        public override NumberString ReadJson(JsonReader reader, Type objectType, NumberString existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Integer)
            {
                return new NumberString((long)reader.Value);
            }

            if (reader.TokenType == JsonToken.String)
            {
                return new NumberString((string)reader.Value);
            }

            return new NumberString();
        }

        public override bool CanRead => true;
    }
}
