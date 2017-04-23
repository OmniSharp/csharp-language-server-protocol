using System;

namespace JsonRpc
{
    public interface IInputHandler : IDisposable
    {
        void Start();
    }
}