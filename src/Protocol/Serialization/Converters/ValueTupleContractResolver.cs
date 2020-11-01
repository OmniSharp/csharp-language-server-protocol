using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    internal class ValueTupleContractResolver<T1, T2> : JsonConverter<(T1, T2)>
    {
        public override void WriteJson(JsonWriter writer, (T1, T2) value, JsonSerializer serializer) => serializer.Serialize(writer, new object[] { value.Item1!, value.Item2! });

        public override (T1, T2) ReadJson(JsonReader reader, Type objectType, (T1, T2) existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var a = JArray.Load(reader);
            return ( a.ToObject<T1>(), a.ToObject<T2>() );
        }
    }
}
