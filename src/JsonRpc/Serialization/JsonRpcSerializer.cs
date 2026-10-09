using System;

namespace OmniSharp.Extensions.JsonRpc.Serialization
{
    public class JsonRpcSerializer : SystemTextJsonSerializer
    {
        public override string SerializeObject(object value, Type type) => base.SerializeObject(value, value.GetType());
    }
}
