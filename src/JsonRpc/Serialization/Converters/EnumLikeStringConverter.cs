using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OmniSharp.Extensions.JsonRpc.Serialization.Converters
{
    internal class EnumLikeStringConverter : JsonConverter<IEnumLikeString>
    {
        public override void WriteJson(JsonWriter writer, IEnumLikeString value, JsonSerializer serializer) => new JValue(value.ToString()).WriteTo(writer);

        public override IEnumLikeString ReadJson(
            JsonReader reader, Type objectType, IEnumLikeString existingValue,
            bool hasExistingValue,
            JsonSerializer serializer
        ) =>
            ( reader.TokenType, Nullable.GetUnderlyingType(objectType) ) switch {
                (JsonToken.String, null)         => (IEnumLikeString) Activator.CreateInstance(objectType, (string) reader.Value),
                (JsonToken.String, { } realType) => (IEnumLikeString) Activator.CreateInstance(realType, (string) reader.Value),
                (_, { })                         => (IEnumLikeString) Activator.CreateInstance(objectType, null),
                _                                => null
            };

        public override bool CanRead => true;
    }
}
