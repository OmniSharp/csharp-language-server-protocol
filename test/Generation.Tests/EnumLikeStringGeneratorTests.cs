using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc.Generators;
using OmniSharp.Extensions.JsonRpc.Generators.Cache;
using TestingUtils;

namespace Generation.Tests
{
    public class EnumLikeStringGeneratorTests
    {
        [FactWithSkipOn(SkipOnPlatform.Windows)]
        public async Task Auto_Magically_Implements_IEnumLikeString()
        {
            var source = @"
using OmniSharp.Extensions.JsonRpc.Generation;
namespace Test {
    [StringEnum]
    public readonly partial struct ThreadEventReason
    {
        public static ThreadEventReason Started { get; } = new ThreadEventReason(""started"");
        public static ThreadEventReason Exited { get; } = new ThreadEventReason(""exited"");
    }
}
";
            var expected = @"
#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Serialization.Converters;

namespace Test
{
    [JsonConverter(typeof(EnumLikeStringConverter))]
    [DebuggerDisplay(""{_value}"")]
    public readonly partial struct ThreadEventReason : IEquatable<string>, IEquatable<ThreadEventReason>, IEnumLikeString
    {
        private static readonly Lazy<IReadOnlyList<ThreadEventReason>> _defaults = new Lazy<IReadOnlyList<ThreadEventReason>>(() =>
        {
            return typeof(ThreadEventReason).GetProperties(BindingFlags.Static | BindingFlags.Public).Where(z => z.Name != nameof(Defaults)).Select(z => z.GetValue(null)).Cast<ThreadEventReason>().ToArray();
        });
        public static IEnumerable<ThreadEventReason> Defaults => _defaults.Value;
        private readonly string _value;
        public ThreadEventReason(string type) => _value = type;
        public static implicit operator ThreadEventReason(string kind) => new ThreadEventReason(kind);
        public static implicit operator string (ThreadEventReason kind) => kind._value;
        public override string ToString() => _value;
        public bool Equals(ThreadEventReason other) => _value == other._value;
        public bool Equals(string other) => _value == other;
        public override bool Equals(object obj) => obj is string s && Equals(s) || obj is ThreadEventReason other && Equals(other);
        public override int GetHashCode() => _value.GetHashCode();
        public static bool operator ==(ThreadEventReason left, ThreadEventReason right) => left.Equals(right);
        public static bool operator !=(ThreadEventReason left, ThreadEventReason right) => !left.Equals(right);
    }
}
#nullable restore";
            CacheKeyHasher.Cache = true;
            await GenerationHelpers.AssertGeneratedAsExpected<EnumLikeStringGenerator>(source, expected);
        }
    }
}
