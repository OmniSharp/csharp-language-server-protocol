using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using OmniSharp.Extensions.JsonRpc.Serialization;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Serialization
{
    public class DapProtocolSerializer : DapSerializer, ISerializer
    {
        protected override void AddOrReplaceConverters(ICollection<JsonConverter> converters)
        {
            ReplaceConverter(converters, new NumberStringConverter());
            base.AddOrReplaceConverters(converters);
        }
    }
}
