using System;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    class TextDocumentSyncConverter : JsonConverter<TextDocumentSync>
    {
        public override void Write(Utf8JsonWriter writer, TextDocumentSync value, JsonSerializerOptions options)
        {
            if (value.HasOptions)
            {
                  JsonSerializer.Serialize(writer, value.options);
            }
            else if (value.HasKind)
            {
                new JValue(value.Value).WriteTo(writer);
            }
        }

        public override TextDocumentSync Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonToken.Integer)
            {
                return new TextDocumentSync((TextDocumentSyncKind)Convert.ToInt32(reader.Value));
            }

            return new TextDocumentSync(JObject.Load(reader).ToObject<TextDocumentSyncOptions>(serializer));
        }


    }
}
