using System;
using Newtonsoft.Json;
using OmniSharp.Extensions.JsonRpc.Client;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.DebugAdapterConverters
{
    internal class DapClientRequestConverter : JsonConverter<OutgoingRequest>
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
            writer.WritePropertyName("seq");
            writer.WriteValue(value.Id);
            writer.WritePropertyName("type");
            writer.WriteValue("request");
            writer.WritePropertyName("command");
            writer.WriteValue(value.Method);
            if (value.Params != null)
            {
                writer.WritePropertyName("arguments");
                serializer.Serialize(writer, value.Params);
            }

            writer.WriteEndObject();
        }
    }
}
