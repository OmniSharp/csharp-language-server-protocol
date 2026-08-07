using System;
using Newtonsoft.Json;

namespace OmniSharp.Extensions.JsonRpc
{
    public interface ISerializer
    {
        JsonSerializer JsonSerializer { get; }
        JsonSerializerSettings Settings { get; }
        string SerializeObject(object value);
        string SerializeObject(object value, Type type);
        object DeserializeObject(string json, Type type);
        T DeserializeObject<T>(string json);
        object DeserializeObject(object value, Type type);
        T DeserializeObject<T>(object value);
        void PopulateObject(string json, object target);
        long GetNextId();
    }
}
