using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public abstract class WorkDoneProgress
    {
        public WorkDoneProgress(WorkDoneProgressKind kind)
        {
            Kind = kind;
        }

        public WorkDoneProgressKind Kind { get; }

        /// <summary>
        /// Optional, a final message indicating to for example indicate the outcome
        /// of the operation.
        /// </summary>
        [Optional]
        public string Message { get; set; }
    }

    [JsonConverter(typeof(EnumLikeStringConverter))]
    public readonly struct WorkDoneProgressKind : IEquatable<WorkDoneProgressKind>, IEnumLikeString
    {
        private static readonly Lazy<IReadOnlyList<WorkDoneProgressKind>> _defaults =
            new Lazy<IReadOnlyList<WorkDoneProgressKind>>(
                () => {
                    return typeof(WorkDoneProgressKind)
                        .GetFields(BindingFlags.Static | BindingFlags.Public)
                        .Select(z => z.GetValue(null))
                        .Cast<WorkDoneProgressKind>()
                        .ToArray();
                });

        public static IEnumerable<WorkDoneProgressKind> Defaults => _defaults.Value;

        public static WorkDoneProgressKind Begin = new WorkDoneProgressKind("begin");
        public static WorkDoneProgressKind End = new WorkDoneProgressKind("end");
        public static WorkDoneProgressKind Report = new WorkDoneProgressKind("report");

        private readonly string _value;

        public WorkDoneProgressKind(string modifier)
        {
            _value = modifier;
        }

        public static implicit operator WorkDoneProgressKind(string kind)
        {
            return new WorkDoneProgressKind(kind);
        }

        public static implicit operator string(WorkDoneProgressKind kind)
        {
            return kind._value;
        }

        public override string ToString() => _value;
        public bool Equals(WorkDoneProgressKind other) => _value == other._value;

        public override bool Equals(object obj) => obj is WorkDoneProgressKind other && Equals(other);

        public override int GetHashCode() => (_value != null ? _value.GetHashCode() : 0);

        public static bool operator ==(WorkDoneProgressKind left, WorkDoneProgressKind right) => left.Equals(right);

        public static bool operator !=(WorkDoneProgressKind left, WorkDoneProgressKind right) => !left.Equals(right);
    }
}
