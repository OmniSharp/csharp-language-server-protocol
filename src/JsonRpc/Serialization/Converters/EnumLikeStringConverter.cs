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
            reader.TokenType switch {
                JsonToken.String => (IEnumLikeString?) CreateEnumLikeString(objectType, reader.Value);
                _                => (IEnumLikeString) Activator.CreateInstance(objectType, null)
            };

        public override bool CanRead => true;

        private static object? CreateEnumLikeString(Type objectType, object? value)
        {
            if (objectType.IsGenericType
                && objectType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                // TODO: If the value is null, should we return null? ReadJson()'s return type isn't nullable...
                if (value is null)
                {
                    return null;
                }

                return Activator.CreateInstance(objectType.GetGenericArguments()[0], (string) value);
            }

            return Activator.CreateInstance(objectType, (string?) value);
        }
    }
}
