using System.Text.Json;

namespace JsonRpc.Tests
{
    internal static class JsonTestHelper
    {
        public static JsonElement Parse(string json)
        {
            using var document = JsonDocument.Parse(json);
            return document.RootElement.Clone();
        }

        public static JsonElement ToElement<T>(T value) => JsonSerializer.SerializeToElement(value);
    }
}
