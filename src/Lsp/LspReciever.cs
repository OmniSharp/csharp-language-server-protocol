using System.Collections.Generic;
using JsonRpc;
using JsonRpc.Server;
using Lsp.Messages;
using Lsp.Protocol;
using Newtonsoft.Json.Linq;

namespace Lsp
{
    class LspReciever : Reciever
    {
        private bool _initialized;

        public override (IEnumerable<Renor> results, bool hasResponse) GetRequests(JToken container)
        {
            if (_initialized) return base.GetRequests(container);

            var newResults = new List<Renor>();

            // Based on https://github.com/Microsoft/language-server-protocol/blob/master/protocol.md#initialize-request
            var (results, hasResponse) = base.GetRequests(container);
            foreach (var item in results)
            {
                if (item.IsRequest && item.Request.Method == LspHelper.GetMethodName<IInitializeHandler>())
                {
                    newResults.Add(item);
                }
                else if (item.IsRequest)
                {
                    newResults.Add(new ServerNotInitialized());
                }
                else if (item.IsResponse)
                {
                    newResults.Add(item);
                }
            }

            return (newResults, hasResponse);
        }

        public void Initialized()
        {
            _initialized = true;
        }
    }
}