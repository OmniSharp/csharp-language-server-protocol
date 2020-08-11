using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [JsonConverter(typeof(DiagnosticCodeConverter))]
    public struct DiagnosticCode
    {
        public DiagnosticCode(long value)
        {
            Long = value;
            String = null;
        }

        public DiagnosticCode(string value)
        {
            Long = 0;
            String = value;
        }

        public bool IsLong => String == null;
        public long Long { get; set; }
        public bool IsString => String != null;
        public string String { get; set; }

        public static implicit operator DiagnosticCode(long value) => new DiagnosticCode(value);

        public static implicit operator DiagnosticCode(string value) => new DiagnosticCode(value);

        public static implicit operator long(DiagnosticCode value) => value.IsLong ? value.Long : 0;

        public static implicit operator string(DiagnosticCode value) => value.IsString ? value.String : null;
    }
}
