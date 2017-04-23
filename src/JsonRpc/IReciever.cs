using System.Collections.Generic;
using JsonRpc.Server;
using Newtonsoft.Json.Linq;

namespace JsonRpc
{
    public interface IReciever
    {
        (IEnumerable<Renor> results, bool hasResponse) GetRequests(JToken container);
        bool IsValid(JToken container);
    }
}