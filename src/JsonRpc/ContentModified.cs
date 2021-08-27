using OmniSharp.Extensions.JsonRpc.Server;
using OmniSharp.Extensions.JsonRpc.Server.Messages;

namespace OmniSharp.Extensions.JsonRpc
{
    public class ContentModified : RpcError
    {
        public ContentModified(string method) : base(null, method, new ErrorMessage(ErrorCodes.ContentModified, "Content Modified"))
        {
        }

        public ContentModified(object id, string method) : base(id, method, new ErrorMessage(ErrorCodes.ContentModified, "Content Modified"))
        {
        }
    }
}
