using System.Collections.Generic;
using Newtonsoft.Json;
using OmniSharp.Extensions.JsonRpc.Serialization.Converters;

namespace OmniSharp.Extensions.JsonRpc.Serialization
{
    public class JsonRpcSerializer : SerializerBase
    {
        protected override void AddOrReplaceConverters(ICollection<JsonConverter> converters)
        {
            ReplaceConverter(converters, new ClientNotificationConverter());
            ReplaceConverter(converters, new ClientResponseConverter());
            ReplaceConverter(converters, new ClientRequestConverter());
            ReplaceConverter(converters, new ErrorMessageConverter());
            ReplaceConverter(converters, new RpcErrorConverter());
        }
    }
}
