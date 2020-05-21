using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    public class WorkspaceEditDocumentChangeConverter : JsonConverter<WorkspaceEditDocumentChange>
    {
        public override void WriteJson(JsonWriter writer, WorkspaceEditDocumentChange value, JsonSerializer serializer)
        {
            if (value.IsCreateFile) serializer.Serialize(writer, value.CreateFile);
            if (value.IsDeleteFile) serializer.Serialize(writer, value.DeleteFile);
            if (value.IsRenameFile) serializer.Serialize(writer, value.RenameFile);
            if (value.IsTextDocumentEdit) serializer.Serialize(writer, value.TextDocumentEdit);
        }

        public override WorkspaceEditDocumentChange ReadJson(JsonReader reader, Type objectType, WorkspaceEditDocumentChange existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);
            if (obj.ContainsKey("kind"))
            {
                var kind = obj["kind"].ToString();
                switch (kind)
                {
                    case "create":
                        return new WorkspaceEditDocumentChange(obj.ToObject<CreateFile>());
                    case "rename":
                        return new WorkspaceEditDocumentChange(obj.ToObject<RenameFile>());
                    case "delete":
                        return new WorkspaceEditDocumentChange(obj.ToObject<DeleteFile>());
                    default:
                        throw new NotSupportedException("Object with " + kind + " is not supported");
                }
            }
            else
            {
                return new WorkspaceEditDocumentChange(obj.ToObject<TextDocumentEdit>());
            }
        }

        public override bool CanRead => true;
    }

    class ValueTupleContractResolver<T1, T2> : JsonConverter<ValueTuple<T1, T2>>
    {
        public override void WriteJson(JsonWriter writer, (T1, T2) value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, new object[] { value.Item1, value.Item2 });
        }

        public override (T1, T2) ReadJson(JsonReader reader, Type objectType, (T1, T2) existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var a = JArray.Load(reader);
            return (a.ToObject<T1>(), a.ToObject<T2>());
        }
    }
}
