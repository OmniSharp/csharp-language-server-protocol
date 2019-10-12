using System;
using System.Collections.Generic;
using System.Threading;
using Newtonsoft.Json;
using OmniSharp.Extensions.JsonRpc.Serialization;
using OmniSharp.Extensions.JsonRpc.Serialization.Converters;
using OmniSharp.Extensions.JsonRpc.Serialization.DebugAdapterConverters;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Serialization
{
    public class DapProtocolSerializer : DapSerializer, ISerializer
    {
        protected override void AddOrReplaceConverters(ICollection<JsonConverter> converters)
        {
            ReplaceConverter(converters, new NumberStringConverter());
            base.AddOrReplaceConverters(converters);
        }

        protected override JsonSerializer CreateSerializer()
        {
            var serializer = base.CreateSerializer();
            serializer.ContractResolver = new ContractResolver();
            return serializer;
        }

        protected override JsonSerializerSettings CreateSerializerSettings()
        {
            var settings = base.CreateSerializerSettings();
            settings.ContractResolver = new ContractResolver();
            return settings;
        }
    }
}
