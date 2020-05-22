namespace OmniSharp.Extensions.JsonRpc.Testing
{
    public interface IRequestSettler
    {
        void OnStartRequest();
        void OnEndRequest();
    }
}