namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class ParameterInformationLabel
    {
        public ParameterInformationLabel((int start, int end) range)
        {
            Range = range;
        }

        public ParameterInformationLabel(string label)
        {
            Label = label;
        }

        public (int start, int end) Range { get; }
        public bool IsRange => Label == null;
        public string Label { get; }
        public bool IsLabel => Label != null;

        public static implicit operator ParameterInformationLabel(string label) {
            return new ParameterInformationLabel(label);
        }

        public static implicit operator ParameterInformationLabel((int start, int end) range) {
            return new ParameterInformationLabel(range);
        }
    }
}
