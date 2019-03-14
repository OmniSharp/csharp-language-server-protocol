using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    public class SymbolInformationOrDocumentSymbolConverter : JsonConverter<SymbolInformationOrDocumentSymbol>
    {
        public override void WriteJson(JsonWriter writer, SymbolInformationOrDocumentSymbol value, JsonSerializer serializer)
        {
            if (value.IsDocumentSymbolInformation)
            {
                serializer.Serialize(writer, value.SymbolInformation);
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

        public override SymbolInformationOrDocumentSymbol ReadJson(JsonReader reader, Type objectType, SymbolInformationOrDocumentSymbol existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var result = JObject.Load(reader);

            // Commands have a name, CodeActions do not
            if (result["location"].Type == JTokenType.Object)
            {
                return new SymbolInformationOrDocumentSymbol(result.ToObject<SymbolInformation>());
            }
            else
            {
                return new SymbolInformationOrDocumentSymbol(result.ToObject<DocumentSymbol>());
            }
        }

        public override bool CanRead => true;
    }
}
