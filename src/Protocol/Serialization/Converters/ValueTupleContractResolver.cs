using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    class ValueTupleContractResolver<T1, T2> : JsonConverter<(T1, T2)>
    {
        public override void Write(Utf8JsonWriter writer, (T1, T2) value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, new object[] {value.Item1, value.Item2}, options);
        }

        public override (T1, T2) Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            reader.Read();
            var value1 = JsonSerializer.Deserialize<T1>(ref reader, options);
            var value2 = JsonSerializer.Deserialize<T2>(ref reader, options);
            reader.Read();
            return (value1, value2);
        }
    }
}
