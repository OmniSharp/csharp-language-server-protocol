using System;

namespace JsonRpc
{
    public interface IOutputHandler : IDisposable
    {
        void Start();
        void Send(object value);
    }
}