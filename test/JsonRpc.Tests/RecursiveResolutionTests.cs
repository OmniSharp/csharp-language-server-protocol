using System;
using System.Threading;
using System.Threading.Tasks;
using DryIoc;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Testing;
using Xunit;
using Xunit.Abstractions;

namespace JsonRpc.Tests
{
    public class RecursiveResolutionTests : JsonRpcServerTestBase
    {
        public RecursiveResolutionTests(ITestOutputHelper testOutputHelper) : base(new JsonRpcTestOptions().ConfigureForXUnit(testOutputHelper))
        {
        }

        [Fact(Skip = "Fails windows CI")]
        public async Task Server_Can_Be_Injected_Into_Handler_After_Creation_Using_Registration()
        {
            Func<Task> a = async () => {
                var (client, server) = await Initialize(
                    options => { },
                    options => { }
                );
                server.Register(
                    r => r
                        .AddHandler<InterfaceHandler<IJsonRpcServer>>()
                        .AddHandler<ClassHandler<JsonRpcServer>>()
                );
            };
            await a.Should().NotThrowAsync();
        }

        [Fact]
        public void Server_Cannot_Be_Injected_Into_Handler_During_Creation_Using_Registration()
        {
            Func<Task> a = () => Initialize(
                options => { },
                options => options
                          .AddHandler<InterfaceHandler<IJsonRpcServer>>()
                          .AddHandler<ClassHandler<JsonRpcServer>>()
            );
            var result = a.Should().Throw<ContainerException>();
            result.And.ErrorName.Should().Be("UnableToResolveFromRegisteredServices");
        }

        [Fact]
        public void Server_Cannot_Be_Injected_Into_Handler_During_Creation_Using_Description()
        {
            Func<Task> a = () => Initialize(
                options => { },
                options => options.Services
                                  .AddSingleton(JsonRpcHandlerDescription.Infer(typeof(InterfaceHandler<IJsonRpcServer>)))
                                  .AddSingleton(JsonRpcHandlerDescription.Infer(typeof(ClassHandler<JsonRpcServer>)))
            );
            var result = a.Should().Throw<ContainerException>();
            result.And.ErrorName.Should().Be("UnableToResolveFromRegisteredServices");
        }

        [Fact]
        public void Server_Cannot_Be_Injected_Into_Handler_During_Creation_Using_Injection()
        {
            Func<Task> a = () => Initialize(
                options => { },
                options => options.Services
                                  .AddSingleton<InterfaceHandler<IJsonRpcServer>>()
                                  .AddSingleton<ClassHandler<JsonRpcServer>>()
            );
            var result = a.Should().Throw<ContainerException>();
            result.And.ErrorName.Should().Be("UnableToResolveFromRegisteredServices");
        }

        [Fact(Skip = "Fails windows CI")]
        public async Task Server_Facade_Can_Be_Injected_Into_Handler_During_Creation_Using_Registration()
        {
            Func<Task> a = () => Initialize(
                options => { },
                options => options
                          .AddHandler<ClassHandler<IJsonRpcServerFacade>>()
            );
            await a.Should().NotThrowAsync();
        }

        [Fact(Skip = "Fails windows CI")]
        public async Task Server_Facade_Can_Be_Injected_Into_Handler_During_Creation_Using_Description()
        {
            Func<Task> a = () => Initialize(
                options => { },
                options => options.Services
                                  .AddSingleton(JsonRpcHandlerDescription.Infer(typeof(ClassHandler<IJsonRpcServerFacade>)))
            );
            await a.Should().NotThrowAsync();
        }

        [Fact(Skip = "Fails windows CI")]
        public async Task Server_Facade_Can_Injected_Into_Handler_During_Creation_Using_Injection()
        {
            Func<Task> a = () => Initialize(
                options => { },
                options => options.Services
                                  .AddSingleton<ClassHandler<IJsonRpcServerFacade>>()
            );
            await a.Should().NotThrowAsync();
        }

        [Method(nameof(ClassRequest))]
        public class ClassRequest : IRequest<Unit>
        {
        }

        [Method(nameof(ClassRequest))]
        class ClassHandler<T> : IJsonRpcRequestHandler<ClassRequest, Unit> where T : IJsonRpcServerFacade
        {
            private readonly T _jsonRpcServer;

            public ClassHandler(T jsonRpcServer)
            {
                _jsonRpcServer = jsonRpcServer;
            }

            public Task<Unit> Handle(ClassRequest classRequest, CancellationToken cancellationToken) => throw new NotImplementedException();
        }

        [Method(nameof(InterfaceRequest))]
        public class InterfaceRequest : IRequest<Unit>
        {
        }

        [Method(nameof(InterfaceRequest))]
        class InterfaceHandler<T> : IJsonRpcRequestHandler<InterfaceRequest, Unit> where T : IJsonRpcServerFacade
        {
            private readonly T _jsonRpcServer;

            public InterfaceHandler(T jsonRpcServer)
            {
                _jsonRpcServer = jsonRpcServer;
            }

            public Task<Unit> Handle(InterfaceRequest request, CancellationToken cancellationToken) => throw new NotImplementedException();
        }
    }
}
