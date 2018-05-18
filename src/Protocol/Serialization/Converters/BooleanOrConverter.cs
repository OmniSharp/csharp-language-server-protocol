using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null) return;
            WriteJsonGenericMethod.MakeGenericMethod(value.GetType().GetTypeInfo().GenericTypeArguments[0])
                .Invoke(null, new object[] { writer, value, serializer });
        }

        private static void WriteJsonGeneric<T>(JsonWriter writer, BooleanOr<T> value, JsonSerializer serializer)
        {
            if (value.IsValue)
            {
                new JValue(value.Value).WriteTo(writer);
                return;
            }

            if (value.IsBool && value.Bool)
            {
                new JValue(value.Bool).WriteTo(writer);
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

            return new BooleanOr<T>();
        }

        public override bool CanRead => true;

        public override bool CanConvert(Type objectType) => objectType.GetTypeInfo().IsGenericType && objectType.GetTypeInfo().GetGenericTypeDefinition() == typeof(BooleanOr<>);
    }
}