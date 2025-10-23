using Newtonsoft.Json;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Client;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.DebugAdapterConverters
{
    internal class DapClientResponseConverter : JsonConverter<OutgoingResponse>
    {
        private readonly ISerializer _serializer;

        public DapClientResponseConverter(ISerializer serializer) => _serializer = serializer;

        public override bool CanRead => false;

        public override OutgoingResponse ReadJson(
            JsonReader reader, Type objectType, OutgoingResponse existingValue,
            bool hasExistingValue, JsonSerializer serializer
        ) =>
            throw new NotImplementedException();

        public override void WriteJson(JsonWriter writer, OutgoingResponse value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("seq");
            writer.WriteValue(_serializer.GetNextId());
            writer.WritePropertyName("type");
            writer.WriteValue("response");
            writer.WritePropertyName("request_seq");
            writer.WriteValue(value.Id);
            // TODO: Dynamically set this based on handler execution.
            writer.WritePropertyName("success");
            writer.WriteValue(true);
            writer.WritePropertyName("command");
            writer.WriteValue(value.Request.Method);
            if (value.Result != null)
            {
                writer.WritePropertyName("body");
                serializer.Serialize(writer, value.Result);
            }

            writer.WriteEndObject();
        }
    }
}
