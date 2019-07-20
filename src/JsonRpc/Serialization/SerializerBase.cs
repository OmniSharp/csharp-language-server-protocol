using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace OmniSharp.Extensions.JsonRpc.Serialization
{
    public abstract class SerializerBase : ISerializer
    {
        private readonly object _lock = new object();
        private long _id = 0;
        public SerializerBase()
        {
            JsonSerializer = CreateSerializer();
            Settings = CreateSerializerSettings();
        }

        protected JsonSerializer CreateSerializer()
        {
            var serializer = JsonSerializer.CreateDefault();
            AddOrReplaceConverters(serializer.Converters);
            return serializer;
        }

        protected JsonSerializerSettings CreateSerializerSettings()
        {
            var settings = JsonConvert.DefaultSettings != null ? JsonConvert.DefaultSettings() : new JsonSerializerSettings();
            AddOrReplaceConverters(settings.Converters);
            return settings;
        }

        protected internal static void ReplaceConverter<T>(ICollection<JsonConverter> converters, T item)
            where T : JsonConverter
        {
            var existingConverters = converters.OfType<T>().ToArray();
            if (existingConverters.Any())
            {
                foreach (var converter in existingConverters)
                    converters.Remove(converter);
            }
            converters.Add(item);
        }

        public JsonSerializer JsonSerializer { get; }

        public JsonSerializerSettings Settings { get; }

        public string SerializeObject(object value)
        {
            return JsonConvert.SerializeObject(value, Settings);
        }

        public object DeserializeObject(string json, Type type)
        {
            return JsonConvert.DeserializeObject(json, type, Settings);
        }

        public T DeserializeObject<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, Settings);
        }
        public long GetNextId()
        {
            lock (_lock)
            {
                return _id++;
            }
        }
        protected abstract void AddOrReplaceConverters(ICollection<JsonConverter> converters);
    }
}