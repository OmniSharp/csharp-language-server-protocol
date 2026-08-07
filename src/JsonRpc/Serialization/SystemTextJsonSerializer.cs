using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc.Client;
using StjJsonSerializer = System.Text.Json.JsonSerializer;

namespace OmniSharp.Extensions.JsonRpc.Serialization
{
    public class SystemTextJsonSerializer : ISerializer
    {
        private long _id;

        public SystemTextJsonSerializer() : this(new JsonSerializerOptions())
        {
        }

        public SystemTextJsonSerializer(JsonSerializerOptions options)
        {
            Options = new JsonSerializerOptions(options)
            {
                MaxDepth = options.MaxDepth == 0 ? 128 : options.MaxDepth,
                PropertyNameCaseInsensitive = true
            };
            Options.Converters.Add(new OutgoingRequestConverter());
            Options.Converters.Add(new OutgoingNotificationConverter());
            Options.Converters.Add(new OutgoingResponseConverter());
        }

        public JsonSerializerOptions Options { get; }

        public string SerializeObject(object value) => SerializeObject(value, value.GetType());

        public string SerializeObject(object value, Type type)
        {
            if (value is JToken token)
            {
                return token.ToString(Newtonsoft.Json.Formatting.None);
            }

            return StjJsonSerializer.Serialize(value, type, Options);
        }

        public object DeserializeObject(string json, Type type) =>
            StjJsonSerializer.Deserialize(json, type, Options)
         ?? throw new JsonException($"Unable to deserialize {type.FullName}.");

        public T DeserializeObject<T>(string json) =>
            StjJsonSerializer.Deserialize<T>(json, Options)
         ?? throw new JsonException($"Unable to deserialize {typeof(T).FullName}.");

        public object DeserializeObject(object value, Type type) => DeserializeObject(GetJson(value), type);

        public T DeserializeObject<T>(object value) => DeserializeObject<T>(GetJson(value));

        public void PopulateObject(string json, object target)
        {
            var source = DeserializeObject(json, target.GetType());
            foreach (var property in target.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (property.CanRead && property.CanWrite)
                {
                    property.SetValue(target, property.GetValue(source));
                }
            }
        }

        public long GetNextId() => Interlocked.Increment(ref _id);

        private string GetJson(object value) => value switch
        {
            JToken token => token.ToString(Newtonsoft.Json.Formatting.None),
            JsonElement element => element.GetRawText(),
            _ => SerializeObject(value)
        };

        private static void WriteValue(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            if (value is JToken token)
            {
                using var document = JsonDocument.Parse(token.ToString(Newtonsoft.Json.Formatting.None));
                document.RootElement.WriteTo(writer);
                return;
            }

            StjJsonSerializer.Serialize(writer, value, value.GetType(), options);
        }

        private sealed class OutgoingRequestConverter : JsonConverter<OutgoingRequest>
        {
            public override OutgoingRequest Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
                throw new NotSupportedException();

            public override void Write(Utf8JsonWriter writer, OutgoingRequest value, JsonSerializerOptions options)
            {
                writer.WriteStartObject();
                writer.WriteString("jsonrpc", "2.0");
                writer.WritePropertyName("id");
                WriteValue(writer, value.Id!, options);
                writer.WriteString("method", value.Method);
                if (value.Params is not null)
                {
                    writer.WritePropertyName("params");
                    WriteValue(writer, value.Params, options);
                }
                WriteTraceData(writer, value.TraceParent, value.TraceState);
                writer.WriteEndObject();
            }
        }

        private sealed class OutgoingNotificationConverter : JsonConverter<OutgoingNotification>
        {
            public override OutgoingNotification Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
                throw new NotSupportedException();

            public override void Write(Utf8JsonWriter writer, OutgoingNotification value, JsonSerializerOptions options)
            {
                writer.WriteStartObject();
                writer.WriteString("jsonrpc", "2.0");
                writer.WriteString("method", value.Method);
                if (value.Params is not null)
                {
                    writer.WritePropertyName("params");
                    WriteValue(writer, value.Params, options);
                }
                WriteTraceData(writer, value.TraceParent, value.TraceState);
                writer.WriteEndObject();
            }
        }

        private sealed class OutgoingResponseConverter : JsonConverter<OutgoingResponse>
        {
            public override OutgoingResponse Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
                throw new NotSupportedException();

            public override void Write(Utf8JsonWriter writer, OutgoingResponse value, JsonSerializerOptions options)
            {
                writer.WriteStartObject();
                writer.WriteString("jsonrpc", "2.0");
                writer.WritePropertyName("id");
                WriteValue(writer, value.Id, options);
                writer.WritePropertyName("result");
                if (value.Result is null)
                {
                    writer.WriteNullValue();
                }
                else
                {
                    WriteValue(writer, value.Result, options);
                }
                writer.WriteEndObject();
            }
        }

        private static void WriteTraceData(Utf8JsonWriter writer, string? traceParent, string? traceState)
        {
            if (traceParent is null) return;
            writer.WriteString("traceparent", traceParent);
            if (!string.IsNullOrWhiteSpace(traceState))
            {
                writer.WriteString("tracestate", traceState);
            }
        }
    }
}
