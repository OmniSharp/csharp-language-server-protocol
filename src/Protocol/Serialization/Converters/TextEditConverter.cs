using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

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
                writer.WritePropertyName("annotation");
                serializer.Serialize(writer, annotatedTextEdit.Annotation);
            }

            writer.WriteEndObject();
        }

        public override TextEdit ReadJson(JsonReader reader, Type objectType, TextEdit existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var result = JObject.Load(reader);
            var isAnnotated = result.ContainsKey("annotation");
            TextEdit edit;
            if (result["annotation"] is { Type: JTokenType.Object } annotation)
            {
                edit = new AnnotatedTextEdit() {
                    Annotation = annotation.ToObject<ChangeAnnotation>()
                };
            }
            else
            {
                edit = new TextEdit();
            }

            if (result["range"] is { Type: JTokenType.Object } range)
            {
                edit = edit with { Range = range.ToObject<Range>()};
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
