using System.Text.Json;

namespace Lsp.Tests
{
    internal static class JsonTestHelper
    {
        public static JsonElement Parse(string json)
        {
            using var document = JsonDocument.Parse(json);
            return document.RootElement.Clone();
        }
    }
}