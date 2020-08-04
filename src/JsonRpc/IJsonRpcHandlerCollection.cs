using System.Collections.Generic;

namespace OmniSharp.Extensions.JsonRpc
{
    public interface IJsonRpcHandlerCollection : IEnumerable<JsonRpcHandlerDescription>
    {
        IJsonRpcHandlerCollection Add(JsonRpcHandlerDescription description);
    }
}