using System;
using System.Globalization;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Lsp.Tests
{
    static class Fixture
    {
        public static string SerializeObject(object value)
        {
            return SerializeObject(value, null, (JsonSerializerSettings)null);
        }

        public static string SerializeObject(object value, Type type, JsonSerializerSettings settings)
        {
            var jsonSerializer = JsonSerializer.CreateDefault(settings);

            return SerializeObjectInternal(value, type, jsonSerializer);
        }

        private static string SerializeObjectInternal(object value, Type type, JsonSerializer jsonSerializer)
        {
            var sb = new StringBuilder(256);
            var sw = new StringWriter(sb, CultureInfo.InvariantCulture);
            using (var jsonWriter = new JsonTextWriter(sw))
            {
                jsonWriter.Formatting = Formatting.Indented;
                jsonWriter.Indentation = 4;

                jsonSerializer.Serialize(jsonWriter, value, type);
            }

            return sw.ToString()?.Replace("\r\n", "\n");//?.Replace("\n", "\r\n");
        }
    }
}