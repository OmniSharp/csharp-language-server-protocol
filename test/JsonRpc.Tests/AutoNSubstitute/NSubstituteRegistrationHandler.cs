using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Builder;
using Autofac.Core;
using MediatR;

// ReSharper disable once CheckNamespace
namespace NSubstitute
{
    /// <summary> Resolves unknown interfaces and Mocks using the <see cref="Substitute" />. </summary>
    internal class NSubstituteRegistrationHandler : IRegistrationSource
    {
        private static readonly IReadOnlyCollection<Type> GenericCollectionTypes = new List<Type>
        {
            typeof(IEnumerable<>),
            typeof(IList<>),
            typeof(IReadOnlyCollection<>),
            typeof(ICollection<>),
            typeof(IReadOnlyList<>)
        };

        /// <summary>
        ///     Retrieve a registration for an unregistered service, to be used
        ///     by the container.
        /// </summary>
        /// <param name="service">The service that was requested.</param>
        /// <param name="registrationAccessor"></param>
        /// <returns>
        ///     Registrations for the service.
        /// </returns>
        public IEnumerable<IComponentRegistration> RegistrationsFor
            (Service service, Func<Service, IEnumerable<IComponentRegistration>> registrationAccessor)
        {
            if (service == null)
                throw new ArgumentNullException(nameof(service));

            if (!(service is IServiceWithType typedService) ||
                !typedService.ServiceType.IsInterface ||
                IsGenericListOrCollectionInterface(typedService.ServiceType) ||
                (typedService.ServiceType.IsGenericType && typedService.ServiceType.GetGenericTypeDefinition() == typeof(IPipelineBehavior<,>)) ||
                typedService.ServiceType.IsArray ||
                typeof(IStartable).IsAssignableFrom(typedService.ServiceType))
                return Enumerable.Empty<IComponentRegistration>();

            var registrations = registrationAccessor(service);
            var componentRegistrations = registrations as IComponentRegistration[] ?? registrations.ToArray();
            if (componentRegistrations.Any())
                return componentRegistrations;

            var rb = RegistrationBuilder.ForDelegate((c, p) => Substitute.For(new[] { typedService.ServiceType }, null))
                .As(service)
                .InstancePerLifetimeScope();

            return new[] { rb.CreateRegistration() };
        }

        public bool IsAdapterForIndividualComponents => false;

        private static bool IsGenericListOrCollectionInterface(Type serviceType)
        {
            return serviceType.IsGenericType && GenericCollectionTypes.Contains(serviceType.GetGenericTypeDefinition());
        }
    }
}
