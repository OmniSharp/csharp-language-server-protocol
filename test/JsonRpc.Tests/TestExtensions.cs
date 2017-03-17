using System.Threading;

namespace JsonRpc.Tests
{
    public static class TestExtensions
    {
        public static void Wait(this CancellationTokenSource cancellationTokenSource)
        {
            cancellationTokenSource.Token.WaitHandle.WaitOne();
        }
    }
}