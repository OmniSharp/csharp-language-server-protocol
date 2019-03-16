using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    public class ParameterInformationLabelConverter : JsonConverter<ParameterInformationLabel>
    {
        public override void WriteJson(JsonWriter writer, ParameterInformationLabel value, JsonSerializer serializer)
        {
            if (value.IsLabel)
                serializer.Serialize(writer, value.Label);
            if (value.IsRange)
                serializer.Serialize(writer, value.Range);
        }

        public override ParameterInformationLabel ReadJson(JsonReader reader, Type objectType, ParameterInformationLabel existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                return new ParameterInformationLabel((string)reader.Value);
            }
            if (reader.TokenType == JsonToken.StartArray)
            {
                var a = JArray.Load(reader);
                return new ParameterInformationLabel((a[0].ToObject<long>(), a[1].ToObject<long>()));
            }
            throw new NotSupportedException();
        }

        public override bool CanRead => true;
    }
}
