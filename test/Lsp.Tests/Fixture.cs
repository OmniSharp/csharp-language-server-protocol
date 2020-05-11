using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;
using Nerdbank.Streams;
using Newtonsoft.Json;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using JsonSerializer = System.Text.Json.JsonSerializer;
using Serializer = OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Serializer;

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
            var jsonSerializer = new Serializer(version);

            return SerializeObjectInternal(value, type, jsonSerializer);
        }

        private static string SerializeObjectInternal(object value, Type type, ISerializer serializer)
        {
            return JsonSerializer.Serialize(value, type)?.Replace("\r\n", "\n")?.TrimEnd();//?.Replace("\n", "\r\n");
        }
    }
}
