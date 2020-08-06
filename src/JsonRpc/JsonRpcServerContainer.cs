using System;
using System.IO.Pipelines;
using System.Linq;
using System.Reflection;
using DryIoc;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.JsonRpc.DryIoc;
using OmniSharp.Extensions.JsonRpc.Pipelines;

namespace OmniSharp.Extensions.JsonRpc
{
    internal static class JsonRpcServerContainer
    {
        public static IContainer Create(IServiceProvider outerServiceProvider)
        {
            IContainer container = new Container()
                .WithDependencyInjectionAdapter()
                .With(rules =>
                    rules
                        //.WithMicrosoftDependencyInjectionRules() // disabled to allow for variant generics within jsonrpcservers
                        .WithTrackingDisposableTransients()
                        .WithoutThrowOnRegisteringDisposableTransient()
                        .WithFactorySelector(Rules.SelectLastRegisteredFactory())
                        .WithResolveIEnumerableAsLazyEnumerable()
                        .With(FactoryMethod.ConstructorWithResolvableArgumentsIncludingNonPublic)
                        //.WithDefaultReuse(Reuse.Singleton)
                        .WithDefaultReuse(Reuse.ScopedOrSingleton)
                );

            var outerLoggerFactory = outerServiceProvider?.GetService<ILoggerFactory>();
            if (outerLoggerFactory != null)
            {
                container.RegisterInstance(outerLoggerFactory, IfAlreadyRegistered.Replace);
            }

            if (outerServiceProvider != null)
            {
                container = container.With(rules => rules.WithUnknownServiceResolvers((request) => {                    
                    var value = outerServiceProvider.GetService(request.ServiceType);
                    return value == null ? null : (Factory)new RegisteredInstanceFactory(value);
                }));
            }

            return container;
        }
    }
}
