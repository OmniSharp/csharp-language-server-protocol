using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;


namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    public class TextEditConverter : JsonConverter<TextEdit>
    {
        public override void WriteJson(JsonWriter writer, TextEdit value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("range");
            serializer.Serialize(writer, value.Range);
            writer.WritePropertyName("newText");
            serializer.Serialize(writer, value.NewText);
            if (value is AnnotatedTextEdit annotatedTextEdit)
            {
                writer.WritePropertyName("annotationId");
                serializer.Serialize(writer, annotatedTextEdit.AnnotationId);
            }

            writer.WriteEndObject();
        }

        public override TextEdit ReadJson(JsonReader reader, Type objectType, TextEdit existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var result = JObject.Load(reader);
            TextEdit edit;
            if (result["annotationId"] is { Type: JTokenType.String } annotation)
            {
                edit = new AnnotatedTextEdit() {
                    AnnotationId = annotation.ToObject<ChangeAnnotationIdentifier>(serializer)
                };
            }
            else
            {
                edit = new TextEdit();
            }

            if (result["range"] is { Type: JTokenType.Object } range)
            {
                edit = edit with { Range = range.ToObject<Range>(serializer)};
            }

            if (result["newText"] is { Type: JTokenType.String } newText)
            {
                edit = edit with { NewText = newText.Value<string>()};
            }

            return edit;
        }

        public override bool CanRead => true;
    }
}
