using DryIoc;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Serialization;
using Xunit.Abstractions;
using NSubstitute.Internals;

namespace JsonRpc.Tests
{
    internal static class JsonRpcTestContainer
    {
        public static IContainer Create(ITestOutputHelper testOutputHelper)
        {
            var container = JsonRpcServerContainer.Create(null)
                .AddJsonRpcMediatR()
                .With(rules => rules
                    .WithTestLoggerResolver((request, loggerType) => ActivatorUtilities.CreateInstance(request.Container, loggerType))
                    .WithUndefinedTestDependenciesResolver(request => Substitute.For(new[] { request.ServiceType }, null))
                    .WithConcreteTypeDynamicRegistrations((type, o) => true, Reuse.Transient)
            );
            container.RegisterMany<JsonRpcSerializer>();
            return container;
        }
    }
}
