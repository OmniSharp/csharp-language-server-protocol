using System;
using System.Globalization;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using ISerializer = OmniSharp.Extensions.JsonRpc.ISerializer;

namespace Lsp.Tests
{
    internal static class Fixture
    {
        public static string SerializeObject(object value, ClientVersion version = ClientVersion.Lsp3) => SerializeObject(value, null, null, version);

        public static string SerializeObject(object value, Type? type, JsonSerializerSettings? settings, ClientVersion version = ClientVersion.Lsp3)
        {
            var jsonSerializer = new LspSerializer(version);

            return SerializeObjectInternal(value, type, jsonSerializer);
        }

        private static string SerializeObjectInternal(object value, Type? type, ISerializer serializer)
        {
            var sb = new StringBuilder(256);
            var sw = new StringWriter(sb, CultureInfo.InvariantCulture);
            using (var jsonWriter = new JsonTextWriter(sw))
            {
                jsonWriter.Formatting = Formatting.Indented;
                jsonWriter.Indentation = 4;

                serializer.JsonSerializer.Serialize(jsonWriter, value, type);
            }

            return sw.ToString().Replace("\r\n", "\n").TrimEnd(); //?.Replace("\n", "\r\n");
        }
    }
}
