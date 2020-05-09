using System;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    public class ParameterInformationLabelConverter : JsonConverter<ParameterInformationLabel>
    {
        public override void Write(Utf8JsonWriter writer, ParameterInformationLabel value, JsonSerializerOptions options)
        {
            if (value.IsLabel)
                  JsonSerializer.Serialize(writer, value.Label, options);
            if (value.IsRange)
                  JsonSerializer.Serialize(writer, new [] { value.Range.start, value.Range.end }, options);
        }

        public override ParameterInformationLabel Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonToken.String)
            {
                return new ParameterInformationLabel((string)reader.Value);
            }
            if (reader.TokenType == JsonToken.StartArray)
            {
                var a = JArray.Load(reader);
                return new ParameterInformationLabel((a[0].ToObject<int>(), a[1].ToObject<int>()));
            }
            throw new NotSupportedException();
        }


    }
}
