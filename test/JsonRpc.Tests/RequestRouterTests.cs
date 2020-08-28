using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DryIoc;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Server;
using Xunit;
using Xunit.Abstractions;
using Arg = NSubstitute.Arg;
using Request = OmniSharp.Extensions.JsonRpc.Server.Request;

namespace JsonRpc.Tests
{
    public class RequestRouterTests : AutoTestBase
    {
        public RequestRouterTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper) => Container = JsonRpcTestContainer.Create(testOutputHelper);

        [Fact]
        public async Task ShouldRoute_CustomRequestResponse()
        {
            var collection = new HandlerCollection(new Container(), new HandlerTypeDescriptorProvider(new [] { typeof(HandlerTypeDescriptorProvider).Assembly, typeof(HandlerResolverTests).Assembly }));
            var registry = new TestLanguageServerRegistry();
            AutoSubstitute.Provide<IHandlersManager>(collection);
            AutoSubstitute.Provide<IEnumerable<IHandlerDescriptor>>(collection);
            var mediator = AutoSubstitute.Resolve<RequestRouter>();
            var method = Substitute.For<Func<string, Task<long>>>();
            method.Invoke(Arg.Any<string>()).Returns(1000L);
            registry.OnRequest("$/my/something/awesome", method);

            registry.Populate(collection, ServiceProvider);

            var request = new Request(Guid.NewGuid().ToString(), "$/my/something/awesome", "123123123123");
            await mediator.RouteRequest(mediator.GetDescriptors(request), request, CancellationToken.None);

            await method.Received(1).Invoke(Arg.Any<string>());
        }

        [Fact]
        public async Task ShouldRoute_CustomRequest()
        {
            var collection = new HandlerCollection(new Container(), new HandlerTypeDescriptorProvider(new [] { typeof(HandlerTypeDescriptorProvider).Assembly, typeof(HandlerResolverTests).Assembly }));
            var registry = new TestLanguageServerRegistry();
            AutoSubstitute.Provide<IHandlersManager>(collection);
            AutoSubstitute.Provide<IEnumerable<IHandlerDescriptor>>(collection);
            var mediator = AutoSubstitute.Resolve<RequestRouter>();
            var method = Substitute.For<Func<string, Task>>();
            method.Invoke(Arg.Any<string>()).Returns(Task.CompletedTask);
            registry.OnRequest("$/my/something/awesome", method);

            registry.Populate(collection, ServiceProvider);

            var request = new Request(Guid.NewGuid().ToString(), "$/my/something/awesome", "123123123123");
            await mediator.RouteRequest(mediator.GetDescriptors(request), request, CancellationToken.None);

            await method.Received(1).Invoke(Arg.Any<string>());
        }

        [Fact]
        public async Task ShouldRoute_CustomNotification()
        {
            var collection = new HandlerCollection(new Container(), new HandlerTypeDescriptorProvider(new [] { typeof(HandlerTypeDescriptorProvider).Assembly, typeof(HandlerResolverTests).Assembly }));
            var registry = new TestLanguageServerRegistry();
            AutoSubstitute.Provide<IHandlersManager>(collection);
            AutoSubstitute.Provide<IEnumerable<IHandlerDescriptor>>(collection);
            var mediator = AutoSubstitute.Resolve<RequestRouter>();
            var method = Substitute.For<Action<string>>();
            registry.OnNotification("$/my/something/awesome", method);

            registry.Populate(collection, ServiceProvider);

            var notification = new Notification("$/my/something/awesome", "123123123123");
            await mediator.RouteNotification(mediator.GetDescriptors(notification), notification, CancellationToken.None);

            method.Received(1).Invoke(Arg.Any<string>());
        }

        [Fact]
        public async Task ShouldRoute_CustomEmptyNotification()
        {
            var collection = new HandlerCollection(new Container(), new HandlerTypeDescriptorProvider(new [] { typeof(HandlerTypeDescriptorProvider).Assembly, typeof(HandlerResolverTests).Assembly }));
            var registry = new TestLanguageServerRegistry();
            AutoSubstitute.Provide<IHandlersManager>(collection);
            AutoSubstitute.Provide<IEnumerable<IHandlerDescriptor>>(collection);
            var mediator = AutoSubstitute.Resolve<RequestRouter>();
            var method = Substitute.For<Action>();
            registry.OnNotification("$/my/something/awesome", method);

            registry.Populate(collection, ServiceProvider);

            var notification = new Notification("$/my/something/awesome", null);
            await mediator.RouteNotification(mediator.GetDescriptors(notification), notification, CancellationToken.None);

            method.Received(1).Invoke();
        }
    }
}
