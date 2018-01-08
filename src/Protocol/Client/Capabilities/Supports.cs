using System;
using Newtonsoft.Json;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public struct Supports<T> : ISupports
    {
        public Supports(bool isSupported, T value)
        {
            IsSupported = isSupported;
            Value = value;
        }

        public Supports(bool isSupported)
        {
            IsSupported = isSupported;
            Value = default(T);
        }

        public T Value { get; set; }
        public bool IsSupported { get; set; }
        public Type ValueType => typeof(T);
        object ISupports.Value => Value;

        public static implicit operator T(Supports<T> value)
        {
            return value.Value;
        }

        public static implicit operator Supports<T>(T value)
        {
            return new Supports<T>(true, value);
        }
    }

    public static class Supports
    {
        public static Supports<T> OfValue<T>(T value)
            where T : class
        {
            return new Supports<T>(value != null, value);
        }

        public static Supports<T> OfBoolean<T>(bool isSupported)
        {
            return new Supports<T>(isSupported);
        }
    }
}
