namespace OmniSharp.Extensions.JsonRpc
{
    public interface IOutputHandler : IDisposable
    {
        void Send(object? value);
        Task StopAsync();
    }
}
