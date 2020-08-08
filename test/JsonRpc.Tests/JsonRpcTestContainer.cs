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
                .With(rules => rules.WithDefaultReuse(Reuse.ScopedOrSingleton));

            var services = new ServiceCollection().AddLogging().AddOptions();
            container.Populate(services);
            container.RegisterInstance(testOutputHelper);
            container.RegisterMany<TestLoggerFactory>(nonPublicServiceTypes: true);
            container.RegisterMany<JsonRpcSerializer>();
            return container;
        }
    }
}
