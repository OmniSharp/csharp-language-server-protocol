using System.Collections.Immutable;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    namespace Models
    {
        [Parallel]
        [Method(TextDocumentNames.Diagnostics, Direction.ClientToServer)]
        [GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Document")]
        [GenerateHandlerMethods]
        [GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
        [RegistrationOptions(typeof(DiagnosticsRegistrationOptions))]
        [Capability(typeof(DiagnosticClientCapabilities))]
        public partial record DocumentDiagnosticParams : ITextDocumentIdentifierParams, IWorkDoneProgressParams,
                                                         IPartialItemWithInitialValueRequest<RelatedDocumentDiagnosticReport,
                                                             DocumentDiagnosticReportPartialResult>
        {
            /// <summary>
            /// The text document.
            /// </summary>
            public TextDocumentIdentifier TextDocument { get; init; } = null!;

            /// <summary>
            /// The additional identifier  provided during registration.
            /// </summary>
            [Optional]
            public string? Identifier { get; init; }

            /// <summary>
            /// The result id of a previous response if provided.
            /// </summary>
            [Optional]
            public string? PreviousResultId { get; init; }
        }

        [Parallel]
        [Method(WorkspaceNames.Diagnostics, Direction.ClientToServer)]
        [GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Workspace")]
        [GenerateHandlerMethods]
        [GenerateRequestMethods(typeof(IWorkspaceLanguageClient), typeof(ILanguageClient))]
        [RegistrationOptions(typeof(DiagnosticsRegistrationOptions))]
        [Capability(typeof(DiagnosticWorkspaceClientCapabilities))]
        public partial record WorkspaceDiagnosticParams : IWorkDoneProgressParams,
                                                          IPartialItemWithInitialValueRequest<WorkspaceDiagnosticReport, WorkspaceDiagnosticReportPartialResult>
        {
            /// <summary>
            /// The additional identifier  provided during registration.
            /// </summary>
            [Optional]
            public string? Identifier { get; init; }

            /// <summary>
            /// The result id of a previous response if provided.
            /// </summary>
            public Container<PreviousResultId> PreviousResultIds { get; init; }
        }


        [Parallel]
        [Method(WorkspaceNames.DiagnosticRefresh, Direction.ServerToClient)]
        [GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Workspace")]
        [GenerateHandlerMethods]
        [GenerateRequestMethods(typeof(IWorkspaceLanguageServer), typeof(ILanguageServer))]
        [Capability(typeof(DiagnosticWorkspaceClientCapabilities))]
        public partial record DiagnosticRefreshParams : IRequest<Unit>;

        public interface IDiagnosticReport
        {
            /// <summary>
            /// A full document diagnostic report.
            /// </summary>
            DocumentDiagnosticReportKind Kind { get; }
        }

        /// <summary>
        /// The result of a document diagnostic pull request. A report can
        /// either be a full report containing all diagnostics for the
        /// requested document or a unchanged report indicating that nothing
        /// has changed in terms of diagnostics in comparison to the last
        /// pull request.
        ///
        /// @since 3.17.0
        /// </summary>
        [JsonConverter(typeof(Converter))]
        public abstract partial record RelatedDocumentDiagnosticReport(DocumentDiagnosticReportKind Kind) : DocumentDiagnosticReportPartialResult,
            IDiagnosticReport
        {
            /// <summary>
            /// The report kind
            /// </summary>
            public DocumentDiagnosticReportKind Kind { get; } = Kind;

            public static RelatedDocumentDiagnosticReport From(RelatedDocumentDiagnosticReport original, DocumentDiagnosticReportPartialResult? result)
            {
                var docs = original.RelatedDocuments?.ToBuilder() ?? ImmutableDictionary<DocumentUri, DocumentDiagnosticReport>.Empty.ToBuilder();
                foreach (var item in result?.RelatedDocuments ?? ImmutableDictionary<DocumentUri, DocumentDiagnosticReport>.Empty)
                {
                    if (docs.ContainsKey(item.Key))
                    {
                        docs[item.Key] = item.Value;
                    }
                    else
                    {
                        docs.Add(item);
                    }
                }

                return original with { RelatedDocuments = docs.ToImmutable() };
            }

            internal class Converter : JsonConverter<RelatedDocumentDiagnosticReport>
            {
                public override void WriteJson(JsonWriter writer, RelatedDocumentDiagnosticReport? value, JsonSerializer serializer)
                {
                    WorkspaceDocumentDiagnosticReport.DiagnosticReportConverter.WriteRelatedDocumentDiagnosticReport(writer, value, serializer);
                }

                public override RelatedDocumentDiagnosticReport ReadJson(
                    JsonReader reader, Type objectType, RelatedDocumentDiagnosticReport? existingValue, bool hasExistingValue, JsonSerializer serializer
                )
                {
                    if (reader.TokenType == JsonToken.Null)
                    {
                        return null!;
                    }

                    return WorkspaceDocumentDiagnosticReport.DiagnosticReportConverter.ReadRelatedDocumentDiagnosticReport(JObject.Load(reader), serializer);
                }
            }
        }

        [JsonConverter(typeof(Converter))]
        public abstract partial record DocumentDiagnosticReport(DocumentDiagnosticReportKind Kind) : IDiagnosticReport
        {
            /// <summary>
            /// The report kind
            /// </summary>
            public DocumentDiagnosticReportKind Kind { get; } = Kind;

            internal class Converter : JsonConverter<DocumentDiagnosticReport>
            {
                public override void WriteJson(JsonWriter writer, DocumentDiagnosticReport value, JsonSerializer serializer)
                {
                    WorkspaceDocumentDiagnosticReport.DiagnosticReportConverter.WriteDocumentDiagnosticReport(writer, value, serializer);
                }

                public override DocumentDiagnosticReport ReadJson(
                    JsonReader reader, Type objectType, DocumentDiagnosticReport existingValue, bool hasExistingValue, JsonSerializer serializer
                )
                {
                    if (reader.TokenType == JsonToken.Null)
                    {
                        return null!;
                    }

                    return WorkspaceDocumentDiagnosticReport.DiagnosticReportConverter.ReadDocumentDiagnosticReport(JObject.Load(reader), serializer);
                }
            }
        }

        /// <summary>
        /// The document diagnostic report kinds.
        ///
        /// @since 3.17.0
        /// </summary>
        [StringEnum]
        public readonly partial struct DocumentDiagnosticReportKind
        {
            /// <summary>
            /// A diagnostic report with a full
            /// set of problems.
            /// </summary>
            public static DocumentDiagnosticReportKind Full { get; } = new("full");

            /// <summary>
            /// A report indicating that the last
            /// returned report is still accurate.
            /// </summary>
            public static DocumentDiagnosticReportKind Unchanged { get; } = new("unchanged");
        }

        /// <summary>
        /// A diagnostic report with a full set of problems.
        ///
        /// @since 3.17.0
        /// </summary>
        public interface IFullDocumentDiagnosticReport : IDiagnosticReport
        {
            /// <summary>
            /// An optional result id. If provided it will
            /// be sent on the next diagnostic request for the
            /// same document.
            /// </summary>
            string? ResultId { get; init; }

            /// <summary>
            /// The actual items.
            /// </summary>
            Container<Diagnostic> Items { get; init; }
        }

        /// <summary>
        /// A diagnostic report with a full set of problems.
        ///
        /// @since 3.17.0
        /// </summary>
        public partial record FullDocumentDiagnosticReport() : DocumentDiagnosticReport(DocumentDiagnosticReportKind.Full), IFullDocumentDiagnosticReport
        {
            /// <summary>
            /// An optional result id. If provided it will
            /// be sent on the next diagnostic request for the
            /// same document.
            /// </summary>
            [Optional]
            public string? ResultId { get; init; }

            /// <summary>
            /// The actual items.
            /// </summary>
            public Container<Diagnostic> Items { get; init; }
        }

        /// <summary>
        /// A diagnostic report indicating that the last returned
        /// report is still accurate.
        ///
        /// @since 3.17.0
        /// </summary>
        public interface IUnchangedDocumentDiagnosticReport : IDiagnosticReport
        {
            /// <summary>
            /// A result id which will be sent on the next
            /// diagnostic request for the same document.
            /// </summary>
            string ResultId { get; init; }
        }

        /// <summary>
        /// A diagnostic report indicating that the last returned
        /// report is still accurate.
        ///
        /// @since 3.17.0
        /// </summary>
        public partial record UnchangedDocumentDiagnosticReport() : DocumentDiagnosticReport(DocumentDiagnosticReportKind.Unchanged),
                                                                    IUnchangedDocumentDiagnosticReport
        {
            /// <summary>
            /// A result id which will be sent on the next
            /// diagnostic request for the same document.
            /// </summary>
            public string ResultId { get; init; }
        }

        /// <summary>
        /// A full diagnostic report with a set of related documents.
        ///
        /// @since 3.17.0
        /// </summary>
        public partial record RelatedFullDocumentDiagnosticReport() : RelatedDocumentDiagnosticReport(DocumentDiagnosticReportKind.Full),
                                                                      IFullDocumentDiagnosticReport
        {
            /// <summary>
            /// The actual items.
            /// </summary>
            public Container<Diagnostic> Items { get; init; }

            /// <summary>
            /// An optional result id. If provided it will
            /// be sent on the next diagnostic request for the
            /// same document.
            /// </summary>
            public string? ResultId { get; init; }
        }

        /// <summary>
        /// An unchanged diagnostic report with a set of related documents.
        ///
        /// @since 3.17.0
        /// </summary>
        public partial record RelatedUnchangedDocumentDiagnosticReport() : RelatedDocumentDiagnosticReport(DocumentDiagnosticReportKind.Unchanged),
                                                                           IUnchangedDocumentDiagnosticReport
        {
            /// <summary>
            /// A result id which will be sent on the next
            /// diagnostic request for the same document.
            /// </summary>
            public string ResultId { get; init; }
        }

        /// <summary>
        /// A partial result for a document diagnostic report.
        ///
        /// @since 3.17.0
        /// </summary>
        public partial record DocumentDiagnosticReportPartialResult
        {
            /// <summary>
            /// Diagnostics of related documents. This information is useful
            /// in programming languages where code in a file A can generate
            /// diagnostics in a file B which A depends on. An example of
            /// such a language is C/C++ where marco definitions in a file
            /// a.cpp and result in errors in a header file b.hpp.
            ///
            /// @since 3.17.0
            /// </summary>
            public ImmutableDictionary<DocumentUri, DocumentDiagnosticReport>? RelatedDocuments { get; init; }

            public static DocumentDiagnosticReportPartialResult? From(RelatedDocumentDiagnosticReport? result)
            {
                if (result is not null)
                {
                    return new DocumentDiagnosticReportPartialResult { RelatedDocuments = result.RelatedDocuments };
                }

                return null;
            }
        }

        /// <summary>
        /// Cancellation data returned from a diagnostic request.
        ///
        /// @since 3.17.0
        /// </summary>
        public partial record DiagnosticServerCancellationData
        {
            public bool RetriggerRequest { get; init; }
        }

        /// <summary>
        /// A previous result id in a workspace pull request.
        ///
        /// @since 3.17.0
        /// </summary>
        public partial record PreviousResultId
        {
            /// <summary>
            /// The URI for which the client knowns a
            /// result id.
            /// </summary>
            public DocumentUri Uri { get; init; }

            /// <summary>
            /// The value of the previous result id.
            /// </summary>
            public string Value { get; init; }
        }

        /// <summary>
        /// A workspace diagnostic report.
        ///
        /// @since 3.17.0
        /// </summary>
        public partial record WorkspaceDiagnosticReport : WorkspaceDiagnosticReportPartialResult
        {
            internal WorkspaceDiagnosticReport()
            {
            }

            internal WorkspaceDiagnosticReport(WorkspaceDiagnosticReportPartialResult partialResult)
            {
                Items = partialResult.Items;
            }

            public static WorkspaceDiagnosticReport From(WorkspaceDiagnosticReport original, WorkspaceDiagnosticReportPartialResult? result)
            {
                return new WorkspaceDiagnosticReport()
                {
                    Items = new Container<WorkspaceDocumentDiagnosticReport>(
                        original.Items.Concat(result?.Items ?? Array.Empty<WorkspaceDocumentDiagnosticReport>())
                    )
                };
            }
        }


        [JsonConverter(typeof(Converter))]
        public abstract partial record WorkspaceDocumentDiagnosticReport(DocumentDiagnosticReportKind Kind) : IDiagnosticReport
        {
            /// <summary>
            /// The report kind
            /// </summary>
            public DocumentDiagnosticReportKind Kind { get; } = Kind;

            /// <summary>
            /// The URI for which diagnostic information is reported.
            /// </summary>
            public DocumentUri Uri { get; init; }

            /// <summary>
            /// The version number for which the diagnostics are reported.
            /// If the document is not marked as open `null` can be provided.
            /// </summary>
            public int? Version { get; init; }

            internal class Converter : JsonConverter<WorkspaceDocumentDiagnosticReport>
            {
                public override void WriteJson(JsonWriter writer, WorkspaceDocumentDiagnosticReport value, JsonSerializer serializer)
                {
                    DiagnosticReportConverter.WriteWorkspaceDocumentDiagnosticReport(writer, value, serializer);
                }

                public override WorkspaceDocumentDiagnosticReport ReadJson(
                    JsonReader reader, Type objectType, WorkspaceDocumentDiagnosticReport existingValue, bool hasExistingValue, JsonSerializer serializer
                )
                {
                    if (reader.TokenType == JsonToken.Null)
                    {
                        return null!;
                    }

                    return DiagnosticReportConverter.ReadWorkspaceDocumentDiagnosticReport(JObject.Load(reader), serializer);
                }
            }

            internal static class DiagnosticReportConverter
            {
                public static DocumentDiagnosticReportKind GetKind(JObject result)
                {
                    var kind = result["kind"]?.Value<string>();
                    if (string.IsNullOrWhiteSpace(kind))
                    {
                        throw new JsonSerializationException("Diagnostic report is missing a kind.");
                    }

                    return new DocumentDiagnosticReportKind(kind);
                }

                public static DocumentDiagnosticReport ReadDocumentDiagnosticReport(JObject result, JsonSerializer serializer)
                {
                    return GetKind(result) switch {
                        var kind when kind == DocumentDiagnosticReportKind.Full => new FullDocumentDiagnosticReport {
                            ResultId = result["resultId"]?.Value<string>(),
                            Items = ReadItems(result, serializer)
                        },
                        var kind when kind == DocumentDiagnosticReportKind.Unchanged => new UnchangedDocumentDiagnosticReport {
                            ResultId = ReadRequiredString(result, "resultId")
                        },
                        var kind => throw new JsonSerializationException($"Unknown diagnostic report kind '{kind}'")
                    };
                }

                public static RelatedDocumentDiagnosticReport ReadRelatedDocumentDiagnosticReport(JObject result, JsonSerializer serializer)
                {
                    return GetKind(result) switch {
                        var kind when kind == DocumentDiagnosticReportKind.Full => new RelatedFullDocumentDiagnosticReport {
                            ResultId = result["resultId"]?.Value<string>(),
                            Items = ReadItems(result, serializer),
                            RelatedDocuments = ReadRelatedDocuments(result, serializer)
                        },
                        var kind when kind == DocumentDiagnosticReportKind.Unchanged => new RelatedUnchangedDocumentDiagnosticReport {
                            ResultId = ReadRequiredString(result, "resultId"),
                            RelatedDocuments = ReadRelatedDocuments(result, serializer)
                        },
                        var kind => throw new JsonSerializationException($"Unknown diagnostic report kind '{kind}'")
                    };
                }

                public static WorkspaceDocumentDiagnosticReport ReadWorkspaceDocumentDiagnosticReport(JObject result, JsonSerializer serializer)
                {
                    return GetKind(result) switch {
                        var kind when kind == DocumentDiagnosticReportKind.Full => new WorkspaceFullDocumentDiagnosticReport {
                            Uri = ReadRequired<DocumentUri>(result, "uri", serializer),
                            Version = result["version"]?.Type == JTokenType.Null ? null : result["version"]?.Value<int?>(),
                            ResultId = result["resultId"]?.Value<string>(),
                            Items = ReadItems(result, serializer)
                        },
                        var kind when kind == DocumentDiagnosticReportKind.Unchanged => new WorkspaceUnchangedDocumentDiagnosticReport {
                            Uri = ReadRequired<DocumentUri>(result, "uri", serializer),
                            Version = result["version"]?.Type == JTokenType.Null ? null : result["version"]?.Value<int?>(),
                            ResultId = ReadRequiredString(result, "resultId")
                        },
                        var kind => throw new JsonSerializationException($"Unknown workspace diagnostic report kind '{kind}'")
                    };
                }

                public static void WriteDocumentDiagnosticReport(JsonWriter writer, DocumentDiagnosticReport? value, JsonSerializer serializer)
                {
                    switch (value)
                    {
                        case null:
                            writer.WriteNull();
                            return;
                        case FullDocumentDiagnosticReport full:
                            WriteFull(writer, full, serializer);
                            return;
                        case UnchangedDocumentDiagnosticReport unchanged:
                            WriteUnchanged(writer, unchanged);
                            return;
                        default:
                            throw new JsonSerializationException($"Unknown diagnostic report type {value.GetType()}");
                    }
                }

                public static void WriteRelatedDocumentDiagnosticReport(JsonWriter writer, RelatedDocumentDiagnosticReport? value, JsonSerializer serializer)
                {
                    switch (value)
                    {
                        case null:
                            writer.WriteNull();
                            return;
                        case RelatedFullDocumentDiagnosticReport full:
                            WriteStartObject(writer, full.Kind);
                            WriteOptionalString(writer, "resultId", full.ResultId);
                            WriteProperty(writer, "items", full.Items, serializer);
                            WriteRelatedDocuments(writer, full.RelatedDocuments, serializer);
                            writer.WriteEndObject();
                            return;
                        case RelatedUnchangedDocumentDiagnosticReport unchanged:
                            WriteStartObject(writer, unchanged.Kind);
                            WriteProperty(writer, "resultId", unchanged.ResultId, serializer);
                            WriteRelatedDocuments(writer, unchanged.RelatedDocuments, serializer);
                            writer.WriteEndObject();
                            return;
                        default:
                            throw new JsonSerializationException($"Unknown related diagnostic report type {value.GetType()}");
                    }
                }

                public static void WriteWorkspaceDocumentDiagnosticReport(JsonWriter writer, WorkspaceDocumentDiagnosticReport? value, JsonSerializer serializer)
                {
                    switch (value)
                    {
                        case null:
                            writer.WriteNull();
                            return;
                        case WorkspaceFullDocumentDiagnosticReport full:
                            WriteStartObject(writer, full.Kind);
                            WriteProperty(writer, "uri", full.Uri, serializer);
                            WriteProperty(writer, "version", full.Version, serializer);
                            WriteOptionalString(writer, "resultId", full.ResultId);
                            WriteProperty(writer, "items", full.Items, serializer);
                            writer.WriteEndObject();
                            return;
                        case WorkspaceUnchangedDocumentDiagnosticReport unchanged:
                            WriteStartObject(writer, unchanged.Kind);
                            WriteProperty(writer, "uri", unchanged.Uri, serializer);
                            WriteProperty(writer, "version", unchanged.Version, serializer);
                            WriteProperty(writer, "resultId", unchanged.ResultId, serializer);
                            writer.WriteEndObject();
                            return;
                        default:
                            throw new JsonSerializationException($"Unknown workspace diagnostic report type {value.GetType()}");
                    }
                }

                private static void WriteFull(JsonWriter writer, IFullDocumentDiagnosticReport full, JsonSerializer serializer)
                {
                    WriteStartObject(writer, full.Kind);
                    WriteOptionalString(writer, "resultId", full.ResultId);
                    WriteProperty(writer, "items", full.Items, serializer);
                    writer.WriteEndObject();
                }

                private static void WriteUnchanged(JsonWriter writer, IUnchangedDocumentDiagnosticReport unchanged)
                {
                    WriteStartObject(writer, unchanged.Kind);
                    WriteProperty(writer, "resultId", unchanged.ResultId, null);
                    writer.WriteEndObject();
                }

                private static void WriteStartObject(JsonWriter writer, DocumentDiagnosticReportKind kind)
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName("kind");
                    writer.WriteValue(kind.ToString());
                }

                private static void WriteRelatedDocuments(
                    JsonWriter writer, ImmutableDictionary<DocumentUri, DocumentDiagnosticReport>? relatedDocuments, JsonSerializer serializer
                )
                {
                    if (relatedDocuments is null || relatedDocuments.Count == 0)
                    {
                        return;
                    }

                    writer.WritePropertyName("relatedDocuments");
                    writer.WriteStartObject();
                    foreach (var item in relatedDocuments)
                    {
                        writer.WritePropertyName(item.Key.ToString());
                        WriteDocumentDiagnosticReport(writer, item.Value, serializer);
                    }

                    writer.WriteEndObject();
                }

                private static ImmutableDictionary<DocumentUri, DocumentDiagnosticReport>? ReadRelatedDocuments(JObject result, JsonSerializer serializer)
                {
                    if (result["relatedDocuments"] is not JObject relatedDocuments)
                    {
                        return null;
                    }

                    var builder = ImmutableDictionary<DocumentUri, DocumentDiagnosticReport>.Empty.ToBuilder();
                    foreach (var property in relatedDocuments.Properties())
                    {
                        builder.Add(DocumentUri.Parse(property.Name), ReadDocumentDiagnosticReport((JObject)property.Value, serializer));
                    }

                    return builder.ToImmutable();
                }

                private static Container<Diagnostic> ReadItems(JObject result, JsonSerializer serializer)
                {
                    return result["items"]?.ToObject<Container<Diagnostic>>(serializer) ?? new Container<Diagnostic>();
                }

                private static string ReadRequiredString(JObject result, string name)
                {
                    return result[name]?.Value<string>() ??
                           throw new JsonSerializationException($"Diagnostic report is missing required property '{name}'.");
                }

                private static T ReadRequired<T>(JObject result, string name, JsonSerializer serializer)
                {
                    var token = result[name] ??
                                throw new JsonSerializationException($"Diagnostic report is missing required property '{name}'.");
                    var value = token.ToObject<T>(serializer);
                    if (value is null)
                    {
                        throw new JsonSerializationException($"Diagnostic report property '{name}' cannot be null.");
                    }

                    return value;
                }

                private static void WriteOptionalString(JsonWriter writer, string name, string? value)
                {
                    if (value is null)
                    {
                        return;
                    }

                    WriteProperty(writer, name, value, null);
                }

                private static void WriteProperty(JsonWriter writer, string name, object? value, JsonSerializer? serializer)
                {
                    writer.WritePropertyName(name);
                    if (serializer is null)
                    {
                        writer.WriteValue(value);
                    }
                    else
                    {
                        serializer.Serialize(writer, value);
                    }
                }
            }
        }

        /// <summary>
        /// A full document diagnostic report for a workspace diagnostic result.
        ///
        /// @since 3.17.0
        /// </summary>
        public partial record WorkspaceFullDocumentDiagnosticReport() : WorkspaceDocumentDiagnosticReport(DocumentDiagnosticReportKind.Full),
                                                                        IFullDocumentDiagnosticReport
        {
            /// <summary>
            /// An optional result id. If provided it will
            /// be sent on the next diagnostic request for the
            /// same document.
            /// </summary>
            public string? ResultId { get; init; }

            /// <summary>
            /// The actual items.
            /// </summary>
            public Container<Diagnostic> Items { get; init; }
        }

        /// <summary>
        /// An unchanged document diagnostic report for a workspace diagnostic result.
        ///
        /// @since 3.17.0
        /// </summary>
        public partial record WorkspaceUnchangedDocumentDiagnosticReport() : WorkspaceDocumentDiagnosticReport(DocumentDiagnosticReportKind.Unchanged),
                                                                             IUnchangedDocumentDiagnosticReport
        {
            /// <summary>
            /// A result id which will be sent on the next
            /// diagnostic request for the same document.
            /// </summary>
            public string ResultId { get; init; }
        };

        /// <summary>
        /// A partial result for a workspace diagnostic report.
        ///
        /// @since 3.17.0
        /// </summary>
        public partial record WorkspaceDiagnosticReportPartialResult
        {
            public Container<WorkspaceDocumentDiagnosticReport> Items { get; init; }

            internal WorkspaceDiagnosticReportPartialResult()
            {
            }

            internal WorkspaceDiagnosticReportPartialResult(WorkspaceDiagnosticReport partialResult)
            {
                Items = partialResult.Items;
            }
        }


        [GenerateRegistrationOptions(nameof(ServerCapabilities.DiagnosticProvider))]
        [RegistrationName(TextDocumentNames.Diagnostics)]
        public partial class DiagnosticsRegistrationOptions : ITextDocumentRegistrationOptions, IWorkDoneProgressOptions, IStaticRegistrationOptions
        {
            /// <summary>
            /// An optional identifier under which the diagnostics are
            /// managed by the client.
            /// </summary>
            [Optional]
            public string? Identifier { get; set; }

            /// <summary>
            /// Whether the language has inter file dependencies meaning that
            /// editing code in one file can result in a different diagnostic
            /// set in another file. Inter file dependencies are common for
            /// most programming languages and typically uncommon for linters.
            /// </summary>
            public bool InterFileDependencies { get; set; }

            /// <summary>
            /// The server provides support for workspace diagnostics as well.
            /// </summary>
            public bool WorkspaceDiagnostics { get; set; }
        }
    }

    namespace Client.Capabilities
    {
        [CapabilityKey(nameof(ClientCapabilities.TextDocument), nameof(TextDocumentClientCapabilities.Diagnostic))]
        public partial class DiagnosticClientCapabilities : DynamicCapability
        {
            /// <summary>
            /// Whether the clients supports related documents for document diagnostic
            /// pulls.
            /// </summary>
            [Optional]
            public bool RelatedDocumentSupport { get; set; }

            /// <summary>
            /// Whether the client supports the `Diagnostic.message` property being a `MarkupContent`.
            ///
            /// @since 3.18.0
            /// </summary>
            [Optional]
            public bool MarkupMessageSupport { get; set; }
        }

        /// <summary>
        /// Capabilities specific to the code lens requests scoped to the
        /// workspace.
        ///
        /// @since 3.17.0.
        /// </summary>
        [CapabilityKey(nameof(ClientCapabilities.Workspace), nameof(WorkspaceClientCapabilities.Diagnostics))]
        public class DiagnosticWorkspaceClientCapabilities : ICapability
        {
            /// <summary>
            /// Whether the client implementation supports a refresh request sent from
            /// the server to the client.
            ///
            /// Note that this event is global and will force the client to refresh all
            /// pulled diagnostics currently shown. It should be used with absolute care
            /// and is useful for situation where a server for example detects a project
            /// wide change that requires such a calculation.
            /// </summary>
            [Optional]
            public bool RefreshSupport { get; set; }
        }
    }

    namespace Document
    {
    }
}
