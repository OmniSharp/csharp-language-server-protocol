using System.Collections.Generic;
using System.Text.Json;
using OmniSharp.Extensions.JsonRpc.Server;

namespace OmniSharp.Extensions.JsonRpc
{
    public interface IReceiver
    {
        (IEnumerable<Renor> results, bool hasResponse) GetRequests(JsonElement container);
        bool IsValid(JsonElement container);
    }
}
