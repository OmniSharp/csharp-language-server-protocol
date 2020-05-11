using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    public class RangeValueConverter<T> : JsonConverter<T>
        where T : struct
    {
        private readonly JsonConverter<T> _converter;
        private readonly T[] _validValues;
        private readonly T _defaultValue;

        public RangeValueConverter(JsonConverter<T> converter, T[] validValues)
        {
            _converter = converter;
            _validValues = validValues;
            _defaultValue = validValues[0];
        }

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = _converter.Read(ref reader, typeToConvert, options);
            return _validValues.Any(z => z.Equals(value)) ? value : _defaultValue;
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            _converter.Write(writer, value, options);
        }
    }
}
