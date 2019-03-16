namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class ParameterInformationLabel
    {
        public ParameterInformationLabel((long start, long end) range)
        {
            Range = range;
        }

        public ParameterInformationLabel(string label)
        {
            Label = label;
        }

        public (long start, long end) Range { get; }
        public bool IsRange => Label == null;
        public string Label { get; }
        public bool IsLabel => Label != null;

        public static implicit operator ParameterInformationLabel(string label) {
            return new ParameterInformationLabel(label);
        }

        public static implicit operator ParameterInformationLabel((long start, long end) range) {
            return new ParameterInformationLabel(range);
        }
    }
}
