using System.Text.Json.Serialization;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Models
{
    /// <summary>
    /// CompletionItems are the suggestions returned from the CompletionsRequest.
    /// </summary>
    public class CompletionItem
    {
        /// <summary>
        /// The label of this completion item. By default this is also the text that is inserted when selecting this completion.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// If text is not falsy then it is inserted instead of the label.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public string Text { get; set; }

        /// <summary>
        /// The item's type. Typically the client uses this information to render the item in the UI with an icon.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public CompletionItemType Type { get; set; }

        /// <summary>
        /// This value determines the location (in the CompletionsRequest's 'text' attribute) where the completion text is added.
        /// If missing the text is added at the location specified by the CompletionsRequest's 'column' attribute.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public int? Start { get; set; }

        /// <summary>
        /// This value determines how many characters are overwritten by the completion text.
        /// If missing the value 0 is assumed which results in the completion text being inserted.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public int? Length { get; set; }
    }
}
