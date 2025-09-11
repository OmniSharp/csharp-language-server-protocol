using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    internal class TextDocumentSyncConverter : JsonConverter<TextDocumentSync?>
    {
        public override void WriteJson(JsonWriter writer, TextDocumentSync? value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteValue(TextDocumentSyncKind.None);
                return;
            }
            if (value.HasOptions)
            {
                serializer.Serialize(writer, value.Options);
            }
            else if (value.HasKind)
            {
                new JValue(value.Value).WriteTo(writer);
            }
        }

        public override TextDocumentSync ReadJson(JsonReader reader, Type objectType, TextDocumentSync? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.Integer:
                    return new TextDocumentSync((TextDocumentSyncKind) Convert.ToInt32(reader.Value));
                case JsonToken.Null:
                case JsonToken.Undefined:
                    return new TextDocumentSync(TextDocumentSyncKind.None);
                default:
                    return new TextDocumentSync(JObject.Load(reader).ToObject<TextDocumentSyncOptions>(serializer));
            }
        }

        public override bool CanRead => true;
    }
}
