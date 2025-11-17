using System.Reactive.Disposables;
using FluentAssertions;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc;
using Xunit.Abstractions;

namespace JsonRpc.Tests
{
    public class CompositeHandlersManagerTests : AutoTestBase
    {
        public CompositeHandlersManagerTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Fact]
        public void Should_Add_Handler_Instance_To_Parent()
        {
            var parent = Substitute.For<IHandlersManager>();
            parent.Add(Arg.Any<IJsonRpcHandler>(), Arg.Any<JsonRpcHandlerOptions>()).Returns(Disposable.Empty);

            var manager = new CompositeHandlersManager(parent);
            manager.Add(Substitute.For<IJsonRpcHandler>(), new JsonRpcHandlerOptions());

            parent.Received(0).Add(Arg.Any<string>(), Arg.Any<IJsonRpcHandler>(), Arg.Any<JsonRpcHandlerOptions>());
            parent.Received(1).Add(Arg.Any<IJsonRpcHandler>(), Arg.Any<JsonRpcHandlerOptions>());
            manager.GetDisposable().Count.Should().Be(1);
        }

        [Fact]
        public void Should_Add_Named_Handler_Instance_To_Parent()
        {
            var parent = Substitute.For<IHandlersManager>();
            parent.Add(Arg.Any<string>(), Arg.Any<IJsonRpcHandler>(), Arg.Any<JsonRpcHandlerOptions>()).Returns(Disposable.Empty);

            var manager = new CompositeHandlersManager(parent);
            manager.Add("mymethod", Substitute.For<IJsonRpcHandler>(), new JsonRpcHandlerOptions());

            parent.Received(0).Add(Arg.Any<IJsonRpcHandler>(), Arg.Any<JsonRpcHandlerOptions>());
            parent.Received(1).Add("mymethod", Arg.Any<IJsonRpcHandler>(), Arg.Any<JsonRpcHandlerOptions>());
            manager.GetDisposable().Count.Should().Be(1);
        }

        [Fact]
        public void Should_Add_Handler_Factory_To_Parent()
        {
            var parent = Substitute.For<IHandlersManager>();
            parent.Add(Arg.Any<JsonRpcHandlerFactory>(), Arg.Any<JsonRpcHandlerOptions>()).Returns(Disposable.Empty);

            var manager = new CompositeHandlersManager(parent);
            manager.Add(Substitute.For<JsonRpcHandlerFactory>(), new JsonRpcHandlerOptions());

            parent.Received(0).Add(Arg.Any<string>(), Arg.Any<JsonRpcHandlerFactory>(), Arg.Any<JsonRpcHandlerOptions>());
            parent.Received(1).Add(Arg.Any<JsonRpcHandlerFactory>(), Arg.Any<JsonRpcHandlerOptions>());
            manager.GetDisposable().Count.Should().Be(1);
        }

        [Fact]
        public void Should_Add_Named_Handler_Factory_To_Parent()
        {
            var parent = Substitute.For<IHandlersManager>();
            parent.Add(Arg.Any<string>(), Arg.Any<JsonRpcHandlerFactory>(), Arg.Any<JsonRpcHandlerOptions>()).Returns(Disposable.Empty);

            var manager = new CompositeHandlersManager(parent);
            manager.Add("mymethod", Substitute.For<JsonRpcHandlerFactory>(), new JsonRpcHandlerOptions());

            parent.Received(0).Add(Arg.Any<JsonRpcHandlerFactory>(), Arg.Any<JsonRpcHandlerOptions>());
            parent.Received(1).Add("mymethod", Arg.Any<JsonRpcHandlerFactory>(), Arg.Any<JsonRpcHandlerOptions>());
            manager.GetDisposable().Count.Should().Be(1);
        }

        [Fact]
        public void Should_Add_Handler_Type_To_Parent()
        {
            var parent = Substitute.For<IHandlersManager>();
            parent.Add(Arg.Any<Type>(), Arg.Any<JsonRpcHandlerOptions>()).Returns(Disposable.Empty);

            var manager = new CompositeHandlersManager(parent);
            manager.Add(Substitute.For<Type>(), new JsonRpcHandlerOptions());

            parent.Received(0).Add(Arg.Any<string>(), Arg.Any<Type>(), Arg.Any<JsonRpcHandlerOptions>());
            parent.Received(1).Add(Arg.Any<Type>(), Arg.Any<JsonRpcHandlerOptions>());
            manager.GetDisposable().Count.Should().Be(1);
        }

        [Fact]
        public void Should_Add_Named_Handler_Type_To_Parent()
        {
            var parent = Substitute.For<IHandlersManager>();
            parent.Add(Arg.Any<string>(), Arg.Any<Type>(), Arg.Any<JsonRpcHandlerOptions>()).Returns(Disposable.Empty);

            var manager = new CompositeHandlersManager(parent);
            manager.Add("mymethod", Substitute.For<Type>(), new JsonRpcHandlerOptions());

            parent.Received(0).Add(Arg.Any<Type>(), Arg.Any<JsonRpcHandlerOptions>());
            parent.Received(1).Add("mymethod", Arg.Any<Type>(), Arg.Any<JsonRpcHandlerOptions>());
            manager.GetDisposable().Count.Should().Be(1);
        }
    }
}
