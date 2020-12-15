using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using OmniSharp.Extensions.DebugAdapter.Protocol.DebugAdapterConverters;
using OmniSharp.Extensions.JsonRpc.Serialization;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Serialization
{
    [Obsolete("DapProtocolSerializer will be removed in a future version, DapSerializer will be a drop in replacement")]
    public class DapProtocolSerializer : SerializerBase, ISerializer
    {
        protected override void AddOrReplaceConverters(ICollection<JsonConverter> converters)
        {
            ReplaceConverter(converters, new NumberStringConverter());
            ReplaceConverter(converters, new DapClientNotificationConverter(this));
            ReplaceConverter(converters, new DapClientResponseConverter(this));
            ReplaceConverter(converters, new DapClientRequestConverter());
            ReplaceConverter(converters, new DapRpcErrorConverter(this));
            ReplaceConverter(converters, new ProgressTokenConverter());
        }

        protected override JsonSerializer CreateSerializer()
        {
            var serializer = base.CreateSerializer();
            serializer.ContractResolver = new DapContractResolver();
            return serializer;
        }

        protected override JsonSerializerSettings CreateSerializerSettings()
        {
            var settings = base.CreateSerializerSettings();
            settings.ContractResolver = new DapContractResolver();
            return settings;
        }
    }
}
