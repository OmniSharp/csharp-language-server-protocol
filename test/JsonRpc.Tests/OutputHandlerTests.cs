using System;
using System.IO;
using System.Threading;
using NSubstitute;
using Xunit;

namespace JsonRpc.Tests
{
    public class OutputHandlerTests
    {
        private static (OutputHandler handler, Action wait) NewHandler(TextWriter textWriter,Action<CancellationTokenSource> action)
        {
            var cts = new CancellationTokenSource();
            if (!System.Diagnostics.Debugger.IsAttached)
                cts.CancelAfter(TimeSpan.FromSeconds(5));
            action(cts);

            var handler = new OutputHandler(textWriter);
            handler.Start();
            return (handler, () => cts.Wait());
        }

        [Fact]
        public void ShouldSerializeValues()
        {
            var tw = Substitute.For<TextWriter>();

            var (handler, wait) = NewHandler(tw, cts => {
                tw.When(x => x.Write(Arg.Any<string>()))
                    .Do(c => cts.Cancel());
            });
            var value = new JsonRpc.Client.Response(1);

            using (handler)
            {

                handler.Send(value);
                wait();

                tw.Received().Write("Content-Length: 46\r\n\r\n{\"protocolVersion\":\"2.0\",\"id\":1,\"result\":null}");
            }
        }
    }
}