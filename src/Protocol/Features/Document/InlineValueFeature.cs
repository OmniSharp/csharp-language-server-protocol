using System.Diagnostics;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    namespace Models
    {
        [Parallel]
        [Method(TextDocumentNames.InlineValue, Direction.ClientToServer)]
        [
            GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Document", Name = "InlineValues"),
            GenerateHandlerMethods,
            GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))
        ]
        [RegistrationOptions(typeof(InlineValueRegistrationOptions)), Capability(typeof(InlineValueWorkspaceClientCapabilities))]
        public partial record InlineValueParams : ITextDocumentIdentifierParams, IWorkDoneProgressParams,
                                                  IRequest<Container<InlineValueBase>?>
        {
            /// <summary>
            /// The text document.
            /// </summary>
            public TextDocumentIdentifier TextDocument { get; init; }

            /// <summary>
            /// The document range for which inline values should be computed.
            /// </summary>
            public Range Range { get; init; }

            /// <summary>
            /// Additional information about the context in which inline values were
            /// requested.
            /// </summary>
            public InlineValueContext Context { get; init; }
        }

        /// <summary>
        /// @since 3.17.0
        /// </summary>
        public partial record InlineValueContext
        {
            /// <summary>
            /// The stack frame (as a DAP Id) where the execution has stopped.
            /// </summary>
            public int FrameId { get; set; }

            /// <summary>
            /// The document range where execution has stopped.
            /// Typically the end position of the range denotes the line where the
            /// inline values are shown.
            /// </summary>
            public Range StoppedLocation { get; set; }
        }

        [Parallel]
        [Method(WorkspaceNames.InlineValueRefresh, Direction.ServerToClient)]
        [GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Workspace")]
        [GenerateHandlerMethods]
        [GenerateRequestMethods(typeof(IWorkspaceLanguageServer), typeof(ILanguageServer))]
        [Capability(typeof(CodeLensWorkspaceClientCapabilities))]
        public partial record InlineValueRefreshParams : IRequest;

        [JsonConverter(typeof(Converter))]
        public abstract partial record InlineValueBase
        {
            /// <summary>
            /// The document range for which the inline value applies.
            /// </summary>
            public Range Range { get; init; }

            internal class Converter : JsonConverter<InlineValueBase>
            {
                public override bool CanWrite => false;

                public override void WriteJson(JsonWriter writer, InlineValueBase value, JsonSerializer serializer)
                {
                    throw new NotImplementedException();
                }

                public override InlineValueBase ReadJson(
                    JsonReader reader, Type objectType, InlineValueBase existingValue, bool hasExistingValue, JsonSerializer serializer
                )
                {
                    var result = JObject.Load(reader);
                    if (result.ContainsKey("text"))
                    {
                        return new InlineValueText()
                        {
                            Range = result["range"]!.ToObject<Range?>(serializer)!,
                            Text = result["text"]!.Value<string>()!
                        };
                    }

                    if (result.ContainsKey("variableName") || result.ContainsKey("caseSensitiveLookup"))
                    {
                        return new InlineValueVariableLookup()
                        {
                            Range = result["range"].ToObject<Range>(serializer)!,
                            VariableName = result["variableName"]!.Value<string>()!,
                            CaseSensitiveLookup = result["caseSensitiveLookup"]?.Value<bool?>() ?? false,
                        };
                    }

                    return new InlineValueEvaluatableExpression()
                    {
                        Range = result["range"].ToObject<Range>(serializer)!,
                        Expression = result["expression"]?.Value<string>()
                    };
                }
            }
        }

        /// <summary>
        /// Provide inline value as text.
        ///
        /// @since 3.17.0
        /// </summary>
        public partial record InlineValueText : InlineValueBase
        {
            /// <summary>
            /// The text of the inline value.
            /// </summary>
            public string Text { get; init; }
        }

        /// <summary>
        /// Provide inline value through a variable lookup.
        ///
        /// If only a range is specified, the variable name will be extracted from
        /// the underlying document.
        ///
        /// An optional variable name can be used to override the extracted name.
        ///
        /// @since 3.17.0
        /// </summary>
        public partial record InlineValueVariableLookup : InlineValueBase
        {
            /// <summary>
            /// If specified the name of the variable to look up.
            /// </summary>
            [Optional]
            public string? VariableName { get; init; }

            /// <summary>
            /// How to perform the lookup.
            /// </summary>
            public bool CaseSensitiveLookup { get; init; }
        }

        /// <summary>
        /// Provide an inline value through an expression evaluation.
        ///
        /// If only a range is specified, the expression will be extracted from the
        /// underlying document.
        ///
        /// An optional expression can be used to override the extracted expression.
        ///
        /// @since 3.17.0
        /// </summary>
        public partial record InlineValueEvaluatableExpression : InlineValueBase
        {
            /// <summary>
            /// If specified the expression overrides the extracted expression.
            /// </summary>
            public string? Expression { get; init; }
        }

        [GenerateRegistrationOptions(nameof(ServerCapabilities.InlineValueProvider))]
        [RegistrationName(TextDocumentNames.InlineValue)]
        public partial class InlineValueRegistrationOptions : ITextDocumentRegistrationOptions, IWorkDoneProgressOptions
        {
        }
    }

    namespace Server.Capabilities
    {
    }

    namespace Client.Capabilities
    {
        /// <summary>
        /// Client workspace capabilities specific to inline values.
        ///
        /// @since 3.17.0
        /// </summary>
        [CapabilityKey(nameof(ClientCapabilities.TextDocument), nameof(TextDocumentClientCapabilities.InlineValue))]
        public partial class InlineValueWorkspaceClientCapabilities : ICapability
        {
            /// <summary>
            /// Whether the client implementation supports a refresh request sent from
            /// the server to the client.
            ///
            /// Note that this event is global and will force the client to refresh all
            /// inline values currently shown. It should be used with absolute care and
            /// is useful for situation where a server for example detect a project wide
            /// change that requires such a calculation.
            /// </summary>
            [Optional]
            public bool RefreshSupport { get; set; }
        }
    }

    namespace Document
    {
    }
}
