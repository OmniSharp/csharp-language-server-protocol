using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace OmniSharp.Extensions.DebugAdapter.Protocol
{
    /// <summary>
    /// Some predefined types for the CompletionItem.Please note that not all clients have specific icons for all of them.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CompletionItemType
    {
        Method, Function, Constructor, Field, Variable, Class, Interface, Module, Property, Unit, Value, Enum, Keyword, Snippet, Text, Color, File, Reference, CustomColor
    }
}