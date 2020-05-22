using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    class NumberEnumConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            new JValue(value).WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return Enum.Parse(Nullable.GetUnderlyingType(objectType) ?? objectType, reader.Value.ToString());
        }

        public override bool CanRead => true;

        public override bool CanConvert(Type objectType) => objectType.GetTypeInfo().IsEnum;
    }
}
