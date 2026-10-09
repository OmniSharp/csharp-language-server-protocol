using System;
using System.Text.Json;

namespace OmniSharp.Extensions.JsonRpc
{
    public class DelegatingNotification<T> : IRequest<Unit>
    {
        private static readonly JsonSerializerOptions SerializerOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        public DelegatingNotification(object value) => Value = ToJsonElement(value);

        public JsonElement Value { get; }

        private static JsonElement ToJsonElement(object value)
        {
            if (typeof(T) == typeof(Unit) || value is Unit)
            {
                return JsonSerializer.SerializeToElement(new { });
            }

            if (value is null) throw new ArgumentNullException(nameof(value));

            if (value is JsonElement element)
            {
                return element.Clone();
            }

            return JsonSerializer.SerializeToElement(value, value.GetType(), SerializerOptions);
        }
    }
}
