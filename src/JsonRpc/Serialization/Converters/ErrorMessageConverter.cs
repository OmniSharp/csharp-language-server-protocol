using System;
using Newtonsoft.Json;
using OmniSharp.Extensions.JsonRpc.Server.Messages;

namespace OmniSharp.Extensions.JsonRpc.Serialization.Converters
{
    public class ErrorMessageConverter : JsonConverter<ErrorMessage>
    {
        public override void WriteJson(JsonWriter writer, ErrorMessage value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("code");
            writer.WriteValue(value.Code);
            if (value.Data != null)
            {
                writer.WritePropertyName("data");
                writer.WriteValue(value.Data);
            }
            writer.WritePropertyName("message");
            writer.WriteValue(value.Message);
            writer.WriteEndObject();
        }

        public override ErrorMessage ReadJson(JsonReader reader, Type objectType, ErrorMessage existingValue, bool hasExistingValue, JsonSerializer serializer) => throw new NotImplementedException();

        public override bool CanRead => false;
    }
}
