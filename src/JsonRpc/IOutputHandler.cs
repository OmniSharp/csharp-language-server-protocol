using System;

namespace OmniSharp.Extensions.JsonRpc
{
    public interface IOutputHandler : IDisposable
    {
        void Start();
        void Send(object value);
    }
}