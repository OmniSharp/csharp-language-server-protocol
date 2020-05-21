using System;
using System.Threading.Tasks;

namespace OmniSharp.Extensions.JsonRpc
{
    public interface IOutputHandler : IDisposable
    {
        void Send(object value);
        Task WaitForShutdown();
    }
}
