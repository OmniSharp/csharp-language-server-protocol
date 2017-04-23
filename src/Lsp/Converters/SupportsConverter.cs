using System;
using System.Reflection;
using Lsp.Capabilities.Client;
using Newtonsoft.Json;

namespace Lsp.Converters
{
    class SupportsConverter : JsonConverter
    {
        private static readonly MethodInfo OfValueMethod = typeof(Supports)
            .GetTypeInfo()
            .GetMethod(nameof(Supports.OfValue), BindingFlags.Static | BindingFlags.Public);
        private static readonly MethodInfo OfBooleanMethod = typeof(Supports)
            .GetTypeInfo()
            .GetMethod(nameof(Supports.OfBoolean), BindingFlags.Static | BindingFlags.Public);

        private static readonly PropertyInfo ValueProperty = typeof(Supports<>)
            .GetProperty(nameof(Supports<object>.Value), BindingFlags.Public | BindingFlags.Instance);

        private static readonly PropertyInfo IsSupportedProperty = typeof(Supports<>)
            .GetProperty(nameof(Supports<object>.IsSupported), BindingFlags.Public | BindingFlags.Instance);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var isSupported = value?.GetType()
                ?.GetProperty(nameof(Supports<object>.IsSupported), BindingFlags.Public | BindingFlags.Instance)
                ?.GetValue(value) as bool?;
            if (isSupported == true)
            {
                serializer.Serialize(writer, value.GetType()
                    .GetProperty(nameof(Supports<object>.Value), BindingFlags.Public | BindingFlags.Instance)
                    .GetValue(value));
            }
            else
            {
                serializer.Serialize(writer, false);
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var targetType = objectType.GetGenericArguments()[0];
            if (reader.TokenType == JsonToken.Boolean)
            {
                return OfBooleanMethod
                    .MakeGenericMethod(targetType)
                    .Invoke(null, new [] { reader.Value });
            }

            var target = serializer.Deserialize(reader, targetType);

            return OfValueMethod
                .MakeGenericMethod(targetType)
                .Invoke(null, new[] {target});
        }

        public override bool CanRead => true;

        public override bool CanConvert(Type objectType) => objectType.GetGenericTypeDefinition() == typeof(Supports<>);
    }
}