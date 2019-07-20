using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;

namespace OmniSharp.Extensions.DebugAdapter.Protocol
{
    public struct NumberString
    {
        private long? _long;
        private string _string;

        public NumberString(long value)
        {
            _long = value;
            _string = null;
        }
        public NumberString(string value)
        {
            _long = null;
            _string = value;
        }

        public bool IsLong => _long.HasValue;
        public long Long
        {
            get => _long ?? 0;
            set
            {
                String = null;
                _long = value;
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
            }
        }

        public static implicit operator NumberString(long value)
        {
            return new NumberString(value);
        }

        public static implicit operator NumberString(string value)
        {
            return new NumberString(value);
        }
    }
}
