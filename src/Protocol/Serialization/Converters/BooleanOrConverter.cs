using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    internal class BooleanOrConverter : JsonConverter
    {
        private static readonly MethodInfo WriteJsonGenericMethod = typeof(BooleanOrConverter)
                                                                   .GetTypeInfo()
                                                                   .GetMethod(nameof(WriteJsonGeneric), BindingFlags.NonPublic | BindingFlags.Static)!;

        private static readonly MethodInfo ReadJsonGenericMethod = typeof(BooleanOrConverter)
                                                                  .GetTypeInfo()
                                                                  .GetMethod(nameof(ReadJsonGeneric), BindingFlags.NonPublic | BindingFlags.Static)!;

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value == null) return;
            WriteJsonGenericMethod.MakeGenericMethod(value.GetType().GetTypeInfo().GenericTypeArguments[0])
                                  .Invoke(null, new[] { writer, value, serializer });
        }

        private static void WriteJsonGeneric<T>(JsonWriter writer, BooleanOr<T> value, JsonSerializer serializer)
            where T : class?
        {
            if (value.IsBool)
            {
                new JValue(value.Bool).WriteTo(writer);
                return;
            }

            if (value.IsValue)
            {
                serializer.Serialize(writer, value.Value);
                return;
            }

            writer.WriteNull();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var parentType = objectType.GetTypeInfo().GenericTypeArguments[0];
            return ReadJsonGenericMethod.MakeGenericMethod(parentType)
                                        .Invoke(null, new object[] { reader, serializer });
        }

        private static BooleanOr<T> ReadJsonGeneric<T>(JsonReader reader, JsonSerializer serializer)
            where T : class?
        {
            if (reader.TokenType == JsonToken.Boolean)
            {
                return new BooleanOr<T>((bool) reader.Value);
            }

            if (reader.TokenType == JsonToken.StartObject)
            {
                return new BooleanOr<T>(JObject.Load(reader).ToObject<T>(serializer));
            }

            return new BooleanOr<T>(false);
        }

        public override bool CanRead => true;

        public override bool CanConvert(Type objectType) => objectType.GetTypeInfo().IsGenericType && objectType.GetTypeInfo().GetGenericTypeDefinition() == typeof(BooleanOr<>);
    }
}
