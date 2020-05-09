using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;

namespace OmniSharp.Extensions.JsonRpc.Serialization
{
    public abstract class SerializerBase : ISerializer
    {
        private long _id = 0;

        protected virtual JsonSerializerOptions CreateSerializerSettings()
        {
            var settings = new JsonSerializerOptions();
            AddOrReplaceConverters(settings.Converters);
            return _options = settings;
        }

        protected internal void ReplaceConverter<T>(ICollection<JsonConverter> converters, T item)
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

        private JsonSerializerOptions _options;
        public JsonSerializerOptions Options => _options ?? ( CreateSerializerSettings() );

        public long GetNextId()
        {
            return Interlocked.Increment(ref _id);
        }
        protected abstract void AddOrReplaceConverters(ICollection<JsonConverter> converters);
    }
}
