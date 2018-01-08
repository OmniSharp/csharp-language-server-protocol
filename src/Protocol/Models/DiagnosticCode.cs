using Newtonsoft.Json;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
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
        public bool IsLong => this.String == null;
        public long Long { get; set; }
        public bool IsString => this.String != null;
        public string String { get; set; }
        public object Value => String ?? (object)Long;

        public static implicit operator DiagnosticCode(long value)
        {
            return new DiagnosticCode(value);
        }

        public static implicit operator DiagnosticCode(string value)
        {
            return new DiagnosticCode(value);
        }
    }
}
