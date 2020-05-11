using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    abstract class ConstrainedConverter<T> : JsonConverter<T>
        where T : new()
    {
        private readonly IDictionary<string, (string name, PropertyInfo propertyInfo, bool optional)> _properties =
            typeof(T)
                .GetProperties()
                .ToDictionary(
                    x => x.Name[0].ToString().ToLower() + x.Name.Substring(1),
                    x => (x.Name[0].ToString().ToLower() + x.Name.Substring(1), x,
                        CustomAttributeExtensions.GetCustomAttributes<JsonIgnoreAttribute>((MemberInfo) x)
                            .Any(z => z.Condition == JsonIgnoreCondition.WhenNull)),
                    StringComparer.OrdinalIgnoreCase
                );
        protected virtual void ProcessProperty(string name, PropertyInfo propertyInfo, ref object value)
        {
        }

        protected TValue ValidValue<TValue>(TValue[] validValues, object value)
        {
            if (value is TValue v)
                return validValues.Any(z => z.Equals(value)) ? v : validValues[0];
            return validValues[0];
        }

        protected IEnumerable<TValue> ValidValues<TValue>(TValue[] validValues, object value)
        {
            if (value is IEnumerable<TValue> values)
                return  values.Join(validValues, z => z, z => z, (a, b) => a).AsEnumerable();
            return Enumerable.Empty<TValue>();
        }

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert,
            JsonSerializerOptions options)
        {
            var value = new T();
            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException("Expected an object");
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    var propertyName = reader.GetString();
                    reader.Read();
                    var (_, property, optional) = _properties[propertyName];
                    var propertyValue = JsonSerializer.Deserialize(ref reader, property.PropertyType, options);
                    ProcessProperty(propertyName, property, ref propertyValue);
                    property.SetValue(value, propertyValue);
                }
            }

            reader.Read();
            return value;
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            foreach (var (name, property, optional) in _properties.Values)
            {
                var propertyValue = property.GetValue(value);
                if (optional && propertyValue == null) continue;
                writer.WritePropertyName(name);
                ProcessProperty(name, property, ref propertyValue);
                JsonSerializer.Serialize(writer, propertyValue, options);
            }
        }
    }
}
