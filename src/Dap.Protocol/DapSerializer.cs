using System.Collections.Generic;
using Newtonsoft.Json;
using OmniSharp.Extensions.DebugAdapter.Protocol.DebugAdapterConverters;
using OmniSharp.Extensions.JsonRpc.Serialization;

namespace OmniSharp.Extensions.DebugAdapter.Protocol
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
