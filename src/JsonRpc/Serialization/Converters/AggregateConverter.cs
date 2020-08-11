using System;
using System.Collections;
using Newtonsoft.Json;

namespace OmniSharp.Extensions.JsonRpc.Serialization.Converters
{
    public class AggregateConverter<T> : JsonConverter<AggregateResponse<T>> where T : IEnumerable
    {
        public override void WriteJson(JsonWriter writer, AggregateResponse<T> value, JsonSerializer serializer)
        {
            writer.WriteStartArray();
            foreach (var item in value.Items)
            {
                foreach (var v in item)
                {
                    serializer.Serialize(writer, v);
                }
            }

            writer.WriteEndArray();
        }

        public override bool CanRead => false;

        public override AggregateResponse<T> ReadJson(JsonReader reader, Type objectType, AggregateResponse<T> existingValue, bool hasExistingValue, JsonSerializer serializer) =>
            throw new NotImplementedException();
    }
}
