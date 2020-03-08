using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    public class CommandOrCodeActionConverter : JsonConverter<CommandOrCodeAction>
    {
        public override void WriteJson(JsonWriter writer, CommandOrCodeAction value, JsonSerializer serializer)
        {
            if (value.IsCodeAction)
            {
                serializer.Serialize(writer, value.CodeAction);
            }
            else if (value.IsCommand)
            {
                serializer.Serialize(writer, value.Command);
            }
            else
            {
                writer.WriteNull();
            }
        }

        public override CommandOrCodeAction ReadJson(JsonReader reader, Type objectType, CommandOrCodeAction existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var result = JObject.Load(reader);

            // Commands have a name, CodeActions do not
            JToken command = result["command"];
            if (command?.Type == JTokenType.String)
            {
                return new CommandOrCodeAction(result.ToObject<Command>());
            }
            else
            {
                return new CommandOrCodeAction(result.ToObject<CodeAction>());
            }
        }

        public override bool CanRead => true;
    }
}
