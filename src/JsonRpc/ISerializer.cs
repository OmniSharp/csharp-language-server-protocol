using Newtonsoft.Json;

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public interface ISerializer
    {
        JsonSerializer JsonSerializer { get; }
        JsonSerializerSettings Settings { get; }
        string SerializeObject(object value);
        object DeserializeObject(string json);
        T DeserializeObject<T>(string json);
    }
}
