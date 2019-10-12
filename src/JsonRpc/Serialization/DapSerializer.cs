using System.Collections.Generic;
using Newtonsoft.Json;
using OmniSharp.Extensions.JsonRpc.Serialization.DebugAdapterConverters;

namespace OmniSharp.Extensions.JsonRpc.Serialization
{
    public class DapSerializer : SerializerBase
    {
        protected override void AddOrReplaceConverters(ICollection<JsonConverter> converters)
        {
            ReplaceConverter(converters, new DapClientNotificationConverter(this));
            ReplaceConverter(converters, new DapClientResponseConverter(this));
            ReplaceConverter(converters, new DapClientRequestConverter());
            ReplaceConverter(converters, new DapRpcErrorConverter(this));
        }
    }
}
