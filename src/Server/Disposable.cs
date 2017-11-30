using System;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    class Disposable : IDisposable
    {
        private readonly Action _action;

        public Disposable(Action action)
        {
            _action = action;
        }

        public static implicit operator Disposable(Action action)
        {
            return new Disposable(action);
        }

        public void Dispose()
        {
            _action();
        }
    }
}
