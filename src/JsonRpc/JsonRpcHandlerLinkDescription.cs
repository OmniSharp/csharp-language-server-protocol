namespace OmniSharp.Extensions.JsonRpc
{
    public class JsonRpcHandlerLinkDescription : JsonRpcHandlerDescription
    {
        public JsonRpcHandlerLinkDescription(string method, string linkToMethod) : base(new JsonRpcHandlerOptions())
        {
            Method = method;
            LinkToMethod = linkToMethod;
        }

        public string Method { get; }
        public string LinkToMethod { get; }
    }
}
