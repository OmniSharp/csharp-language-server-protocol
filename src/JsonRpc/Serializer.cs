using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol;

namespace OmniSharp.Extensions.JsonRpc
{
    class Serializer : ISerializer
    {
        public Serializer()
        {
            JsonSerializer = CreateSerializer();
            Settings = CreateSerializerSettings();
        }

        private static JsonSerializer CreateSerializer()
        {
            var serializer = JsonSerializer.CreateDefault();
            return serializer;
        }

        private static JsonSerializerSettings CreateSerializerSettings()
        {
            var settings = JsonConvert.DefaultSettings != null ? JsonConvert.DefaultSettings() : new JsonSerializerSettings();
            return settings;
        }

        public JsonSerializer JsonSerializer { get; }

        public JsonSerializerSettings Settings { get; }

        public string SerializeObject(object value)
        {
            return JsonConvert.SerializeObject(value, Settings);
        }

        public object DeserializeObject(string json)
        {
            return JsonConvert.DeserializeObject(json, Settings);
        }

        public T DeserializeObject<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, Settings);
        }
    }
}