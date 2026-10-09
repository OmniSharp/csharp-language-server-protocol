using System.ComponentModel;
using System.Reflection;
using System.Text.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// Common interface for types that support resolution.
    /// </summary>
    public interface ICanBeResolved
    {
        /// <summary>
        /// A data entry field that is preserved for resolve requests
        /// </summary>
        [Optional]
        JsonElement? Data { get; init; }
    }

    public static class CanBeResolvedExtension
    {
        private static readonly PropertyInfo CanBeResolvedProperty = typeof(ICanBeResolved).GetProperty(nameof(ICanBeResolved.Data))!;
        private static readonly PropertyInfo CanHaveDataProperty = typeof(ICanHaveData).GetProperty(nameof(ICanHaveData.Data))!;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void SetRawData(this ICanBeResolved? resolved, JsonElement? value)
        {
            if (resolved is null) return;
            CanBeResolvedProperty.SetValue(resolved, value);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void SetRawData<T>(this ICanBeResolved? resolved, T value) where T : class?
        {
            if (resolved is null) return;
            CanBeResolvedProperty.SetValue(resolved, JsonSerializer.SerializeToElement(value));
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static JsonElement? GetRawData(this ICanBeResolved? resolved)
        {
            if (resolved is null) return null;
            return CanBeResolvedProperty.GetValue(resolved) is JsonElement value ? value : null;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static T? GetRawData<T>(this ICanBeResolved? resolved) where T : class?
        {
            if (resolved is null) return null;
            return CanBeResolvedProperty.GetValue(resolved) is JsonElement value ? value.Deserialize<T>() : null;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void SetRawData(this ICanHaveData? resolved, JsonElement? value)
        {
            if (resolved is null) return;
            CanHaveDataProperty.SetValue(resolved, value);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void SetRawData<T>(this ICanHaveData? resolved, T value)
        {
            if (resolved is null) return;
            CanHaveDataProperty.SetValue(resolved, JsonSerializer.SerializeToElement(value));
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static JsonElement? GetRawData(this ICanHaveData? resolved)
        {
            if (resolved is null) return null;
            return CanHaveDataProperty.GetValue(resolved) is JsonElement value ? value : null;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static T? GetRawData<T>(this ICanHaveData? resolved) where T : class?
        {
            if (resolved is null) return null;
            return CanHaveDataProperty.GetValue(resolved) is JsonElement value ? value.Deserialize<T>() : null;
        }
    }

    public interface ICanHaveData
    {
        /// <summary>
        /// A data entry field that is preserved for resolve requests
        /// </summary>
        [Optional]
        JsonElement? Data { get; init; }
    }
}
