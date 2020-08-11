namespace OmniSharp.Extensions.JsonRpc
{
    internal class JsonRpcServerRegistry : InterimJsonRpcServerRegistry<IJsonRpcServerRegistry>, IJsonRpcServerRegistry
    {
        public JsonRpcServerRegistry(CompositeHandlersManager handlersManager) : base(handlersManager)
        {
        }
    }
}
