using System;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    class BooleanNumberStringConverter : JsonConverter<BooleanNumberString>
    {
        public override void WriteJson(JsonWriter writer, BooleanNumberString value, JsonSerializer serializer)
        {
            if (value.IsBool) serializer.Serialize(writer, value.Bool);
            else if (value.IsLong) serializer.Serialize(writer, value.Long);
            else if (value.IsString) serializer.Serialize(writer, value.String);
            else writer.WriteNull();
        }

        public override BooleanNumberString ReadJson(JsonReader reader, Type objectType, BooleanNumberString existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Integer)
            {
                return new BooleanNumberString((long)reader.Value);
            }

            if (reader.TokenType == JsonToken.String)
            {
                return new BooleanNumberString((string)reader.Value);
            }

            if (reader.TokenType == JsonToken.Boolean)
            {
                return new BooleanNumberString((bool)reader.Value);
            }

            return new BooleanNumberString();
        }

        public override bool CanRead => true;
    }
}
