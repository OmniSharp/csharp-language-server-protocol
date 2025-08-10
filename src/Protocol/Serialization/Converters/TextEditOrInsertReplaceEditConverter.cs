using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    public class TextEditOrInsertReplaceEditConverter : JsonConverter<TextEditOrInsertReplaceEdit>
    {
        public override void WriteJson(JsonWriter writer, TextEditOrInsertReplaceEdit value, JsonSerializer serializer)
        {
            if (value.IsTextEdit)
            {
                serializer.Serialize(writer, value.TextEdit);
            }
            else if (value.IsInsertReplaceEdit)
            {
                serializer.Serialize(writer, value.InsertReplaceEdit);
            }
            else
            {
                writer.WriteNull();
            }
        }

        public override TextEditOrInsertReplaceEdit ReadJson(JsonReader reader, Type objectType, TextEditOrInsertReplaceEdit existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var result = JObject.Load(reader);

            // InsertReplaceEdit have a name, TextEdits do not
            var command = result["insert"];
            if (command?.Type == JTokenType.String)
            {
                return new TextEditOrInsertReplaceEdit(result.ToObject<InsertReplaceEdit>(serializer));
            }

            return new TextEditOrInsertReplaceEdit(result.ToObject<TextEdit>(serializer));
        }

        public override bool CanRead => true;
    }
}
