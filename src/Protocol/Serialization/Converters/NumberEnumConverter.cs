using System;
using System.Reflection;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    class NumberEnumConverter : JsonConverter
    {
        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            new JValue(value).WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return Enum.Parse(Nullable.GetUnderlyingType(objectType) ?? objectType, reader.Value.ToString());
        }



        public override bool CanConvert(Type objectType) => objectType.GetTypeInfo().IsEnum;
    }
}
