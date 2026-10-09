using System;
using System.Text.Encodings.Web;
using System.Text.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using ISerializer = OmniSharp.Extensions.JsonRpc.ISerializer;

namespace Lsp.Tests
{
    internal static class Fixture
    {
        private static readonly JsonSerializerOptions _indented = new() {
            WriteIndented = true,
            IndentSize = 4,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        public static string SerializeObject(object value, ClientVersion version = ClientVersion.Lsp3)
        {
            var jsonSerializer = new LspSerializer(version);
            return SerializeObjectInternal(value, null, jsonSerializer);
        }

        private static string SerializeObjectInternal(object value, Type? type, ISerializer serializer)
        {
            var json = serializer.SerializeObject(value, type ?? value.GetType());
            using var doc = JsonDocument.Parse(json);
            return JsonSerializer.Serialize(doc.RootElement, _indented).Replace("\r\n", "\n").TrimEnd();
        }
    }
}
