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
        public class CompletionsArguments : IRequest<CompletionsResponse>
        {
            /// <summary>
            /// Returns completions in the scope of this stack frame. If not specified, the completions are returned for the global scope.
            /// </summary>
            [Optional]
            public long? FrameId { get; set; }

            /// <summary>
            /// One or more source lines.Typically this is the text a user has typed into the debug console before he asked for completion.
            /// </summary>
            public string Text { get; set; } = null!;

            /// <summary>
            /// The character position for which to determine the completion proposals.
            /// </summary>
            public long Column { get; set; }

            /// <summary>
            /// An optional line for which to determine the completion proposals.If missing the first line of the text is assumed.
            /// </summary>
            [Optional]
            public long? Line { get; set; }
        }

        public class CompletionsResponse
        {
            /// <summary>
            /// The possible completions for .
            /// </summary>
            public Container<CompletionItem> Targets { get; set; } = null!;
        }
    }

    namespace Models
    {
        /// <summary>
        /// CompletionItems are the suggestions returned from the CompletionsRequest.
        /// </summary>
        public class CompletionItem
        {
            /// <summary>
            /// The label of this completion item. By default this is also the text that is inserted when selecting this completion.
            /// </summary>
            public string Label { get; set; } = null!;

            /// <summary>
            /// If text is not falsy then it is inserted instead of the label.
            /// </summary>
            [Optional]
            public string? Text { get; set; } = null!;

            /// <summary>
            /// The item's type. Typically the client uses this information to render the item in the UI with an icon.
            /// </summary>
            [Optional]
            public CompletionItemType Type { get; set; }

            /// <summary>
            /// This value determines the location (in the CompletionsRequest's 'text' attribute) where the completion text is added.
            /// If missing the text is added at the location specified by the CompletionsRequest's 'column' attribute.
            /// </summary>
            [Optional]
            public int? Start { get; set; }

            /// <summary>
            /// This value determines how many characters are overwritten by the completion text.
            /// If missing the value 0 is assumed which results in the completion text being inserted.
            /// </summary>
            [Optional]
            public int? Length { get; set; }

            /// <summary>
            /// Determines the start of the new selection after the text has been inserted (or replaced).
            /// The start position must in the range 0 and length of the completion text.
            /// If omitted the selection starts at the end of the completion text.
            /// </summary>
            [Optional]
            public int? SelectionStart { get; set; }

            /// <summary>
            /// Determines the length of the new selection after the text has been inserted (or replaced).
            /// The selection can not extend beyond the bounds of the completion text.
            /// If omitted the length is assumed to be 0.
            /// </summary>
            [Optional]
            public int? SelectionLength { get; set; }
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
