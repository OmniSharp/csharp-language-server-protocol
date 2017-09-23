using System;

namespace OmniSharp.Extensions.JsonRpc
{
    public interface IInputHandler : IDisposable
    {
        void Start();
    }
}