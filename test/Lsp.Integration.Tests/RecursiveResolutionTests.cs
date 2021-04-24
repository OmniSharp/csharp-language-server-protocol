using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DryIoc;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Testing;
using OmniSharp.Extensions.LanguageProtocol.Testing;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Server;
using TestingUtils;
using Xunit;
using Xunit.Abstractions;

namespace Lsp.Tests.Integration
{
    public class RecursiveResolutionTests : LanguageProtocolTestBase
    {
        public RecursiveResolutionTests(ITestOutputHelper testOutputHelper) : base(new JsonRpcTestOptions().ConfigureForXUnit(testOutputHelper))
        {
        }

        [TheoryWithSkipOn(Skip = "appears to cause a deadlock")]
        [InlineData(Side.Client)]
        [InlineData(Side.Server)]
        public async Task Server_Can_Be_Injected_Into_Handler_After_Creation_Using_Registration(Side side)
        {
            Func<Task> a = async () => {
                var (client, server) = await Initialize(
                    options => { },
                    options => { }
                );

                if (side == Side.Client)
                {
                    client.Register(
                        r =>
                            r.AddHandler<InterfaceHandler<ILanguageClient>>()
                             .AddHandler<ClassHandler<LanguageClient>>()
                    );
                }

                if (side == Side.Server)
                {
                    server.Register(
                        r =>
                            r.AddHandler<InterfaceHandler<ILanguageServer>>()
                             .AddHandler<ClassHandler<LanguageServer>>()
                    );
                }
            };
            await a.Should().NotThrowAsync();
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
                           .AddHandler<InterfaceHandler<ILanguageClient>>()
                           .AddHandler<ClassHandler<LanguageClient>>();
                    }
                },
                options => {
                    if (side == Side.Server)
                    {
                        options
                           .AddHandler<InterfaceHandler<ILanguageServer>>()
                           .AddHandler<ClassHandler<LanguageServer>>();
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
                               .AddSingleton(JsonRpcHandlerDescription.Infer(typeof(InterfaceHandler<ILanguageClient>)))
                               .AddSingleton(JsonRpcHandlerDescription.Infer(typeof(ClassHandler<LanguageClient>)));
                    }
                },
                options => {
                    if (side == Side.Server)
                    {
                        options.Services
                               .AddSingleton(JsonRpcHandlerDescription.Infer(typeof(InterfaceHandler<ILanguageServer>)))
                               .AddSingleton(JsonRpcHandlerDescription.Infer(typeof(ClassHandler<LanguageServer>)));
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
                               .AddSingleton<InterfaceHandler<ILanguageClient>>()
                               .AddSingleton<ClassHandler<LanguageClient>>();
                    }
                },
                options => {
                    if (side == Side.Server)
                    {
                        options.Services
                               .AddSingleton<InterfaceHandler<ILanguageServer>>()
                               .AddSingleton<ClassHandler<LanguageServer>>();
                    }
                }
            );
            var result = await a.Should().ThrowAsync<ContainerException>();
            result.And.ErrorName.Should().Be("UnableToResolveFromRegisteredServices");
        }

        [TheoryWithSkipOn(Skip = "appears to cause a deadlock")]
        [InlineData(Side.Client)]
        [InlineData(Side.Server)]
        public async Task Server_Facade_Can_Be_Injected_Into_Handler_During_Creation_Using_Registration(Side side)
        {
            Func<Task> a = () => Initialize(
                options => {
                    if (side == Side.Client)
                    {
                        options
                           .AddHandler<InterfaceHandler<ILanguageClientFacade>>();
                    }
                },
                options => {
                    if (side == Side.Server)
                    {
                        options
                           .AddHandler<InterfaceHandler<ILanguageServerFacade>>();
                    }
                }
            );
            await a.Should().NotThrowAsync();
        }

        [TheoryWithSkipOn(Skip = "appears to cause a deadlock")]
        [InlineData(Side.Client)]
        [InlineData(Side.Server)]
        public async Task Server_Facade_Can_Be_Injected_Into_Handler_During_Creation_Using_Description(Side side)
        {
            Func<Task> a = () => Initialize(
                options => {
                    if (side == Side.Client)
                    {
                        options.Services
                               .AddSingleton(JsonRpcHandlerDescription.Infer(typeof(InterfaceHandler<ILanguageClientFacade>)));
                    }
                },
                options => {
                    if (side == Side.Server)
                    {
                        options.Services
                               .AddSingleton(JsonRpcHandlerDescription.Infer(typeof(InterfaceHandler<ILanguageServerFacade>)));
                    }
                }
            );
            await a.Should().NotThrowAsync();
        }

        [TheoryWithSkipOn(Skip = "appears to cause a deadlock")]
        [InlineData(Side.Client)]
        [InlineData(Side.Server)]
        public async Task Server_Facade_Can_Injected_Into_Handler_During_Creation_Using_Injection(Side side)
        {
            Func<Task> a = () => Initialize(
                options => {
                    if (side == Side.Client)
                    {
                        options.Services
                               .AddSingleton<InterfaceHandler<ILanguageClientFacade>>();
                    }
                },
                options => {
                    if (side == Side.Server)
                    {
                        options.Services
                               .AddSingleton<InterfaceHandler<ILanguageServerFacade>>();
                    }
                }
            );
            await a.Should().NotThrowAsync();
        }

        [TheoryWithSkipOn(Skip = "appears to cause a deadlock")]
        [InlineData(Side.Client)]
        [InlineData(Side.Server)]
        public async Task Should_Allow_Nested_Registration_During_Creation_Using_Registration(Side side)
        {
            var (client, server) = await Initialize(
                options => {
                    if (side == Side.Client)
                    {
                        options.AddHandler<NestedClientHandler>();
                    }
                },
                options => {
                    if (side == Side.Server)
                    {
                        options.AddHandler<NestedServerHandler>();
                    }
                }
            );

            if (side == Side.Client)
            {
                client.GetRequiredService<IHandlersManager>().Descriptors
                      .Where(z => z.Handler.GetType().Assembly == typeof(RecursiveResolutionTests).Assembly)
                      .Should()
                      .HaveCount(2);
            }

            if (side == Side.Server)
            {
                server.GetRequiredService<IHandlersManager>().Descriptors
                      .Where(z => z.Handler.GetType().Assembly == typeof(RecursiveResolutionTests).Assembly)
                      .Should()
                      .HaveCount(2);
            }
        }

        [TheoryWithSkipOn(Skip = "appears to cause a deadlock")]
        [InlineData(Side.Client)]
        [InlineData(Side.Server)]
        public async Task Should_Allow_Nested_Registration_During_Creation_Using_Description(Side side)
        {
            var (client, server) = await Initialize(
                options => {
                    if (side == Side.Client)
                    {
                        options.Services.AddSingleton(JsonRpcHandlerDescription.Infer(typeof(NestedClientHandler)));
                    }
                },
                options => {
                    if (side == Side.Server)
                    {
                        options.Services.AddSingleton(JsonRpcHandlerDescription.Infer(typeof(NestedServerHandler)));
                    }
                }
            );

            if (side == Side.Client)
            {
                client.GetRequiredService<IHandlersManager>().Descriptors
                      .Where(z => z.Handler.GetType().Assembly == typeof(RecursiveResolutionTests).Assembly)
                      .Should()
                      .HaveCount(2);
            }

            if (side == Side.Server)
            {
                server.GetRequiredService<IHandlersManager>().Descriptors
                      .Where(z => z.Handler.GetType().Assembly == typeof(RecursiveResolutionTests).Assembly)
                      .Should()
                      .HaveCount(2);
            }
        }

        [TheoryWithSkipOn(Skip = "appears to cause a deadlock")]
        [InlineData(Side.Client)]
        [InlineData(Side.Server)]
        public async Task Should_Allow_Nested_Registration_During_Creation_Using_Injection(Side side)
        {
            var (client, server) = await Initialize(
                options => {
                    if (side == Side.Client)
                    {
                        options.Services.AddSingleton<NestedClientHandler>();
                    }
                },
                options => {
                    if (side == Side.Server)
                    {
                        options.Services.AddSingleton<NestedServerHandler>();
                    }
                }
            );

            if (side == Side.Client)
            {
                client.GetRequiredService<IHandlersManager>().Descriptors
                      .Where(z => z.Handler.GetType().Assembly == typeof(RecursiveResolutionTests).Assembly)
                      .Should()
                      .HaveCount(2);
            }

            if (side == Side.Server)
            {
                server.GetRequiredService<IHandlersManager>().Descriptors
                      .Where(z => z.Handler.GetType().Assembly == typeof(RecursiveResolutionTests).Assembly)
                      .Should()
                      .HaveCount(2);
            }
        }

        public enum Side
        {
            Server,
            Client
        }

        [Method(nameof(ClassRequest))]
        public class ClassRequest : IRequest
        {
        }

        [Method(nameof(ClassRequest))]
        class ClassHandler<T> : IJsonRpcRequestHandler<ClassRequest, Unit> where T : class
        {
            // ReSharper disable once NotAccessedField.Local
            private readonly T _jsonRpcServer;

            public ClassHandler(T jsonRpcServer)
            {
                _jsonRpcServer = jsonRpcServer;
            }

            public Task<Unit> Handle(ClassRequest classRequest, CancellationToken cancellationToken) => throw new NotImplementedException();
        }

        [Method(nameof(InterfaceRequest))]
        public class InterfaceRequest : IRequest
        {
        }

        [Method(nameof(InterfaceRequest))]
        class InterfaceHandler<T> : IJsonRpcRequestHandler<InterfaceRequest, Unit>
        {
            // ReSharper disable once NotAccessedField.Local
            private readonly T _jsonRpcServer;

            public InterfaceHandler(T jsonRpcServer)
            {
                _jsonRpcServer = jsonRpcServer;
            }

            public Task<Unit> Handle(InterfaceRequest request, CancellationToken cancellationToken) => throw new NotImplementedException();
        }

        [Method(nameof(NestedRequest))]
        public class NestedRequest : IRequest
        {
        }

        [Method(nameof(NestedRequest))]
        class NestedClientHandler : IJsonRpcRequestHandler<NestedRequest, Unit>
        {
            // ReSharper disable once NotAccessedField.Local
            private readonly ILanguageClientFacade _languageClientFacade;

            public NestedClientHandler(ILanguageClientFacade languageClientFacade)
            {
                _languageClientFacade = languageClientFacade;
                languageClientFacade.Register(r => r.AddHandler<InterfaceHandler<ILanguageClientFacade>>());
            }

            public Task<Unit> Handle(NestedRequest request, CancellationToken cancellationToken) => throw new NotImplementedException();
        }

        [Method(nameof(NestedRequest))]
        class NestedServerHandler : IJsonRpcRequestHandler<NestedRequest, Unit>
        {
            // ReSharper disable once NotAccessedField.Local
            private readonly ILanguageServerFacade _languageClientFacade;

            public NestedServerHandler(ILanguageServerFacade languageClientFacade)
            {
                _languageClientFacade = languageClientFacade;
                languageClientFacade.Register(z => z.AddHandler<InterfaceHandler<ILanguageServerFacade>>());
            }

            public Task<Unit> Handle(NestedRequest request, CancellationToken cancellationToken) => throw new NotImplementedException();
        }
    }
}
