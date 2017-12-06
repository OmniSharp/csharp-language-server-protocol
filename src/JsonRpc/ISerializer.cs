using System;
using Newtonsoft.Json;

namespace OmniSharp.Extensions.JsonRpc
{
    public interface ISerializer
    {
        JsonSerializer JsonSerializer { get; }
        JsonSerializerSettings Settings { get; }
        string SerializeObject(object value);
        object DeserializeObject(string json, Type type);
        T DeserializeObject<T>(string json);
    }
}
