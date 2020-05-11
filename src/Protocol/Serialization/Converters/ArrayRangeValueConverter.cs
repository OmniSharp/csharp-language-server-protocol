using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    public class ArrayRangeValueConverter<T> : JsonConverter<T>
    {
        private readonly JsonConverter<T> _converter;
        private readonly T[] _validValues;
        private readonly T _defaultValue;

        public ArrayRangeValueConverter(JsonConverter<T> converter, T[] validValues)
        {
            _converter = converter;
            _validValues = validValues;
            _defaultValue = validValues[0];
        }

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = _converter.Read(ref reader, typeToConvert, options);

            if (value is Container<T> container)
            {
                return (T) container.Join(_validValues, z => z, z => z, (a, b) => a);
            }

            if (value is IEnumerable<T> values)
            {
                return (T) values.Join(_validValues, z => z, z => z, (a, b) => a).AsEnumerable();
            }

            return value;
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            _converter.Write(writer, value, options);
        }
    }
}
