using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc.Server;

namespace OmniSharp.Extensions.JsonRpc
{
    public interface IReceiver
    {
        (IEnumerable<Renor> results, bool hasResponse) GetRequests(JToken container);
        bool IsValid(JToken container);
        void Initialized();
    }
}
