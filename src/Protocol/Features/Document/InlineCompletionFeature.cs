using System.Collections;
using System.Reflection;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    namespace Models
    {
        [Parallel]
        [Method(TextDocumentNames.InlineCompletion, Direction.ClientToServer)]
        [
            GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Document"),
            GenerateHandlerMethods,
            GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))
        ]
        [RegistrationOptions(typeof(InlineCompletionRegistrationOptions)), Capability(typeof(InlineCompletionClientCapabilities))]
        public partial record InlineCompletionParams : TextDocumentPositionParams, IWorkDoneProgressParams,
                                                       IRequest<InlineCompletionList?>
        {
            /// <summary>
            /// Additional information about the context in which inline completions were requested.
            /// </summary>
            public InlineCompletionContext Context { get; init; } = null!;
        }

        /// <summary>
        /// Provides information about the context in which an inline completion was requested.
        ///
        /// @since 3.18.0
        /// </summary>
        public record InlineCompletionContext
        {
            /// <summary>
            /// Describes how the inline completion was triggered.
            /// </summary>
            public InlineCompletionTriggerKind TriggerKind { get; init; }

            /// <summary>
            /// Provides information about the currently selected item in the autocomplete widget if it is visible.
            /// </summary>
            [Optional]
            public SelectedCompletionInfo? SelectedCompletionInfo { get; init; }
        }

        /// <summary>
        /// Describes the currently selected completion item.
        ///
        /// @since 3.18.0
        /// </summary>
        public record SelectedCompletionInfo
        {
            /// <summary>
            /// The range that will be replaced if this completion item is accepted.
            /// </summary>
            public Range Range { get; init; } = null!;

            /// <summary>
            /// The text the range will be replaced with if this completion is accepted.
            /// </summary>
            public string Text { get; init; } = null!;
        }

        /// <summary>
        /// Describes how an inline completion was triggered.
        ///
        /// @since 3.18.0
        /// </summary>
        [JsonConverter(typeof(NumberEnumConverter))]
        public enum InlineCompletionTriggerKind
        {
            /// <summary>
            /// Completion was triggered explicitly by a user gesture.
            /// </summary>
            Invoked = 1,

            /// <summary>
            /// Completion was triggered automatically while editing.
            /// </summary>
            Automatic = 2
        }

        /// <summary>
        /// An inline completion item represents text proposed inline to complete text that is being typed.
        ///
        /// @since 3.18.0
        /// </summary>
        [GenerateContainer("InlineCompletionList", GenerateImplicitConversion = false)]
        public partial record InlineCompletionItem
        {
            /// <summary>
            /// The text to replace the range with. Must be set.
            /// </summary>
            public StringOrStringValue InsertText { get; init; } = null!;

            /// <summary>
            /// A text that is used to decide if this inline completion should be shown.
            /// </summary>
            [Optional]
            public string? FilterText { get; init; }

            /// <summary>
            /// The range to replace. Must begin and end on the same line.
            /// </summary>
            [Optional]
            public Range? Range { get; init; }

            /// <summary>
            /// An optional command that is executed after inserting this completion.
            /// </summary>
            [Optional]
            public Command? Command { get; init; }
        }

        /// <summary>
        /// Represents a collection of inline completion items to be presented in the editor.
        ///
        /// @since 3.18.0
        /// </summary>
        [JsonConverter(typeof(Converter))]
        public partial class InlineCompletionList
        {
            public IEnumerable<InlineCompletionItem> Items => this;

            public static InlineCompletionList From(InlineCompletionList? source, IEnumerable<InlineCompletionItem>? result)
                => new((source?.Items ?? Array.Empty<InlineCompletionItem>()).Concat(result ?? Array.Empty<InlineCompletionItem>()));

            internal class Converter : JsonConverter<InlineCompletionList>
            {
                public override void WriteJson(JsonWriter writer, InlineCompletionList? value, JsonSerializer serializer)
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName("items");
                    serializer.Serialize(writer, (value?.Items ?? Array.Empty<InlineCompletionItem>()).ToArray());
                    writer.WriteEndObject();
                }

                public override InlineCompletionList? ReadJson(
                    JsonReader reader, Type objectType, InlineCompletionList? existingValue, bool hasExistingValue, JsonSerializer serializer
                )
                {
                    if (reader.TokenType == JsonToken.StartArray)
                    {
                        return new InlineCompletionList(JArray.Load(reader).ToObject<IEnumerable<InlineCompletionItem>>(serializer)!);
                    }

                    if (reader.TokenType == JsonToken.Null)
                    {
                        return null;
                    }

                    var result = JObject.Load(reader);
                    return new InlineCompletionList(result["items"]!.ToObject<IEnumerable<InlineCompletionItem>>(serializer)!);
                }

                public override bool CanRead => true;
            }
        }

        /// <summary>
        /// A string value used as a snippet.
        ///
        /// @since 3.18.0
        /// </summary>
        public record StringValue
        {
            public string Kind { get; init; } = "snippet";

            public string Value { get; init; } = null!;
        }

        [JsonConverter(typeof(Converter))]
        public record StringOrStringValue
        {
            public StringOrStringValue(string value) => String = value;

            public StringOrStringValue(StringValue stringValue) => StringValue = stringValue;

            public string? String { get; }
            public bool HasString => StringValue is null;

            public StringValue? StringValue { get; }
            public bool HasStringValue => StringValue is { };

            public static implicit operator StringOrStringValue?(string? value) => value is null ? null : new StringOrStringValue(value);

            public static implicit operator StringOrStringValue?(StringValue? value) => value is null ? null : new StringOrStringValue(value);

            internal class Converter : JsonConverter<StringOrStringValue>
            {
                public override void WriteJson(JsonWriter writer, StringOrStringValue value, JsonSerializer serializer)
                {
                    if (value.HasString)
                    {
                        writer.WriteValue(value.String);
                    }
                    else
                    {
                        serializer.Serialize(writer, value.StringValue);
                    }
                }

                public override StringOrStringValue ReadJson(
                    JsonReader reader, Type objectType, StringOrStringValue existingValue, bool hasExistingValue, JsonSerializer serializer
                )
                {
                    return reader.TokenType == JsonToken.StartObject
                        ? new StringOrStringValue(JObject.Load(reader).ToObject<StringValue>(serializer)!)
                        : new StringOrStringValue((reader.Value as string)!);
                }

                public override bool CanRead => true;
            }
        }

        [GenerateRegistrationOptions(nameof(ServerCapabilities.InlineCompletionProvider))]
        [RegistrationName(TextDocumentNames.InlineCompletion)]
        public partial class InlineCompletionRegistrationOptions : ITextDocumentRegistrationOptions, IWorkDoneProgressOptions, IStaticRegistrationOptions
        {
        }
    }

    namespace Client.Capabilities
    {
        [CapabilityKey(nameof(ClientCapabilities.TextDocument), nameof(TextDocumentClientCapabilities.InlineCompletion))]
        public partial class InlineCompletionClientCapabilities : DynamicCapability
        {
        }
    }

    namespace Document
    {
    }
}
