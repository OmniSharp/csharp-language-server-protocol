using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    internal class ProgressTokenConverter : JsonConverter<ProgressToken?>
    {
        public override void WriteJson(JsonWriter writer, ProgressToken? value, JsonSerializer serializer)
        {
            if (value == null) writer.WriteNull();
            else if (value.IsLong) serializer.Serialize(writer, value.Long);
            else if (value.IsString) serializer.Serialize(writer, value.String);
            else writer.WriteNull();
        }

        public override ProgressToken? ReadJson(JsonReader reader, Type objectType, ProgressToken? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return reader.TokenType switch {
                JsonToken.Integer                                                                   => new ProgressToken((long) reader.Value),
                JsonToken.String when reader.Value is string str && !string.IsNullOrWhiteSpace(str) => new ProgressToken(str),
                _                                                                                   => null
            };
        }

        public override bool CanRead => true;
    }
}
