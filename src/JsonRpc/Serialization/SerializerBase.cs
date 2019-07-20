using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;

namespace OmniSharp.Extensions.JsonRpc.Serialization
{
    public abstract class SerializerBase : ISerializer
    {
        private long _id = 0;

        protected virtual JsonSerializer CreateSerializer()
        {
            var serializer = JsonSerializer.CreateDefault();
            AddOrReplaceConverters(serializer.Converters);
            return _jsonSerializer = serializer;
        }

        protected virtual JsonSerializerSettings CreateSerializerSettings()
        {
            var settings = JsonConvert.DefaultSettings != null ? JsonConvert.DefaultSettings() : new JsonSerializerSettings();
            AddOrReplaceConverters(settings.Converters);
            return _settings = settings;
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

        private JsonSerializer _jsonSerializer;
        public JsonSerializer JsonSerializer => _jsonSerializer ?? ( CreateSerializer() );


        private JsonSerializerSettings _settings;
        public JsonSerializerSettings Settings => _settings ?? ( CreateSerializerSettings() );

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
            return Interlocked.Increment(ref _id);
        }
        protected abstract void AddOrReplaceConverters(ICollection<JsonConverter> converters);
    }
}
