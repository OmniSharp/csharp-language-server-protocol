using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    internal class WorkspaceFolderOrUriConverter : JsonConverter<WorkspaceFolderOrUri>
    {
        public override void WriteJson(JsonWriter writer, WorkspaceFolderOrUri value, JsonSerializer serializer)
        {
            if (value.HasWorkspaceFolder)
            {
                serializer.Serialize(writer, value.WorkspaceFolder);
            }
            else
            {
                serializer.Serialize(writer, value.Uri);
            }
        }

        public override WorkspaceFolderOrUri ReadJson(
            JsonReader reader, Type objectType, WorkspaceFolderOrUri existingValue, bool hasExistingValue, JsonSerializer serializer
        )
        {
            if (reader.TokenType == JsonToken.String)
            {
                return new WorkspaceFolderOrUri(( reader.Value as string )!);
            }

            if (reader.TokenType == JsonToken.StartObject)
            {
                var obj = JObject.Load(reader);
                if (obj.ContainsKey("name"))
                {
                    return new WorkspaceFolderOrUri(obj.ToObject<WorkspaceFolder>());
                }

                return new WorkspaceFolderOrUri(obj.ToObject<DocumentUri>());
            }

            return new WorkspaceFolderOrUri("");
        }

        public override bool CanRead => true;
    }
}
