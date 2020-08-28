using System;
using DryIoc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace OmniSharp.Extensions.JsonRpc
{
    internal static class JsonRpcServerContainer
    {
        public static IContainer Create(IServiceProvider outerServiceProvider)
        {
            var container = new Container()
                           .WithDependencyInjectionAdapter()
                           .With(
                                rules =>
                                    rules
                                       .WithTrackingDisposableTransients()
                                       .WithoutThrowOnRegisteringDisposableTransient()
                                       .WithFactorySelector(Rules.SelectLastRegisteredFactory())
                                       .WithResolveIEnumerableAsLazyEnumerable()
                                       .With(FactoryMethod.ConstructorWithResolvableArgumentsIncludingNonPublic)
                                        //.WithDefaultReuse(Reuse.Singleton)
                                       .WithDefaultReuse(Reuse.Scoped)
                                       .WithConcreteTypeDynamicRegistrations((req, _) => typeof(IJsonRpcHandler).IsAssignableFrom(req), Reuse.Transient)
                            );

            var outerLoggerFactory = outerServiceProvider?.GetService<ILoggerFactory>();
            if (outerLoggerFactory != null)
            {
                container.RegisterInstance(outerLoggerFactory, IfAlreadyRegistered.Replace);
            }

            if (outerServiceProvider != null)
            {
                container.RegisterInstance<IExternalServiceProvider>(new ExternalServiceProvider(outerServiceProvider));
                container = container.With(
                    rules => rules.WithUnknownServiceResolvers(
                        request => {
                            var value = outerServiceProvider.GetService(request.ServiceType);
                            return value == null ? null : (Factory) new RegisteredInstanceFactory(value, Reuse.Transient);
                        }
                    )
                );
            }
            else
            {
                // lets not break folks... just ourselves!
                container.RegisterDelegate<IExternalServiceProvider>(_ => new ExternalServiceProvider(null), Reuse.Singleton);
            }

            return container;
        }
    }
}
