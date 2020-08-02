using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Serialization;
using OmniSharp.Extensions.JsonRpc.Server;
using Xunit;
using Xunit.Abstractions;

namespace JsonRpc.Tests
{
    public class TestLanguageServerRegistry : JsonRpcCommonMethodsBase<IJsonRpcServerRegistry>, IJsonRpcServerRegistry
    {
        private List<IJsonRpcHandler> Handlers { get; set; } = new List<IJsonRpcHandler>();
        private List<(string name, IJsonRpcHandler handler)> NamedHandlers { get; set; } = new List<(string name, IJsonRpcHandler handler)>();

        private List<(string name, Func<IServiceProvider, IJsonRpcHandler> handlerFunc)> NamedServiceHandlers { get; set; } =
            new List<(string name, Func<IServiceProvider, IJsonRpcHandler> handlerFunc)>();

        public TestLanguageServerRegistry()
        {
        }

        public override IJsonRpcServerRegistry AddHandler(string method, IJsonRpcHandler handler, JsonRpcHandlerOptions options = null)
        {
            NamedHandlers.Add((method, handler));
            return this;
        }

        public override IJsonRpcServerRegistry AddHandler<THandler>(THandler handler, JsonRpcHandlerOptions options = null) => throw new NotImplementedException();

        public override IJsonRpcServerRegistry AddHandler<T>(JsonRpcHandlerOptions options)
        {
            throw new NotImplementedException();
        }

        public override IJsonRpcServerRegistry AddHandler<TTHandler>(string method, JsonRpcHandlerOptions options = null) => throw new NotImplementedException();

        public override IJsonRpcServerRegistry AddHandler(Type type, JsonRpcHandlerOptions options = null) => throw new NotImplementedException();

        public override IJsonRpcServerRegistry AddHandler(string method, Type type, JsonRpcHandlerOptions options = null) => throw new NotImplementedException();

        public override IJsonRpcServerRegistry AddHandler(string method, Func<IServiceProvider, IJsonRpcHandler> handlerFunc, JsonRpcHandlerOptions options = null)
        {
            NamedServiceHandlers.Add((method, handlerFunc));
            return this;
        }

        public override IJsonRpcServerRegistry AddHandlers(params IJsonRpcHandler[] handlers)
        {
            Handlers.AddRange(handlers);
            return this;
        }

        public override IJsonRpcServerRegistry AddHandler<THandler>(Func<IServiceProvider, THandler> handlerFunc, JsonRpcHandlerOptions options = null) => throw new NotImplementedException();

        public void Populate(HandlerCollection collection, IServiceProvider serviceProvider, JsonRpcHandlerOptions options = null)
        {
            collection.Add(Handlers.ToArray());
            foreach (var (name, handler) in NamedHandlers)
            {
                collection.Add(name, handler, options);
            }

            foreach (var (name, handlerFunc) in NamedServiceHandlers)
            {
                collection.Add(name, handlerFunc(serviceProvider), options);
            }
        }
    }

    public class RequestRouterTests : AutoTestBase
    {
        public RequestRouterTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            Services
                .AddJsonRpcMediatR(new[] {typeof(RequestRouterTests).Assembly})
                .AddSingleton<ISerializer>(new JsonRpcSerializer());
        }

        [Fact]
        public async Task ShouldRoute_CustomRequestResponse()
        {
            var collection = new HandlerCollection(Enumerable.Empty<IJsonRpcHandler>()) { };
            var registry = new TestLanguageServerRegistry();
            AutoSubstitute.Provide(collection);
            AutoSubstitute.Provide<IEnumerable<IHandlerDescriptor>>(collection);
            var mediator = AutoSubstitute.Resolve<RequestRouter>();
            var method = Substitute.For<Func<string, Task<long>>>();
            method.Invoke(Arg.Any<string>()).Returns(1000L);
            registry.OnRequest<string, long>("$/my/something/awesome", method);

            registry.Populate(collection, ServiceProvider);

            var request = new Request(Guid.NewGuid().ToString(), "$/my/something/awesome", "123123123123");
            await mediator.RouteRequest(mediator.GetDescriptors(request), request, "123123123123", CancellationToken.None);

            await method.Received(1).Invoke(Arg.Any<string>());
        }

        [Fact]
        public async Task ShouldRoute_CustomRequest()
        {
            var collection = new HandlerCollection(Enumerable.Empty<IJsonRpcHandler>()) { };
            var registry = new TestLanguageServerRegistry();
            AutoSubstitute.Provide(collection);
            AutoSubstitute.Provide<IEnumerable<IHandlerDescriptor>>(collection);
            var mediator = AutoSubstitute.Resolve<RequestRouter>();
            var method = Substitute.For<Func<string, Task>>();
            method.Invoke(Arg.Any<string>()).Returns(Task.CompletedTask);
            registry.OnRequest<string>("$/my/something/awesome", method);

            registry.Populate(collection, ServiceProvider);

            var request = new Request(Guid.NewGuid().ToString(), "$/my/something/awesome", "123123123123");
            await mediator.RouteRequest(mediator.GetDescriptors(request), request, "123123123123", CancellationToken.None);

            await method.Received(1).Invoke(Arg.Any<string>());
        }

        [Fact]
        public async Task ShouldRoute_CustomNotification()
        {
            var collection = new HandlerCollection(Enumerable.Empty<IJsonRpcHandler>()) { };
            var registry = new TestLanguageServerRegistry();
            AutoSubstitute.Provide(collection);
            AutoSubstitute.Provide<IEnumerable<IHandlerDescriptor>>(collection);
            var mediator = AutoSubstitute.Resolve<RequestRouter>();
            var method = Substitute.For<Action<string>>();
            registry.OnNotification<string>("$/my/something/awesome", method);

            registry.Populate(collection, ServiceProvider);

            var notification = new Notification("$/my/something/awesome", "123123123123");
            await mediator.RouteNotification(mediator.GetDescriptors(notification), notification, "123123123123", CancellationToken.None);

            method.Received(1).Invoke(Arg.Any<string>());
        }

        [Fact]
        public async Task ShouldRoute_CustomEmptyNotification()
        {
            var collection = new HandlerCollection(Enumerable.Empty<IJsonRpcHandler>()) { };
            var registry = new TestLanguageServerRegistry();
            AutoSubstitute.Provide(collection);
            AutoSubstitute.Provide<IEnumerable<IHandlerDescriptor>>(collection);
            var mediator = AutoSubstitute.Resolve<RequestRouter>();
            var method = Substitute.For<Action>();
            registry.OnNotification("$/my/something/awesome", method);

            registry.Populate(collection, ServiceProvider);

            var notification = new Notification("$/my/something/awesome", null);
            await mediator.RouteNotification(mediator.GetDescriptors(notification), notification, new object(), CancellationToken.None);

            method.Received(1).Invoke();
        }
    }
}
