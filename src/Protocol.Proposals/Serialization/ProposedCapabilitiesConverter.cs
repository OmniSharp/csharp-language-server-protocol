using System;
using Newtonsoft.Json;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization
{
    internal class ProposedCapabilitiesConverter<TFrom, TTo> : JsonConverter
        where TTo : TFrom
        where TFrom : notnull
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return serializer.Deserialize<TTo>(reader);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(TFrom);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanWrite => false;
    }
}
