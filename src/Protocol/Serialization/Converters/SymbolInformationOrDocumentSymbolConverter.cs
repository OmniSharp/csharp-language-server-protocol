using System;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    public class SymbolInformationOrDocumentSymbolConverter : JsonConverter<SymbolInformationOrDocumentSymbol>
    {
        public override void Write(Utf8JsonWriter writer, SymbolInformationOrDocumentSymbol value, JsonSerializerOptions options)
        {
            if (value.IsDocumentSymbolInformation)
            {
                  JsonSerializer.Serialize(writer, value.SymbolInformation, options);
            }
            else if (value.IsDocumentSymbol)
            {
                  JsonSerializer.Serialize(writer, value.DocumentSymbol, options);
            }
            else
            {
                writer.WriteNull();
            }
        }

        public override SymbolInformationOrDocumentSymbol Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var result = JObject.Load(reader);

            // SymbolInformation has property location, DocumentSymbol does not.
            if (result["location"] != null)
            {
                return new SymbolInformationOrDocumentSymbol(result.ToObject<SymbolInformation>());
            }
            else
            {
                return new SymbolInformationOrDocumentSymbol(result.ToObject<DocumentSymbol>());
            }
        }


    }
}
