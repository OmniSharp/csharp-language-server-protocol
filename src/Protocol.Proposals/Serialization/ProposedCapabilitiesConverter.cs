using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization
{
    internal class ProposedCapabilitiesConverter<TFrom, TTo> : JsonConverter<TFrom>
        where TTo : TFrom
        where TFrom : notnull
    {
        public override TFrom? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return JsonSerializer.Deserialize<TTo>(ref reader, options);
        }

        public override void Write(Utf8JsonWriter writer, TFrom value, JsonSerializerOptions options)
        {
            var writeOptions = new JsonSerializerOptions(options);
            for (var i = writeOptions.Converters.Count - 1; i >= 0; i--)
            {
                if (ReferenceEquals(writeOptions.Converters[i], this))
                {
                    writeOptions.Converters.RemoveAt(i);
                }
            }

            JsonSerializer.Serialize(writer, value, value.GetType(), writeOptions);
        }
    }
}
