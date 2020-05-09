using System;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    public class CommandOrCodeActionConverter : JsonConverter<CommandOrCodeAction>
    {
        public override void Write(Utf8JsonWriter writer, CommandOrCodeAction value, JsonSerializerOptions options)
        {
            if (value.IsCodeAction)
            {
                  JsonSerializer.Serialize(writer, value.CodeAction, options);
            }
            else if (value.IsCommand)
            {
                  JsonSerializer.Serialize(writer, value.Command, options);
            }
            else
            {
                writer.WriteNull();
            }
        }

        public override CommandOrCodeAction Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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


    }
}
