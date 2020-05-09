using System;
using System.Reflection;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    class BooleanOrConverter : JsonConverter
    {
        private static readonly MethodInfo WriteJsonGenericMethod = typeof(BooleanOrConverter)
            .GetTypeInfo()
            .GetMethod(nameof(WriteJsonGeneric), BindingFlags.NonPublic | BindingFlags.Static);
        private static readonly MethodInfo ReadJsonGenericMethod = typeof(BooleanOrConverter)
            .GetTypeInfo()
            .GetMethod(nameof(ReadJsonGeneric), BindingFlags.NonPublic | BindingFlags.Static);
        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            if (value == null) return;
            WriteJsonGenericMethod.MakeGenericMethod(value.GetType().GetTypeInfo().GenericTypeArguments[0])
                .Invoke(null, new object[] { writer, value, serializer });
        }

        private static void WriteJsonGeneric<T>(JsonWriter writer, BooleanOr<T> value, JsonSerializer serializer)
        {
            if (value.IsBool)
            {
                new JValue(value.Bool).WriteTo(writer);
                return;
            }

            if (value.IsValue)
            {
                  JsonSerializer.Serialize(writer, value.Value, options);
                return;
            }

            writer.WriteNull();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var parentType = objectType.GetTypeInfo().GenericTypeArguments[0];
            return ReadJsonGenericMethod.MakeGenericMethod(parentType)
                .Invoke(null, new object[] { reader, existingValue, serializer });
        }

        private static BooleanOr<T> ReadJsonGeneric<T>(JsonReader reader, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Boolean)
            {
                return new BooleanOr<T>((bool)reader.Value);
            }

            if (reader.TokenType == JsonToken.StartObject)
            {
                return new BooleanOr<T>(JObject.Load(reader).ToObject<T>(serializer));
            }

            return new BooleanOr<T>(default(T));
        }



        public override bool CanConvert(Type objectType) => objectType.GetTypeInfo().IsGenericType && objectType.GetTypeInfo().GetGenericTypeDefinition() == typeof(BooleanOr<>);
    }
}
