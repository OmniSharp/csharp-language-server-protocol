using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [JsonConverter(typeof(Converter))]
    public struct BooleanNumberString
    {
        private long? _long;
        private string _string;
        private bool? _bool;

        public BooleanNumberString(long value)
        {
            _long = value;
            _string = null;
            _bool = null;
        }
        public BooleanNumberString(string value)
        {
            _long = null;
            _string = value;
            _bool = null;
        }
        public BooleanNumberString(bool value)
        {
            _long = null;
            _string = null;
            _bool = value;
        }

        public bool IsLong => _long.HasValue;
        public long Long
        {
            get => _long ?? 0;
            set
            {
                String = null;
                _long = value;
                _bool = null;
            }
        }

        public bool IsString => _string != null;
        public string String
        {
            get => _string;
            set
            {
                _string = value;
                _long = null;
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
                _long = null;
                _bool = value;
            }
        }

        public static implicit operator BooleanNumberString(long value)
        {
            return new BooleanNumberString(value);
        }

        public static implicit operator BooleanNumberString(string value)
        {
            return new BooleanNumberString(value);
        }

        public static implicit operator BooleanNumberString(bool value)
        {
            return new BooleanNumberString(value);
        }

        class Converter : JsonConverter<BooleanNumberString>
        {
            public override void Write(Utf8JsonWriter writer, BooleanNumberString value, JsonSerializerOptions options)
            {
                if (value.IsBool)   JsonSerializer.Serialize(writer, value.Bool, options);
                else if (value.IsLong)   JsonSerializer.Serialize(writer, value.Long, options);
                else if (value.IsString)   JsonSerializer.Serialize(writer, value.String, options);
                else writer.WriteNullValue();
            }

            public override BooleanNumberString Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.Number)
                {
                    return new BooleanNumberString(reader.GetInt64());
                }

                if (reader.TokenType == JsonTokenType.String)
                {
                    return new BooleanNumberString(reader.GetString());
                }

                if (reader.TokenType == JsonTokenType.False)
                {
                    return new BooleanNumberString(false);
                }

                if (reader.TokenType == JsonTokenType.True)
                {
                    return new BooleanNumberString(true);
                }

                return new BooleanNumberString();
            }
        }
    }
}
