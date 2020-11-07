using System;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [JsonConverter(typeof(ProgressTokenConverter))]
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public readonly struct ProgressToken : IEquatable<ProgressToken>, IEquatable<long>, IEquatable<string>, IEquatable<Guid>
    {
        private readonly long? _long;
        private readonly string? _string;

        public ProgressToken(long value)
        {
            _long = value;
            _string = null;
        }

        public ProgressToken(string value)
        {
            _long = null;
            _string = value;
        }

        public ProgressToken(Guid value)
        {
            _long = null;
            _string = value.ToString();
        }

        public bool IsLong => _long.HasValue;
        public long Long => _long ?? 0;

        public bool IsString => _string != null;
        public string? String => _string;

        public static implicit operator ProgressToken(long value) => new ProgressToken(value);

        public static implicit operator ProgressToken(string value) => new ProgressToken(value);
        public static implicit operator ProgressToken(Guid value) => new ProgressToken(value);

        public ProgressParams Create<T>(T value, JsonSerializer jsonSerializer) => ProgressParams.Create(this, value, jsonSerializer);

        public override bool Equals(object obj) =>
            obj is ProgressToken token &&
            Equals(token);

        public override int GetHashCode()
        {
            var hashCode = 1456509845;
            hashCode = hashCode * -1521134295 + Long.GetHashCode();
            if (String != null) hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(String);
            return hashCode;
        }

        public bool Equals(ProgressToken other) =>
            IsLong == other.IsLong &&
            Long == other.Long &&
            IsString == other.IsString &&
            String == other.String;

        public bool Equals(long other) => IsLong && Long == other;
        public bool Equals(string other) => IsString && String == other;
        public bool Equals(Guid other) => IsString && String == other.ToString();

        private string DebuggerDisplay => IsString ? String! : IsLong ? Long.ToString() : "";

        /// <inheritdoc />
        public override string ToString() => DebuggerDisplay;
    }
}
