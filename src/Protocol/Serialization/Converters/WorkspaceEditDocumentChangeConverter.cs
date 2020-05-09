using System;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    public class WorkspaceEditDocumentChangeConverter : JsonConverter<WorkspaceEditDocumentChange>
    {
        public override void Write(Utf8JsonWriter writer, WorkspaceEditDocumentChange value, JsonSerializerOptions options)
        {
            if (value.IsCreateFile)   JsonSerializer.Serialize(writer, value.CreateFile, options);
            if (value.IsDeleteFile)   JsonSerializer.Serialize(writer, value.DeleteFile, options);
            if (value.IsRenameFile)   JsonSerializer.Serialize(writer, value.RenameFile, options);
            if (value.IsTextDocumentEdit)   JsonSerializer.Serialize(writer, value.TextDocumentEdit, options);
        }

        public override WorkspaceEditDocumentChange Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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


    }

    class ValueTupleContractResolver<T1, T2> : JsonConverter<ValueTuple<T1, T2>>
    {
        public override void Write(Utf8JsonWriter writer, (T1, T2) value, JsonSerializerOptions options)
        {
              JsonSerializer.Serialize(writer, new object[] { value.Item1, value.Item2 }, options);
        }

        public override (T1, T2) Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var a = JArray.Load(reader);
            return (a.ToObject<T1>(), a.ToObject<T2>());
        }
    }
}
