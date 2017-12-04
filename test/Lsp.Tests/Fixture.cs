using System;
using System.Globalization;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;

namespace Lsp.Tests
{
    static class Fixture
    {
        public static string SerializeObject(object value, ClientVersion version = ClientVersion.Lsp3)
        {
            return SerializeObject(value, null, (JsonSerializerSettings)null, version);
        }

        public static string SerializeObject(object value, Type type, JsonSerializerSettings settings, ClientVersion version = ClientVersion.Lsp3)
        {
            var jsonSerializer = Serializer.CreateSerializer(version);

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

            return sw.ToString()?.Replace("\r\n", "\n")?.TrimEnd();//?.Replace("\n", "\r\n");
        }
    }
}
