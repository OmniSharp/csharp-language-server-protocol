using System;
using System.Threading;
using System.Threading.Tasks;
using DryIoc;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OmniSharp.Extensions.DebugAdapter.Client;
using OmniSharp.Extensions.DebugAdapter.Protocol.Client;
using OmniSharp.Extensions.DebugAdapter.Protocol.Server;
using OmniSharp.Extensions.DebugAdapter.Server;
using OmniSharp.Extensions.DebugAdapter.Testing;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Testing;
using TestingUtils;
using Xunit;
using Xunit.Abstractions;

namespace Dap.Tests.Integration
{
    public class RecursiveResolutionTests : DebugAdapterProtocolTestBase
    {
        public RecursiveResolutionTests(ITestOutputHelper testOutputHelper) : base(new JsonRpcTestOptions().ConfigureForXUnit(testOutputHelper))
        {
        }

        [TheoryWithSkipOn(Skip = "appears to cause a deadlock")]
        [InlineData(Side.Client)]
        [InlineData(Side.Server)]
        public async Task Server_Cannot_Be_Injected_Into_Handler_During_Creation_Using_Registration(Side side)
        {
            Func<Task> a = () => Initialize(
                options => {
                    if (side == Side.Client)
                    {
                        options
                           .AddHandler<InterfaceHandler<IDebugAdapterClient>>()
                           .AddHandler<ClassHandler<DebugAdapterClient>>();
                    }
                },
                options => {
                    if (side == Side.Server)
                    {
                        options
                           .AddHandler<InterfaceHandler<IDebugAdapterServer>>()
                           .AddHandler<ClassHandler<DebugAdapterServer>>();
                    }
                }
            );
            var result = await a.Should().ThrowAsync<ContainerException>();
            result.And.ErrorName.Should().Be("UnableToResolveFromRegisteredServices");
        }

        [TheoryWithSkipOn(Skip = "appears to cause a deadlock")]
        [InlineData(Side.Client)]
        [InlineData(Side.Server)]
        public async Task Server_Cannot_Be_Injected_Into_Handler_During_Creation_Using_Description(Side side)
        {
            Func<Task> a = () => Initialize(
                options => {
                    if (side == Side.Client)
                    {
                        options.Services
                               .AddSingleton(JsonRpcHandlerDescription.Infer(typeof(InterfaceHandler<IDebugAdapterClient>)))
                               .AddSingleton(JsonRpcHandlerDescription.Infer(typeof(ClassHandler<DebugAdapterClient>)));
                    }
                },
                options => {
                    if (side == Side.Server)
                    {
                        options.Services
                               .AddSingleton(JsonRpcHandlerDescription.Infer(typeof(InterfaceHandler<IDebugAdapterServer>)))
                               .AddSingleton(JsonRpcHandlerDescription.Infer(typeof(ClassHandler<DebugAdapterServer>)));
                    }
                }
            );
            var result = await a.Should().ThrowAsync<ContainerException>();
            result.And.ErrorName.Should().Be("UnableToResolveFromRegisteredServices");
        }

        [TheoryWithSkipOn(Skip = "appears to cause a deadlock")]
        [InlineData(Side.Client)]
        [InlineData(Side.Server)]
        public async Task Server_Cannot_Be_Injected_Into_Handler_During_Creation_Using_Injection(Side side)
        {
            Func<Task> a = () => Initialize(
                options => {
                    if (side == Side.Client)
                    {
                        options.Services
                               .AddSingleton<InterfaceHandler<IDebugAdapterClient>>()
                               .AddSingleton<ClassHandler<DebugAdapterClient>>();
                    }
                },
                options => {
                    if (side == Side.Server)
                    {
                        options.Services
                               .AddSingleton<InterfaceHandler<IDebugAdapterServer>>()
                               .AddSingleton<ClassHandler<DebugAdapterServer>>();
                    }
                }
            );
            var result = await a.Should().ThrowAsync<ContainerException>();
            result.And.ErrorName.Should().Be("UnableToResolveFromRegisteredServices");
        }

        [Theory]
        [InlineData(Side.Client)]
        [InlineData(Side.Server)]
        public async Task Server_Facade_Can_Be_Injected_Into_Handler_During_Creation_Using_Registration(Side side)
        {
            Func<Task> a = () => Initialize(
                options => {
                    if (side == Side.Client)
                    {
                        options
                           .AddHandler<InterfaceHandler<IDebugAdapterClientFacade>>();
                    }
                },
                options => {
                    if (side == Side.Server)
                    {
                        options
                           .AddHandler<InterfaceHandler<IDebugAdapterServerFacade>>();
                    }
                }
            );
            await a.Should().NotThrowAsync();
        }

        [Theory]
        [InlineData(Side.Client)]
        [InlineData(Side.Server)]
        public async Task Server_Facade_Can_Be_Injected_Into_Handler_During_Creation_Using_Description(Side side)
        {
            Func<Task> a = () => Initialize(
                options => {
                    if (side == Side.Client)
                    {
                        options.Services
                               .AddSingleton(JsonRpcHandlerDescription.Infer(typeof(InterfaceHandler<IDebugAdapterClientFacade>)));
                    }
                },
                options => {
                    if (side == Side.Server)
                    {
                        options.Services
                               .AddSingleton(JsonRpcHandlerDescription.Infer(typeof(InterfaceHandler<IDebugAdapterServerFacade>)));
                    }
                }
            );
            await a.Should().NotThrowAsync();
        }

        [Theory]
        [InlineData(Side.Client)]
        [InlineData(Side.Server)]
        public async Task Server_Facade_Can_Injected_Into_Handler_During_Creation_Using_Injection(Side side)
        {
            Func<Task> a = () => Initialize(
                options => {
                    if (side == Side.Client)
                    {
                        options.Services
                               .AddSingleton<InterfaceHandler<IDebugAdapterClientFacade>>();
                    }
                },
                options => {
                    if (side == Side.Server)
                    {
                        options.Services
                               .AddSingleton<InterfaceHandler<IDebugAdapterServerFacade>>();
                    }
                }
            );
            await a.Should().NotThrowAsync();
        }

        public enum Side
        {
            Server,
            Client
        }

        [Method(nameof(ClassRequest))]
        public class ClassRequest : IRequest<Unit>
        {
        }

        [Method(nameof(ClassRequest))]
        class ClassHandler<T> : IJsonRpcRequestHandler<ClassRequest, Unit> where T : class
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
        class InterfaceHandler<T> : IJsonRpcRequestHandler<InterfaceRequest, Unit>
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
