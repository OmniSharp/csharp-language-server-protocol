using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Server;
using Xunit;
using Xunit.Abstractions;
using System.Reactive.Disposables;
using OmniSharp.Extensions.JsonRpc.Serialization;

namespace Lsp.Tests
{
    public class TestLanguageServerRegistry : IJsonRpcHandlerRegistry
    {
        private List<IJsonRpcHandler> Handlers { get; set; } = new List<IJsonRpcHandler>();
        private List<(string name, IJsonRpcHandler handler)> NamedHandlers { get; set; } = new List<(string name, IJsonRpcHandler handler)>();
        private List<(string name, Func<IServiceProvider, IJsonRpcHandler> handlerFunc)> NamedServiceHandlers { get; set; } = new List<(string name, Func<IServiceProvider, IJsonRpcHandler> handlerFunc)>();

        public TestLanguageServerRegistry()
        {
        }

        public IDisposable AddHandler(string method, IJsonRpcHandler handler)
        {
            NamedHandlers.Add((method, handler));
            return Disposable.Empty;
        }

        public IDisposable AddHandler<T>() where T : IJsonRpcHandler
        {
            throw new NotImplementedException();
        }

        public IDisposable AddHandler(string method, Func<IServiceProvider, IJsonRpcHandler> handlerFunc)
        {
            NamedServiceHandlers.Add((method, handlerFunc));
            return Disposable.Empty;
        }

        public IDisposable AddHandlers(params IJsonRpcHandler[] handlers)
        {
            Handlers.AddRange(handlers);
            return Disposable.Empty;
        }

        public void Populate(HandlerCollection collection, IServiceProvider serviceProvider)
        {
            collection.Add(Handlers.ToArray());
            foreach (var (name, handler) in NamedHandlers)
            {
                collection.Add(name, handler);
            }
            foreach (var (name, handlerFunc) in NamedServiceHandlers)
            {
                collection.Add(name, handlerFunc(serviceProvider));
            }
        }
    }
    public class RequestRouterTests : AutoTestBase
    {
        public RequestRouterTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            Services
                .AddJsonRpcMediatR(new[] { typeof(RequestRouterTests).Assembly })
                .AddSingleton<ISerializer>(new JsonRpcSerializer());
        }

        [Fact]
        public async Task ShouldRoute_CustomRequestResponse()
        {
            var collection = new HandlerCollection() { };
            var registry = new TestLanguageServerRegistry();
            AutoSubstitute.Provide(collection);
            AutoSubstitute.Provide<IEnumerable<IHandlerDescriptor>>(collection);
            var mediator = AutoSubstitute.Resolve<RequestRouter>();
            var method = Substitute.For<Func<string, Task<long>>>();
            method.Invoke(Arg.Any<string>()).Returns(1000L);
            registry.OnRequest<string, long>("$/my/something/awesome", method);

            registry.Populate(collection, ServiceProvider);

            var request = new Request(Guid.NewGuid().ToString(), "$/my/something/awesome", "123123123123");
            await mediator.RouteRequest(mediator.GetDescriptor(request), request, CancellationToken.None);

            await method.Received(1).Invoke(Arg.Any<string>());
        }

        [Fact]
        public async Task ShouldRoute_CustomRequest()
        {
            var collection = new HandlerCollection() { };
            var registry = new TestLanguageServerRegistry();
            AutoSubstitute.Provide(collection);
            AutoSubstitute.Provide<IEnumerable<IHandlerDescriptor>>(collection);
            var mediator = AutoSubstitute.Resolve<RequestRouter>();
            var method = Substitute.For<Func<string, Task>>();
            method.Invoke(Arg.Any<string>()).Returns(Task.CompletedTask);
            registry.OnRequest<string>("$/my/something/awesome", method);

            registry.Populate(collection, ServiceProvider);

            var request = new Request(Guid.NewGuid().ToString(), "$/my/something/awesome", "123123123123");
            await mediator.RouteRequest(mediator.GetDescriptor(request), request, CancellationToken.None);

            await method.Received(1).Invoke(Arg.Any<string>());
        }

        [Fact]
        public async Task ShouldRoute_CustomNotification()
        {
            var collection = new HandlerCollection() { };
            var registry = new TestLanguageServerRegistry();
            AutoSubstitute.Provide(collection);
            AutoSubstitute.Provide<IEnumerable<IHandlerDescriptor>>(collection);
            var mediator = AutoSubstitute.Resolve<RequestRouter>();
            var method = Substitute.For<Action<string>>();
            registry.OnNotification<string>("$/my/something/awesome", method);

            registry.Populate(collection, ServiceProvider);

            var notification = new Notification("$/my/something/awesome", "123123123123");
            await mediator.RouteNotification(mediator.GetDescriptor(notification), notification, CancellationToken.None);

            method.Received(1).Invoke(Arg.Any<string>());
        }

        [Fact]
        public async Task ShouldRoute_CustomEmptyNotification()
        {
            var collection = new HandlerCollection() { };
            var registry = new TestLanguageServerRegistry();
            AutoSubstitute.Provide(collection);
            AutoSubstitute.Provide<IEnumerable<IHandlerDescriptor>>(collection);
            var mediator = AutoSubstitute.Resolve<RequestRouter>();
            var method = Substitute.For<Action>();
            registry.OnNotification("$/my/something/awesome", method);

            registry.Populate(collection, ServiceProvider);

            var notification = new Notification("$/my/something/awesome", null);
            await mediator.RouteNotification(mediator.GetDescriptor(notification), notification, CancellationToken.None);

            method.Received(1).Invoke();
        }
    }
}
