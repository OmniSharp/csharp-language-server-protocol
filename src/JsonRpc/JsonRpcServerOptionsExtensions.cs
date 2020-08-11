namespace OmniSharp.Extensions.JsonRpc
{
    public static class JsonRpcServerOptionsExtensions
    {
        public static JsonRpcServerOptions WithSerializer(this JsonRpcServerOptions options, ISerializer serializer)
        {
            options.Serializer = serializer;
            return options;
        }
    }
}
