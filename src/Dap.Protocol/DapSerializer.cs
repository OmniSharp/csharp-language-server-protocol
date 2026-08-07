using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Client;
using OmniSharp.Extensions.JsonRpc.Serialization;
using OmniSharp.Extensions.JsonRpc.Server.Messages;

namespace OmniSharp.Extensions.DebugAdapter.Protocol
{
    public class DapSerializer : SystemTextJsonSerializer
    {
        public DapSerializer() : base(CreateOptions())
        {
            Options.Converters.Insert(0, new DapRpcErrorConverterFactory(this));
            Options.Converters.Insert(0, new DapOutgoingResponseConverter(this));
            Options.Converters.Insert(0, new DapOutgoingNotificationConverter(this));
            Options.Converters.Insert(0, new DapOutgoingRequestConverter());
        }

        private static JsonSerializerOptions CreateOptions()
        {
            var resolver = new DefaultJsonTypeInfoResolver();
            resolver.Modifiers.Add(ConfigureTypeInfo);

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                TypeInfoResolver = resolver
            };
            options.Converters.Add(new NumberStringConverter());
            options.Converters.Add(new ProgressTokenConverter());
            options.Converters.Add(new InferredObjectConverter());
            return options;
        }

        private static void ConfigureTypeInfo(JsonTypeInfo typeInfo)
        {
            foreach (var property in typeInfo.Properties)
            {
                if (typeInfo.Type.Name.EndsWith("Capabilities")
                 || property.AttributeProvider?.GetCustomAttributes(typeof(OptionalAttribute), true).Any() == true)
                {
                    property.ShouldSerialize = (_, value) => !IsDefaultValue(value, property.PropertyType);
                }
            }
        }

        private static bool IsDefaultValue(object? value, Type type) =>
            value is null || type.IsValueType && value.Equals(Activator.CreateInstance(type));

        private static void WriteValue(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            if (value is JToken token)
            {
                using var document = JsonDocument.Parse(token.ToString(Newtonsoft.Json.Formatting.None));
                document.RootElement.WriteTo(writer);
                return;
            }

            JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }

        private static void WriteTraceData(Utf8JsonWriter writer, string? traceParent, string? traceState)
        {
            if (traceParent is null) return;
            writer.WriteString("traceparent", traceParent);
            if (!string.IsNullOrWhiteSpace(traceState)) writer.WriteString("tracestate", traceState);
        }

        private sealed class DapOutgoingRequestConverter : JsonConverter<OutgoingRequest>
        {
            public override OutgoingRequest Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
                throw new NotSupportedException();

            public override void Write(Utf8JsonWriter writer, OutgoingRequest value, JsonSerializerOptions options)
            {
                writer.WriteStartObject();
                writer.WritePropertyName("seq");
                WriteValue(writer, value.Id!, options);
                writer.WriteString("type", "request");
                writer.WriteString("command", value.Method);
                if (value.Params is not null)
                {
                    writer.WritePropertyName("arguments");
                    WriteValue(writer, value.Params, options);
                }
                WriteTraceData(writer, value.TraceParent, value.TraceState);
                writer.WriteEndObject();
            }
        }

        private sealed class DapOutgoingNotificationConverter : JsonConverter<OutgoingNotification>
        {
            private readonly DapSerializer _serializer;

            public DapOutgoingNotificationConverter(DapSerializer serializer) => _serializer = serializer;

            public override OutgoingNotification Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
                throw new NotSupportedException();

            public override void Write(Utf8JsonWriter writer, OutgoingNotification value, JsonSerializerOptions options)
            {
                writer.WriteStartObject();
                writer.WriteNumber("seq", _serializer.GetNextId());
                writer.WriteString("type", "event");
                writer.WriteString("event", value.Method);
                if (value.Params is not null)
                {
                    writer.WritePropertyName("body");
                    WriteValue(writer, value.Params, options);
                }
                WriteTraceData(writer, value.TraceParent, value.TraceState);
                writer.WriteEndObject();
            }
        }

        private sealed class DapOutgoingResponseConverter : JsonConverter<OutgoingResponse>
        {
            private readonly DapSerializer _serializer;

            public DapOutgoingResponseConverter(DapSerializer serializer) => _serializer = serializer;

            public override OutgoingResponse Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
                throw new NotSupportedException();

            public override void Write(Utf8JsonWriter writer, OutgoingResponse value, JsonSerializerOptions options)
            {
                writer.WriteStartObject();
                writer.WriteNumber("seq", _serializer.GetNextId());
                writer.WritePropertyName("type");
                writer.WriteStringValue("response");
                writer.WritePropertyName("request_seq");
                WriteValue(writer, value.Id, options);
                writer.WriteBoolean("success", true);
                writer.WriteString("command", value.Request.Method);
                if (value.Result is not null)
                {
                    writer.WritePropertyName("body");
                    WriteValue(writer, value.Result, options);
                }
                writer.WriteEndObject();
            }
        }

        private sealed class DapRpcErrorConverterFactory : JsonConverterFactory
        {
            private readonly DapSerializer _serializer;

            public DapRpcErrorConverterFactory(DapSerializer serializer) => _serializer = serializer;

            public override bool CanConvert(Type typeToConvert) => typeof(RpcError).IsAssignableFrom(typeToConvert);

            public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options) => new DapRpcErrorConverter(_serializer);
        }

        private sealed class DapRpcErrorConverter : JsonConverter<RpcError>
        {
            private readonly DapSerializer _serializer;

            public DapRpcErrorConverter(DapSerializer serializer) => _serializer = serializer;

            public override RpcError Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                using var document = JsonDocument.ParseValue(ref reader);
                var root = document.RootElement;
                object? id = null;
                if (root.TryGetProperty("id", out var idElement))
                {
                    id = idElement.ValueKind switch
                    {
                        JsonValueKind.Number => idElement.GetInt64(),
                        JsonValueKind.String => idElement.GetString(),
                        _ => null
                    };
                }

                var error = root.TryGetProperty("message", out var messageElement) && messageElement.ValueKind == JsonValueKind.Object
                    ? messageElement.Deserialize<ErrorMessage>(options)
                    : null;
                return new RpcError(id, error);
            }

            public override void Write(Utf8JsonWriter writer, RpcError value, JsonSerializerOptions options)
            {
                writer.WriteStartObject();
                writer.WriteNumber("seq", _serializer.GetNextId());
                writer.WriteString("type", "response");
                if (value.Id is not null)
                {
                    writer.WritePropertyName("request_seq");
                    WriteValue(writer, value.Id, options);
                }
                writer.WriteBoolean("success", false);
                writer.WriteString("command", value.Method);
                writer.WriteString("message", value.Error?.Message);
                writer.WritePropertyName("body");
                JsonSerializer.Serialize(writer, value.Error, options);
                writer.WriteEndObject();
            }
        }

        private sealed class NumberStringConverter : JsonConverter<NumberString>
        {
            public override NumberString Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => reader.TokenType switch
            {
                JsonTokenType.Number => new NumberString(reader.GetInt64()),
                JsonTokenType.String => new NumberString(reader.GetString()!),
                _ => new NumberString()
            };

            public override void Write(Utf8JsonWriter writer, NumberString value, JsonSerializerOptions options)
            {
                if (value.IsLong) writer.WriteNumberValue(value.Long);
                else if (value.IsString) writer.WriteStringValue(value.String);
                else writer.WriteNullValue();
            }
        }

        private sealed class ProgressTokenConverter : JsonConverter<ProgressToken>
        {
            public override ProgressToken? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => reader.TokenType switch
            {
                JsonTokenType.Number => new ProgressToken(reader.GetInt64()),
                JsonTokenType.String when !string.IsNullOrWhiteSpace(reader.GetString()) => new ProgressToken(reader.GetString()!),
                _ => null
            };

            public override void Write(Utf8JsonWriter writer, ProgressToken value, JsonSerializerOptions options)
            {
                if (value.IsLong) writer.WriteNumberValue(value.Long);
                else if (value.IsString) writer.WriteStringValue(value.String);
                else writer.WriteNullValue();
            }
        }

        private sealed class InferredObjectConverter : JsonConverter<object>
        {
            public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => reader.TokenType switch
            {
                JsonTokenType.True => true,
                JsonTokenType.False => false,
                JsonTokenType.Number when reader.TryGetInt64(out var longValue) => longValue,
                JsonTokenType.Number => reader.GetDouble(),
                JsonTokenType.String => reader.GetString(),
                JsonTokenType.Null => null,
                _ => JsonDocument.ParseValue(ref reader).RootElement.Clone()
            };

            public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
            {
                if (value.GetType() == typeof(object))
                {
                    writer.WriteStartObject();
                    writer.WriteEndObject();
                    return;
                }

                JsonSerializer.Serialize(writer, value, value.GetType(), options);
            }
        }

    }
}
