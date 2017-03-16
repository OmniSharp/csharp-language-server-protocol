using Lsp.Converters;
using Newtonsoft.Json;

namespace Lsp.Capabilities.Client
{
    [JsonConverter(typeof(CapabilityConverter))]
    public struct Capability<T>
    {
        public Capability(bool isSupported, T value)
        {
            IsSupported = isSupported;
            Value = value;
        }

        public Capability(bool isSupported)
        {
            IsSupported = isSupported;
            Value = default(T);
        }

        public T Value { get; set; }
        public bool IsSupported { get; set; }

        public static implicit operator T(Capability<T> value)
        {
            return value.Value;
        }

        public static implicit operator Capability<T>(T value)
        {
            return new Capability<T>(true, value);
        }
    }

    public static class Capability
    {
        public static Capability<T> OfValue<T>(T value)
        {
            return new Capability<T>(true, value);
        }

        public static Capability<T> OfBoolean<T>(bool isSupported)
        {
            return new Capability<T>(isSupported);
        }
    }
}