using OmniSharp.Extensions.JsonRpc.Server;

namespace OmniSharp.Extensions.JsonRpc
{
    public delegate Exception CreateResponseExceptionHandler(ServerError serverError, string message);
}
