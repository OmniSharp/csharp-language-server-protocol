using System;
using System.Linq;
using Newtonsoft.Json;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    public class AggregateCompletionListConverter : JsonConverter<AggregateResponse<CompletionList>>
    {
        public override void WriteJson(JsonWriter writer, AggregateResponse<CompletionList> value, JsonSerializer serializer)
        {
            var values = value.Items.ToArray();

            if (!values.Any(z => z.IsIncomplete))
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
                return;
            }

            writer.WriteStartObject();
            writer.WritePropertyName("isIncomplete");
            writer.WriteValue(true);

            writer.WritePropertyName("items");
            writer.WriteStartArray();
            foreach (var item in value.Items)
            {
                foreach (var v in item)
                {
                    serializer.Serialize(writer, v);
                }
            }
            writer.WriteEndArray();
            writer.WriteEndObject();
        }

        public override bool CanRead => false;

        public override AggregateResponse<CompletionList> ReadJson(JsonReader reader, Type objectType, AggregateResponse<CompletionList> existingValue, bool hasExistingValue, JsonSerializer serializer) => throw new NotImplementedException();
    }
}
