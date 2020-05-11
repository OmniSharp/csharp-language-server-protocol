using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [JsonConverter(typeof(Converter))]
    public struct WorkspaceEditDocumentChange
    {
        public WorkspaceEditDocumentChange(TextDocumentEdit textDocumentEdit)
        {
            TextDocumentEdit = textDocumentEdit;
            CreateFile = null;
            RenameFile = null;
            DeleteFile = null;
        }

        public WorkspaceEditDocumentChange(CreateFile createFile)
        {
            TextDocumentEdit = null;
            CreateFile = createFile;
            RenameFile = null;
            DeleteFile = null;
        }

        public WorkspaceEditDocumentChange(RenameFile renameFile)
        {
            TextDocumentEdit = null;
            CreateFile = null;
            RenameFile = renameFile;
            DeleteFile = null;
        }

        public WorkspaceEditDocumentChange(DeleteFile deleteFile)
        {
            TextDocumentEdit = null;
            CreateFile = null;
            RenameFile = null;
            DeleteFile = deleteFile;
        }

        public bool IsTextDocumentEdit => TextDocumentEdit != null;
        public TextDocumentEdit TextDocumentEdit { get; }

        public bool IsCreateFile => CreateFile != null;
        public CreateFile CreateFile { get; }

        public bool IsRenameFile => RenameFile != null;
        public RenameFile RenameFile { get; }

        public bool IsDeleteFile => DeleteFile != null;
        public DeleteFile DeleteFile { get; }

        public static implicit operator WorkspaceEditDocumentChange(TextDocumentEdit textDocumentEdit)
        {
            return new WorkspaceEditDocumentChange(textDocumentEdit);
        }

        public static implicit operator WorkspaceEditDocumentChange(CreateFile createFile)
        {
            return new WorkspaceEditDocumentChange(createFile);
        }

        public static implicit operator WorkspaceEditDocumentChange(RenameFile renameFile)
        {
            return new WorkspaceEditDocumentChange(renameFile);
        }

        public static implicit operator WorkspaceEditDocumentChange(DeleteFile deleteFile)
        {
            return new WorkspaceEditDocumentChange(deleteFile);
        }

        class Converter : JsonConverter<WorkspaceEditDocumentChange>
        {
            public override void Write(Utf8JsonWriter writer, WorkspaceEditDocumentChange value,
                JsonSerializerOptions options)
            {
                if (value.IsCreateFile) JsonSerializer.Serialize(writer, value.CreateFile, options);
                if (value.IsDeleteFile) JsonSerializer.Serialize(writer, value.DeleteFile, options);
                if (value.IsRenameFile) JsonSerializer.Serialize(writer, value.RenameFile, options);
                if (value.IsTextDocumentEdit) JsonSerializer.Serialize(writer, value.TextDocumentEdit, options);
            }

            public override WorkspaceEditDocumentChange Read(ref Utf8JsonReader reader, Type typeToConvert,
                JsonSerializerOptions jsonOptions)
            {
                ResourceOperationKind? kind = default;
                DocumentUri uri = null;
                DocumentUri oldUri = null;
                DocumentUri newUri = null;
                VersionedTextDocumentIdentifier textDocument = null;
                TextEditContainer edits = null;
                var overwrite = false;
                var ignoreIfExists = false;
                var recursive = false;
                var ignoreIfNotExists = false;
                string propertyName = null;
                var optionsOpen = false;

                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndObject)
                    {
                        if (optionsOpen)
                        {
                            optionsOpen = false;
                            continue;
                        }

                        reader.Read();
                        break;
                    }

                    if (reader.TokenType == JsonTokenType.PropertyName)
                    {
                        propertyName = reader.GetString();
                        continue;
                    }

                    switch (propertyName)
                    {
                        case nameof(uri):
                            uri = reader.GetString();
                            break;
                        case nameof(oldUri):
                            oldUri = reader.GetString();
                            break;
                        case nameof(newUri):
                            newUri = reader.GetString();
                            break;
                        case nameof(overwrite):
                            overwrite = reader.GetBoolean();
                            break;
                        case nameof(ignoreIfExists):
                            ignoreIfExists = reader.GetBoolean();
                            break;
                        case nameof(recursive):
                            recursive = reader.GetBoolean();
                            break;
                        case nameof(ignoreIfNotExists):
                            ignoreIfNotExists = reader.GetBoolean();
                            break;
                        case nameof(kind):
                            kind = JsonSerializer.Deserialize<ResourceOperationKind>(ref reader, jsonOptions);
                            break;
                        case nameof(textDocument):
                            textDocument =
                                JsonSerializer.Deserialize<VersionedTextDocumentIdentifier>(ref reader, jsonOptions);
                            break;
                        case nameof(edits):
                            edits = JsonSerializer.Deserialize<TextEditContainer>(ref reader, jsonOptions);
                            break;
                        case "options":
                            reader.Read();
                            optionsOpen = true;
                            // edits = JsonSerializer.Deserialize<TextEditContainer>(ref reader, jsonOptions);
                            break;
                        default:
                            throw new JsonException($"Unsupported property found {propertyName}");
                    }
                }

                return
                    kind switch {
                        ResourceOperationKind.Create => new CreateFile() {
                            Options = new CreateFileOptions() {
                                Overwrite = overwrite,
                                IgnoreIfExists = ignoreIfExists
                            },
                            Uri = uri,
                        },
                        ResourceOperationKind.Rename => new RenameFile() {
                            Options = new RenameFileOptions() {
                                Overwrite = overwrite,
                                IgnoreIfExists = ignoreIfExists
                            },
                            NewUri = newUri,
                            OldUri = oldUri
                        },
                        ResourceOperationKind.Delete => new DeleteFile() {
                            Options = new DeleteFileOptions() {
                                Recursive = recursive,
                                IgnoreIfNotExists = ignoreIfNotExists
                            },
                            Uri = uri
                        },
                        null => new TextDocumentEdit() {
                            Edits = edits,
                            TextDocument = textDocument,
                        },
                        _ => throw new JsonException("Object with " + kind + " is not supported")
                    };
            }
        }
    }
}
