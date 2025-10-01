using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Shared;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    internal class ClientCapabilityProvider
    {
        private readonly IHandlerCollection _collection;
        private readonly bool _supportsProgress;

        public ClientCapabilityProvider(
            IHandlerCollection collection,
            bool supportsProgress
        )
        {
            _collection = collection;
            _supportsProgress = supportsProgress;
        }

        public bool HasStaticHandler<T>(Supports<T> capability)
            where T : IDynamicCapability?
        {
            // Dynamic registration will cause us to double register things if we report our capabilities statically.
            // However if the client does not tell us it's capabilities we should just assume that they do not support
            // dynamic registrations but we should report any capabilities statically
            if (capability.IsSupported && capability.Value != null && capability.Value.DynamicRegistration) return false;
            return _collection.Any(z => z.HasCapability && z.CapabilityType == typeof(T));
        }

        public IOptionsGetter GetStaticOptions<T>(Supports<T> capability) where T : DynamicCapability
        {
            return !HasStaticHandler(capability) ? Null : new OptionsGetter(_collection, _supportsProgress);
        }

        private static readonly IOptionsGetter Null = new NullOptionsGetter();

        public interface IOptionsGetter
        {
            /// <summary>
            /// Gets a single option from a given interface.
            /// </summary>
            /// <param name="action"></param>
            /// <typeparam name="TInterface"></typeparam>
            /// <typeparam name="TOptions"></typeparam>
            /// <returns></returns>
            TOptions? Get<TInterface, TOptions>(Func<TInterface, IEnumerable<IHandlerDescriptor>, TOptions> action)
                where TOptions : class
                where TInterface : notnull;

            /// <summary>
            /// Reduces the options from multiple interfaces to a single option.
            /// </summary>
            /// <typeparam name="TInterface"></typeparam>
            /// <typeparam name="TOptions"></typeparam>
            /// <param name="action"></param>
            /// <returns></returns>
            TOptions? Reduce<TInterface, TOptions>(Func<IEnumerable<TInterface>, IEnumerable<IHandlerDescriptor>, TOptions> action)
                where TOptions : class
                where TInterface : notnull;
        }

        private class NullOptionsGetter : IOptionsGetter
        {
            public TOptions? Get<TInterface, TOptions>(Func<TInterface, IEnumerable<IHandlerDescriptor>, TOptions> action)
                where TOptions : class
                where TInterface : notnull
            {
                return null;
            }

            public TOptions? Reduce<TInterface, TOptions>(Func<IEnumerable<TInterface>, IEnumerable<IHandlerDescriptor>, TOptions> action)
                where TOptions : class
                where TInterface : notnull
            {
                return null;
            }
        }

        private class OptionsGetter : IOptionsGetter
        {
            private readonly IHandlerCollection _collection;
            private readonly bool _supportsProgress;

            public OptionsGetter(IHandlerCollection collection, bool supportsProgress)
            {
                _collection = collection;
                _supportsProgress = supportsProgress;
            }

            public TOptions? Get<TInterface, TOptions>(Func<TInterface, IEnumerable<IHandlerDescriptor>, TOptions> action)
                where TOptions : class
                where TInterface : notnull
            {
                var value = _collection
                           .Select(x => x.RegistrationOptions is TInterface cl ? action(cl, _collection) : null)
                           .FirstOrDefault(x => x != null);
                if (value is IWorkDoneProgressOptions wdpo)
                {
                    wdpo.WorkDoneProgress = _supportsProgress;
                }

                return value;
            }

            public TOptions? Reduce<TInterface, TOptions>(Func<IEnumerable<TInterface>, IEnumerable<IHandlerDescriptor>, TOptions> action)
                where TOptions : class
                where TInterface : notnull
            {
                var value = action(
                    _collection
                       .Select(x => x.RegistrationOptions is TInterface cl ? cl : default!)
                       .Where(x => x != null),
                    _collection
                );

                if (value is IWorkDoneProgressOptions wdpo)
                {
                    wdpo.WorkDoneProgress = _supportsProgress;
                }

                return value;
            }
        }
    }
}
