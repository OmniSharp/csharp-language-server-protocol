using System;
using System.Collections.Generic;
using System.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    class MarkedStringCollectionConverter : JsonConverter<Container<MarkedString>>
    {
        public override void Write(Utf8JsonWriter writer, Container<MarkedString> value, JsonSerializerOptions options)
        {
            var v = value.ToArray();
            if (v.Length == 1)
            {
                  JsonSerializer.Serialize(writer, v[0], options);
                return;
            }

              JsonSerializer.Serialize(writer, v, options);
        }

        public override Container<MarkedString> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonToken.StartArray)
            {
                return new Container<MarkedString>(JArray.Load(reader).ToObject<IEnumerable<MarkedString>>(serializer));
            }
            else if (reader.TokenType == JsonToken.StartObject)
            {
                return new Container<MarkedString>(JObject.Load(reader).ToObject<MarkedString>(serializer));
            }
            else if (reader.TokenType == JsonToken.String)
            {
                return new Container<MarkedString>(reader.Value as string);
            }

            return new Container<MarkedString>();
        }


    }
}
