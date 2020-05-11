using System.Text.Json.Serialization;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Models
{
    /// <summary>
    /// Some predefined types for the CompletionItem.Please note that not all clients have specific icons for all of them.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CompletionItemType
    {
        Method, Function, Constructor, Field, Variable, Class, Interface, Module, Property, Unit, Value, Enum, Keyword, Snippet, Text, Color, File, Reference, CustomColor
    }
}
