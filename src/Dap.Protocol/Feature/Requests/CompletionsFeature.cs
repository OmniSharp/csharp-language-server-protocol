using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.DebugAdapter.Protocol
{
    namespace Requests
    {
        [Parallel]
        [Method(RequestNames.Completions, Direction.ClientToServer)]
        [
            GenerateHandler,
            GenerateHandlerMethods,
            GenerateRequestMethods
        ]
        public record CompletionsArguments : IRequest<CompletionsResponse>
        {
            /// <summary>
            /// Returns completions in the scope of this stack frame. If not specified, the completions are returned for the global scope.
            /// </summary>
            [Optional]
            public long? FrameId { get; init; }

            /// <summary>
            /// One or more source lines.Typically this is the text a user has typed into the debug console before he asked for completion.
            /// </summary>
            public string Text { get; init; }

            /// <summary>
            /// The character position for which to determine the completion proposals.
            /// </summary>
            public long Column { get; init; }

            /// <summary>
            /// An optional line for which to determine the completion proposals.If missing the first line of the text is assumed.
            /// </summary>
            [Optional]
            public long? Line { get; init; }
        }

        public record CompletionsResponse
        {
            /// <summary>
            /// The possible completions for .
            /// </summary>
            public Container<CompletionItem> Targets { get; init; }
        }
    }

    namespace Models
    {
        /// <summary>
        /// CompletionItems are the suggestions returned from the CompletionsRequest.
        /// </summary>
        public record CompletionItem
        {
            /// <summary>
            /// The label of this completion item. By default this is also the text that is inserted when selecting this completion.
            /// </summary>
            public string Label { get; init; }

            /// <summary>
            /// If text is not falsy then it is inserted instead of the label.
            /// </summary>
            [Optional]
            public string? Text { get; init; }

            /// <summary>
            /// The item's type. Typically the client uses this information to render the item in the UI with an icon.
            /// </summary>
            [Optional]
            public CompletionItemType Type { get; init; }

            /// <summary>
            /// This value determines the location (in the CompletionsRequest's 'text' attribute) where the completion text is added.
            /// If missing the text is added at the location specified by the CompletionsRequest's 'column' attribute.
            /// </summary>
            [Optional]
            public int? Start { get; init; }

            /// <summary>
            /// This value determines how many characters are overwritten by the completion text.
            /// If missing the value 0 is assumed which results in the completion text being inserted.
            /// </summary>
            [Optional]
            public int? Length { get; init; }

            /// <summary>
            /// Determines the start of the new selection after the text has been inserted (or replaced).
            /// The start position must in the range 0 and length of the completion text.
            /// If omitted the selection starts at the end of the completion text.
            /// </summary>
            [Optional]
            public int? SelectionStart { get; init; }

            /// <summary>
            /// Determines the length of the new selection after the text has been inserted (or replaced).
            /// The selection can not extend beyond the bounds of the completion text.
            /// If omitted the length is assumed to be 0.
            /// </summary>
            [Optional]
            public int? SelectionLength { get; init; }
        }

        /// <summary>
        /// Some predefined types for the CompletionItem.Please note that not all clients have specific icons for all of them.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enum CompletionItemType
        {
            Method, Function, Constructor, Field, Variable, Class, Interface, Module, Property, Unit, Value, Enum, Keyword, Snippet, Text, Color, File, Reference, CustomColor
        }
    }
}
