using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Server;
using Xunit;

namespace JsonRpc.Tests;

public sealed class RequestInvocationHandleTests
{
    [Fact]
    public void MultipleSubscribersAreDeliveredOnceDuringConcurrentDispose()
    {
        var handle = Handle();
        var calls = new int[20];
        for (var i = 0; i < calls.Length; i++)
        {
            var index = i;
            handle.OnComplete += _ => Interlocked.Increment(ref calls[index]);
        }
        Parallel.For(0, 100, _ => handle.Dispose());
        Assert.All(calls, count => Assert.Equal(1, count));
    }

    [Fact]
    public void CallbackRunsOutsideCompletionLock()
    {
        var handle = Handle();
        using var workerCompleted = new ManualResetEventSlim();
        handle.OnComplete += _ =>
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                Action<Request> callback = _ => { };
                handle.OnComplete += callback;
                handle.OnComplete -= callback;
                workerCompleted.Set();
            });
            Assert.True(workerCompleted.Wait(TimeSpan.FromSeconds(5)),
                "Another thread must be able to subscribe while the completion callback is running.");
        };
        handle.Dispose();
    }

    [Fact]
    public void CompletionPreservesCancellationTokenLifetime()
    {
        var handle = Handle();
        handle.Dispose();
        var calls = 0;
        using var registration = handle.CancellationTokenSource.Token.Register(() => calls++);
        handle.CancellationTokenSource.Cancel();
        Assert.Equal(1, calls);
        Assert.True(handle.CancellationTokenSource.IsCancellationRequested);
    }

    private static RequestInvocationHandle Handle() => new(new Request(1L, "probe", null));

    [Fact]
    public void LateSubscriberReceivesCompletion()
    {
        var h = Handle();
        h.Dispose();
        int calls = 0;
        h.OnComplete += _ => calls++;
        Assert.Equal(1, calls);
    }

    [Fact]
    public void EarlySubscriberReceivesCompletionOnce()
    {
        var h = Handle();
        int calls = 0;
        h.OnComplete += _ => calls++;
        Parallel.For(0, 100, _ => h.Dispose());
        Assert.Equal(1, calls);
    }

    [Fact]
    public void RemovedSubscriberNotCalled()
    {
        var h = Handle();
        int calls = 0;
        Action<Request> callback = _ => calls++;
        h.OnComplete += callback;
        h.OnComplete -= callback;
        h.Dispose();
        Assert.Equal(0, calls);
    }

    [Fact]
    public void ReentrantLateSubscriptionIsDelivered()
    {
        var h = Handle();
        int calls = 0;
        h.OnComplete += _ =>
        {
            calls++;
            h.OnComplete += _ => calls++;
        };
        h.Dispose();
        Assert.Equal(2, calls);
    }

    [Fact]
    public void SubscribeAndDisposeRaceDoesNotLoseCompletion()
    {
        for (int i = 0; i < 500; i++)
        {
            var h = Handle();
            int calls = 0;
            Parallel.Invoke(() => h.OnComplete += _ => Interlocked.Increment(ref calls), h.Dispose);
            Assert.Equal(1, calls);
        }
    }

    [Fact]
    public async Task RealInputHandlerReleasesSynchronouslyCompletedRequestsAsync()
    {
        var invoker = new ProbeInvoker(true);
        var residual = await RunAsync(invoker, Enumerable.Range(1, 256).Select(
            i => $"{{\"jsonrpc\":\"2.0\",\"id\":{i},\"method\":\"probe\",\"params\":{{\"sample\":{i}}}}}"));
        Assert.Equal(256, invoker.Handles.Count);
        Assert.Equal(0, residual);
    }

    [Fact]
    public async Task RealInputHandlerCancellationRemainsEffectiveAsync()
    {
        var invoker = new ProbeInvoker(false);
        var residual = await RunAsync(invoker, new[]
        {
            "{\"jsonrpc\":\"2.0\",\"id\":1,\"method\":\"probe\"}",
            "{\"jsonrpc\":\"2.0\",\"method\":\"$/cancelRequest\",\"params\":{\"id\":1}}"
        });
        Assert.Equal(1, invoker.Cancelled);
        Assert.Equal(0, residual);
    }

    private static async Task<int> RunAsync(ProbeInvoker invoker, IEnumerable<string> messages)
    {
        var pipe = new Pipe();
        var router = Substitute.For<IRequestRouter<IHandlerDescriptor?>>();
        var descriptor = Substitute.For<IRequestDescriptor<IHandlerDescriptor?>>();
        descriptor.Default.Returns(Substitute.For<IHandlerDescriptor>());
        router.GetDescriptors(Arg.Any<Request>()).Returns(descriptor);
        using var handler = new InputHandler(pipe.Reader, Substitute.For<IOutputHandler>(), new Receiver(), router,
            Substitute.For<IResponseRouter>(), invoker, NullLoggerFactory.Instance, ex => throw ex, null);
        handler.Start();
        foreach (var message in messages)
        {
            var bytes = Encoding.UTF8.GetBytes(message);
            await pipe.Writer.WriteAsync(Encoding.UTF8.GetBytes($"Content-Length: {bytes.Length}\r\n\r\n{message}"));
        }
        await pipe.Writer.CompleteAsync();
        await handler.InputCompleted.WaitAsync(TimeSpan.FromSeconds(5));
        // Observe the real InputHandler registry after all messages have been processed.
        var dictionary = (ConcurrentDictionary<object, RequestInvocationHandle>)typeof(InputHandler)
            .GetField("_requests", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(handler)!;
        return dictionary.Count;
    }

    private sealed class ProbeInvoker(bool immediate) : RequestInvoker
    {
        public List<RequestInvocationHandle> Handles { get; } = [];
        public int Cancelled;

        public override RequestInvocationHandle InvokeRequest(IRequestDescriptor<IHandlerDescriptor?> descriptor, Request request)
        {
            var h = new RequestInvocationHandle(request);
            Handles.Add(h);
            if (immediate) h.Dispose();
            else h.CancellationTokenSource.Token.Register(() =>
            {
                Cancelled++;
                h.Dispose();
            });
            return h;
        }

        public override void InvokeNotification(IRequestDescriptor<IHandlerDescriptor?> descriptor, Notification notification) { }
        public override void Dispose() { }
    }
}
