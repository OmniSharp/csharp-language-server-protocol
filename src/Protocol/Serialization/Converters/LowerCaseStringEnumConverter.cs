using Newtonsoft.Json.Converters;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    class LowercaseStringEnumConverter : StringEnumConverter
    {
        public LowercaseStringEnumConverter() : base(true) { }
    }
}
