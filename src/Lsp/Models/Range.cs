using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServerProtocol.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class Range
    {
        public Range()
        {

        }

        public Range(Position start, Position end)
        {
            Start = start;
            End = end;
        }

        /// <summary>
        /// The range's start position.
        /// </summary>
        public Position Start { get; set; }

        /// <summary>
        /// The range's end position.
        /// </summary>
        public Position End { get; set; }
    }
}