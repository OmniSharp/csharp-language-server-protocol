using System;
using System.Text.Json;
using Newtonsoft.Json.Linq;

namespace OmniSharp.Extensions.JsonRpc
{
    public class DelegatingNotification<T> : IRequest<Unit>
    {
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

            var token = value as JToken ?? JToken.FromObject(value);
            using var document = JsonDocument.Parse(token.ToString(Newtonsoft.Json.Formatting.None));
            return document.RootElement.Clone();
        }
    }
}
