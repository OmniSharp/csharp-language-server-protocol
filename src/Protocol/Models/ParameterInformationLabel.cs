using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [JsonConverter(typeof(Converter))]
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

        public static implicit operator ParameterInformationLabel(string label)
        {
            return new ParameterInformationLabel(label);
        }

        public static implicit operator ParameterInformationLabel((int start, int end) range)
        {
            return new ParameterInformationLabel(range);
        }

        class Converter : JsonConverter<ParameterInformationLabel>
        {
            public override void Write(Utf8JsonWriter writer, ParameterInformationLabel value,
                JsonSerializerOptions options)
            {
                if (value.IsLabel)
                    JsonSerializer.Serialize(writer, value.Label, options);
                if (value.IsRange)
                    JsonSerializer.Serialize(writer, new[] {value.Range.start, value.Range.end}, options);
            }

            public override ParameterInformationLabel Read(ref Utf8JsonReader reader, Type typeToConvert,
                JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.String)
                {
                    return new ParameterInformationLabel(reader.GetString());
                }

                if (reader.TokenType == JsonTokenType.StartArray)
                {
                    reader.Read();
                    var value1 = reader.GetInt32();
                    reader.Read();
                    var value2 = reader.GetInt32();
                    reader.Read();
                    return new ParameterInformationLabel((value1, value2));
                }

                throw new JsonException("Unexpected type");
            }
        }
    }
}
