using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [JsonConverter(typeof(Converter))]
    public struct BooleanString
    {
        private string _string;
        private bool? _bool;
        public BooleanString(string value)
        {
            _string = value;
            _bool = null;
        }
        public BooleanString(bool value)
        {
            _string = null;
            _bool = value;
        }

        public bool IsString => _string != null;
        public string String
        {
            get => _string;
            set
            {
                _string = value;
                _bool = null;
            }
        }

        public bool IsBool => _bool.HasValue;
        public bool Bool
        {
            get => _bool.HasValue && _bool.Value;
            set
            {
                String = null;
                _bool = value;
            }
        }

        public static implicit operator BooleanString(string value)
        {
            return new BooleanString(value);
        }

        public static implicit operator BooleanString(bool value)
        {
            return new BooleanString(value);
        }

        class Converter : JsonConverter<BooleanString>
        {
            public override void Write(Utf8JsonWriter writer, BooleanString value, JsonSerializerOptions options)
            {
                if (value.IsBool)   JsonSerializer.Serialize(writer, value.Bool, options);
                if (value.IsString)   JsonSerializer.Serialize(writer, value.String, options);
            }

            public override BooleanString Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                switch (reader.TokenType)
                {
                    case JsonTokenType.String:
                        return new BooleanString(reader.GetString());
                    case JsonTokenType.False:
                        return new BooleanString(false);
                    case JsonTokenType.True:
                        return new BooleanString(true);
                }

                return new BooleanString();
            }
        }
    }
}
