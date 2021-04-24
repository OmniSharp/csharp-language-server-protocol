using System;
using Newtonsoft.Json;
using OmniSharp.Extensions.JsonRpc.Client;

namespace OmniSharp.Extensions.JsonRpc.Serialization.Converters
{
    public class ClientRequestConverter : JsonConverter<OutgoingRequest>
    {
        public override bool CanRead => false;

        public override OutgoingRequest ReadJson(
            JsonReader reader, Type objectType, OutgoingRequest existingValue,
            bool hasExistingValue, JsonSerializer serializer
        ) =>
            throw new NotImplementedException();

        public override void WriteJson(JsonWriter writer, OutgoingRequest value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("jsonrpc");
            writer.WriteValue("2.0");
            writer.WritePropertyName("id");
            writer.WriteValue(value.Id);
            writer.WritePropertyName("method");
            writer.WriteValue(value.Method);
            if (value.Params != null)
            {
                writer.WritePropertyName("params");
                serializer.Serialize(writer, value.Params);
            }
            if (value.TraceParent != null)
            {
                writer.WritePropertyName("traceparent");
                writer.WriteValue(value.TraceParent);
                if (!string.IsNullOrWhiteSpace(value.TraceState))
                {
                    writer.WritePropertyName("tracestate");
                    writer.WriteValue(value.TraceState);
                }
            }

            writer.WriteEndObject();
        }
    }
}
