using System.Threading;

namespace Lsp.Tests
{
    public static class TestExtensions
    {
        public static void Wait(this CancellationTokenSource cancellationTokenSource)
        {
            cancellationTokenSource.Token.WaitHandle.WaitOne();
        }
    }
}