using System.Diagnostics;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [JsonConverter(typeof(ParameterInformationLabelConverter))]
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class ParameterInformationLabel
    {
        public ParameterInformationLabel((int start, int end) range) => Range = range;

        public ParameterInformationLabel(string label) => Label = label;

        public (int start, int end) Range { get; }
        public bool IsRange => Label == null;
        public string Label { get; }
        public bool IsLabel => Label != null;

        public static implicit operator ParameterInformationLabel(string label) => new ParameterInformationLabel(label);

        public static implicit operator ParameterInformationLabel((int start, int end) range) => new ParameterInformationLabel(range);

        private string DebuggerDisplay => IsRange ? $"(start: {Range.start}, end: {Range.end})" : IsLabel ? Label : string.Empty;

        /// <inheritdoc />
        public override string ToString() => DebuggerDisplay;
    }
}
