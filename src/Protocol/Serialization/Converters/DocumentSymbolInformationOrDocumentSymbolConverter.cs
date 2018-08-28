using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    public class DocumentSymbolInformationOrDocumentSymbolConverter : JsonConverter<DocumentSymbolInformationOrDocumentSymbol>
    {
        public override void WriteJson(JsonWriter writer, DocumentSymbolInformationOrDocumentSymbol value, JsonSerializer serializer)
        {
            if (value.IsDocumentSymbolInformation)
            {
                serializer.Serialize(writer, value.DocumentSymbolInformation);
            }
            else if (value.IsDocumentSymbol)
            {
                serializer.Serialize(writer, value.DocumentSymbol);
            }
            else
            {
                writer.WriteNull();
            }
        }

        public override DocumentSymbolInformationOrDocumentSymbol ReadJson(JsonReader reader, Type objectType, DocumentSymbolInformationOrDocumentSymbol existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var result = JObject.Load(reader);

            // Commands have a name, CodeActions do not
            if (result["location"].Type == JTokenType.Object)
            {
                return new DocumentSymbolInformationOrDocumentSymbol(result.ToObject<DocumentSymbolInformation>());
            }
            else
            {
                return new DocumentSymbolInformationOrDocumentSymbol(result.ToObject<DocumentSymbol>());
            }
        }

        public override bool CanRead => true;
    }
    //DocumentSymbolInformationOrDocumentSymbolConverter
}
